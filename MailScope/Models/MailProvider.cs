using System;
using System.Collections.Generic;
using System.Text;

namespace EmailAnalyzer.Models;

public class MailProvider
{
    /// <summary>
    /// 邮箱显示名称
    /// </summary>
    public string Name { get; set; } = "";


    /// <summary>
    /// IMAP服务器
    /// </summary>
    public string ImapHost { get; set; } = "";


    /// <summary>
    /// IMAP端口
    /// </summary>
    public int Port { get; set; } = 993;


    /// <summary>
    /// SSL连接
    /// </summary>
    public bool UseSsl { get; set; } = true;


    /// <summary>
    /// 登录提示
    /// </summary>
    public string Description { get; set; } = "";



    public override string ToString()
    {
        return Name;
    }
}
