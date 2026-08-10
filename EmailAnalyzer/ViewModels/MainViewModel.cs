using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmailAnalyzer.Models;
using EmailAnalyzer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmailAnalyzer.Models;
using EmailAnalyzer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
namespace EmailAnalyzer.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly MailService _mailService;
    private readonly ExcelExportService _excelService;
    private ICollectionView? _contactsView;
    private CancellationTokenSource? _cts;
    public MainViewModel()
    {
        _mailService =
            new MailService();
        _excelService =
            new ExcelExportService();
        Providers =
            new ObservableCollection<MailProvider>(
                MailProviderService.Providers
            );
        SelectedProvider =
            Providers.First();
        _contactsView =
            CollectionViewSource.GetDefaultView(
                Contacts
            );
        _contactsView.Filter =
            FilterContacts;
    }
    #region 邮箱配置
    [ObservableProperty]
    private ObservableCollection<MailProvider> providers;
    [ObservableProperty]
    private MailProvider selectedProvider;
    [ObservableProperty]
    private string account = "";
    [ObservableProperty]
    private string password = "";
    #endregion
    #region 状态
    [ObservableProperty]
    private bool isRunning;
    public string StartButtonText
    {
        get
        {
            return IsRunning
                ?
                "⏳分析中..."
                :
                "🚀开始";
        }
    }
    partial void OnIsRunningChanged(
        bool value)
    {
        OnPropertyChanged(
            nameof(StartButtonText)
        );
    }
    #endregion
    #region 数据
    public ObservableCollection<EmailContact> Contacts
    {
        get;
    }
    =
    new();
    public ICollectionView ContactsView
    {
        get
        {
            return _contactsView!;
        }
    }
    [ObservableProperty]
    private string searchText = "";
    [ObservableProperty]
    private int contactCount;
    [ObservableProperty]
    private long totalMail;
    [ObservableProperty]
    private string speed = "0 封/秒";
    [ObservableProperty]
    private string status = "等待";
    [ObservableProperty]
    private string log = "";
    [ObservableProperty]
    private string remaining = "";
    [ObservableProperty]
    private double progress;
    [ObservableProperty]
    private string currentFolder = "";
    #endregion
    #region 开始扫描
    [RelayCommand]
    public async Task StartScanAsync()
    {
        if (IsRunning)
            return;
        if (string.IsNullOrWhiteSpace(Account))
        {
            AddLog(
                "请输入邮箱账号"
            );
            return;
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            AddLog(
                "请输入密码或授权码"
            );
            return;
        }
        try
        {
            IsRunning = true;
            _cts =
                new CancellationTokenSource();
            Contacts.Clear();
            ContactCount = 0;
            Status =
                "正在连接";
            Progress = 0;
            AddLog(
                "开始分析邮件..."
            );
            var result =
                await _mailService.ScanAsync(
                    SelectedProvider,
                    Account,
                    Password,
                    AddLog,
                    UpdateStatus,
                    _cts.Token
                );
            foreach (var item in result)
            {
                Contacts.Add(item);
            }
            _contactsView?.Refresh();
            ContactCount =
                Contacts.Count;
            Status =
                "完成";
            AddLog(
                "分析完成"
            );
        }
        catch (OperationCanceledException)
        {
            Status =
                "已取消";
            AddLog(
                "用户取消扫描"
            );
        }
        catch (Exception ex)
        {
            Status =
                "异常";
            AddLog(
                ex.Message
            );
        }
        finally
        {
            IsRunning = false;
            _cts?.Dispose();
            _cts = null;
        }
    }
    #endregion
    #region 取消扫描
    [RelayCommand]
    public void CancelScan()
    {
        if (!IsRunning)
            return;
        AddLog(
            "正在取消扫描..."
        );
        _cts?.Cancel();
    }
    #endregion
    #region 状态更新
    private void UpdateStatus(
        ScanStatus status)
    {
        TotalMail =
            status.TotalMail;
        ContactCount =
            status.ContactCount;
        CurrentFolder =
            status.CurrentFolder;
        Status =
            status.CurrentFolder;
        Speed =
            $"{status.Speed} 封/秒";
        Remaining =
            $"预计剩余:{status.RemainingSeconds}秒";
        Progress =
            status.Progress;
    }
    #endregion
    #region 日志
    private void AddLog(
        string message)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            Log +=
                DateTime.Now.ToString(
                    "HH:mm:ss"
                )
                +
                " "
                +
                message
                +
                Environment.NewLine;
        });
    }
    #endregion
    #region Excel
    [RelayCommand]
    public void ExportExcel()
    {
        if (Contacts.Count == 0)
        {
            AddLog(
                "暂无联系人数据"
            );
            return;
        }
        var dialog =
            new Microsoft.Win32.SaveFileDialog
            {
                Filter =
                "Excel文件|*.xlsx",
                FileName =
                "EmailContacts.xlsx"
            };
        if (dialog.ShowDialog() == true)
        {
            _excelService.Export(
                dialog.FileName,
                Contacts
            );
            AddLog(
                "Excel导出完成"
            );
        }
    }
    #endregion
    #region 搜索过滤
    partial void OnSearchTextChanged(
        string value)
    {
        _contactsView?.Refresh();
    }
    private bool FilterContacts(
        object obj)
    {
        if (obj is not EmailContact item)
            return false;
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;
        return
            item.Email.Contains(
                SearchText,
                StringComparison.OrdinalIgnoreCase
            )
            ||
            item.Domain.Contains(
                SearchText,
                StringComparison.OrdinalIgnoreCase
            );
    }
    #endregion
}
