using EmailAnalyzer.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
namespace EmailAnalyzer.Services;
public class MailService
{
    private MailProvider? _provider;
    private string _account = "";
    private string _password = "";
    private Action<string>? _log;
    private ImapClient? _client;
    private readonly Dictionary<string, EmailContact> _contacts = new();
    private const int BatchSize = 100;
    private const int ReconnectInterval = 1000;
    private long _finishedCount;
    private Stopwatch _stopwatch = new();
    private int _lastReconnectPosition = -1;
    public async Task<List<EmailContact>> ScanAsync
    (
        MailProvider provider,
        string account,
        string password,
        Action<string>? log = null,
        Action<ScanStatus>? statusCallback = null,
        CancellationToken token = default
    )
    {
        _provider = provider;
        _account = account;
        _password = password;
        _log = log;
        _contacts.Clear();
        _finishedCount = 0;
        _stopwatch.Restart();
        try
        {
            await ConnectAsync();
            var folders =
                await GetTargetFoldersAsync();
            long total = 0;
            foreach (var folder in folders)
            {
                await folder.OpenAsync(
                    FolderAccess.ReadOnly,
                    token
                );
                total += folder.Count;
            }
            _log?.Invoke(
                $"待分析邮件:{total}封"
            );
            foreach (var folder in folders)
            {
                await ScanFolderAsync(
                    folder.FullName,
                    total,
                    statusCallback,
                    token
                );
            }
        }
        catch (OperationCanceledException)
        {
            _log?.Invoke(
                "用户取消"
            );
        }
        catch (Exception ex)
        {
            _log?.Invoke(
                $"分析异常:{ex.Message}"
            );
        }
        finally
        {
            await DisconnectAsync();
        }
        _log?.Invoke(
            $"分析完成，共发现联系人:{_contacts.Count}"
        );
        return _contacts.Values
            .OrderByDescending(x => x.Count)
            .ToList();
    }
    private async Task ScanFolderAsync
    (
        string folderName,
        long totalMail,
        Action<ScanStatus>? callback,
        CancellationToken token
    )
    {
        int current = 0;
        while (true)
        {
            await EnsureConnection();
            var folder =
                await GetFolderAsync(folderName);
            await folder.OpenAsync(
                FolderAccess.ReadOnly,
                token
            );
            if (current >= folder.Count)
                break;
            if (
                current > 0 &&
                current % ReconnectInterval == 0 &&
                current != _lastReconnectPosition
               )
            {
                _log?.Invoke(
                    "重新连接IMAP..."
                );
                _lastReconnectPosition =
                    current;
                await ReconnectAsync();
                continue;
            }
            int end =
                Math.Min(
                    current + BatchSize - 1,
                    folder.Count - 1
                );
            IList<IMessageSummary>? messages = null;
            try
            {
                messages =
                await folder.FetchAsync(
                    current,
                    end,
                    MessageSummaryItems.Envelope,
                    token
                );
            }
            catch (Exception ex)
            {
                _log?.Invoke(
                    $"读取失败:{ex.Message}"
                );
                await ReconnectAsync();
                continue;
            }
            foreach (var msg in messages)
            {
                var env =
                    msg.Envelope;
                if (env == null)
                    continue;
                AddAddresses(
                    env.From,
                    env.Date
                );
                AddAddresses(
                    env.To,
                    env.Date
                );
                AddAddresses(
                    env.Cc,
                    env.Date
                );
                _finishedCount++;
            }
            current =
                end + 1;
            var seconds =
    _stopwatch.Elapsed.TotalSeconds;
            var speed =
                seconds <= 0
                ?
                0
                :
                _finishedCount / seconds;
            callback?.Invoke(
                new ScanStatus
                {
                    TotalMail = totalMail,
                    FinishedMail = _finishedCount,
                    ContactCount = _contacts.Count,
                    CurrentFolder = folder.Name,
                    Speed = Math.Round(
                        speed,
                        2
                    )
                }
            );
            //callback?.Invoke(
            //    new ScanStatus
            //    {
            //        TotalMail = totalMail,
            //        FinishedMail = _finishedCount,
            //        ContactCount = _contacts.Count,
            //        CurrentFolder = folder.Name,
            //        Speed = 0
            //    }
            //);
            _log?.Invoke(
                $"{folder.Name}:{current}/{folder.Count}"
            );
        }
        _log?.Invoke(
            $"{folderName}扫描完成"
        );
    }
    private async Task ConnectAsync()
    {
        _client =
            new ImapClient();
        _client.Timeout =
            120000;
        await _client.ConnectAsync(
            _provider!.ImapHost,
            _provider.Port,
            _provider.UseSsl
            ?
            SecureSocketOptions.SslOnConnect
            :
            SecureSocketOptions.None
        );
        await _client.AuthenticateAsync(
            _account,
            _password
        );
        _log?.Invoke(
            "IMAP连接成功"
        );
    }
    private async Task EnsureConnection()
    {
        if (
            _client == null
            ||
            !_client.IsConnected
            ||
            !_client.IsAuthenticated
        )
        {
            await ReconnectAsync();
        }
    }
    private async Task ReconnectAsync()
    {
        await DisconnectAsync();
        await Task.Delay(1000);
        await ConnectAsync();
    }
    private async Task DisconnectAsync()
    {
        try
        {
            if (_client != null)
            {
                if (_client.IsConnected)
                {
                    await _client.DisconnectAsync(true);
                }
                _client.Dispose();
            }
        }
        catch
        {
        }
        _client = null;
    }
    private async Task<List<IMailFolder>> GetTargetFoldersAsync()
    {
        var result =
            new List<IMailFolder>();
        result.Add(
            _client!.Inbox
        );
        var folders =
            await _client.GetFoldersAsync(
                _client.PersonalNamespaces[0]
            );
        var sent =
            folders.FirstOrDefault(
                x =>
                IsSentFolder(x.Name)
            );
        if (sent != null)
        {
            result.Add(sent);
        }
        return result;
    }
    private bool IsSentFolder(string name)
    {
        string[] keys =
        {
            "sent",
            "sent items",
            "sent mail",
            "已发送",
            "发件箱",
            "发送邮件"
        };
        return keys.Any(
            x =>
            name.Contains(
                x,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }
    private async Task<IMailFolder> GetFolderAsync(
        string name
    )
    {
        var folders =
            await _client!.GetFoldersAsync(
                _client.PersonalNamespaces[0]
            );
        var folder =
            folders.FirstOrDefault(
                x =>
                x.FullName == name
            );
        if (folder == null)
        {
            throw new Exception(
                $"找不到目录:{name}"
            );
        }
        return folder;
    }
    private void AddAddresses
    (
        InternetAddressList list,
        DateTimeOffset? date
    )
    {
        foreach (var item in list)
        {
            if (item is MailboxAddress box)
            {
                AddEmail(
                    box.Address,
                    date
                );
            }
        }
    }
    private void AddEmail
    (
        string email,
        DateTimeOffset? date
    )
    {
        if (string.IsNullOrWhiteSpace(email))
            return;
        email =
            email.ToLower();
        if (
            !Regex.IsMatch(
                email,
                @"^[\w\.-]+@[\w\.-]+\.\w+$"
            )
        )
            return;
        if (!_contacts.ContainsKey(email))
        {
            _contacts[email] =
            new EmailContact
            {
                Email = email,
                Count = 1,
                LastTime =
                    date?.LocalDateTime
                    ??
                    DateTime.Now
            };
        }
        else
        {
            _contacts[email].Count++;
            if (
                date.HasValue &&
                date.Value.LocalDateTime >
                _contacts[email].LastTime
            )
            {
                _contacts[email].LastTime =
                    date.Value.LocalDateTime;
            }
        }
    }
}
