using EmailAnalyzer.Models;
using System.Collections.Generic;

namespace EmailAnalyzer.Services;


public static class MailProviderService
{

    public static List<MailProvider> Providers { get; } =
    [
        new MailProvider
        {
            Name = "阿里云企业邮箱",
            ImapHost = "imap.qiye.aliyun.com",
            Port = 993,
            UseSsl = true,
            Description =
            "阿里云企业邮箱，请使用邮箱密码或授权码"
        },


        new MailProvider
        {
            Name = "腾讯企业邮箱",
            ImapHost = "imap.exmail.qq.com",
            Port = 993,
            UseSsl = true,
            Description =
            "腾讯企业邮箱，请开启IMAP服务"
        },


        new MailProvider
        {
            Name = "QQ邮箱",
            ImapHost = "imap.qq.com",
            Port = 993,
            UseSsl = true,
            Description =
            "QQ邮箱需要使用授权码"
        },


        new MailProvider
        {
            Name = "163邮箱",
            ImapHost = "imap.163.com",
            Port = 993,
            UseSsl = true,
            Description =
            "163邮箱需要开启IMAP"
        },


        new MailProvider
        {
            Name = "126邮箱",
            ImapHost = "imap.126.com",
            Port = 993,
            UseSsl = true,
            Description =
            "126邮箱需要开启IMAP"
        },


        new MailProvider
        {
            Name = "Outlook / Hotmail",
            ImapHost = "outlook.office365.com",
            Port = 993,
            UseSsl = true,
            Description =
            "Microsoft邮箱建议使用应用密码"
        },


        new MailProvider
        {
            Name = "Gmail",
            ImapHost = "imap.gmail.com",
            Port = 993,
            UseSsl = true,
            Description =
            "Gmail需要开启IMAP并使用应用密码"
        },


        new MailProvider
        {
            Name = "自定义IMAP",
            ImapHost = "",
            Port = 993,
            UseSsl = true,
            Description =
            "手动输入IMAP服务器地址"
        }
    ];

}