using System;
namespace EmailAnalyzer.Models;
public class EmailContact
{
    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Email { get; set; } = "";
    /// <summary>
    /// 往来次数
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    /// 最近联系时间
    /// </summary>
    public DateTime LastTime { get; set; }
    /// <summary>
    /// 邮箱域名
    /// </summary>
    public string Domain
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Email))
                return "";
            var index =
                Email.IndexOf("@");
            if (index < 0)
                return "";
            return Email[(index + 1)..];
        }
    }
    /// <summary>
    /// 邮箱类型
    /// </summary>
    public string ProviderType
    {
        get
        {
            var domain =
                Domain.ToLower();
            if (domain.Contains("qq"))
                return "QQ邮箱";
            if (domain.Contains("163"))
                return "163邮箱";
            if (domain.Contains("126"))
                return "126邮箱";
            if (domain.Contains("gmail"))
                return "Gmail";
            if (domain.Contains("outlook")
                ||
                domain.Contains("hotmail"))
                return "Outlook";
            return "企业/其他";
        }
    }
}
//namespace EmailAnalyzer.Models;
//public class EmailContact
//{
//    public string Email { get; set; } = "";
//    public int Count { get; set; }
//    public DateTime LastTime { get; set; }
//}