namespace EmailAnalyzer.Models;
public class ScanStatus
{
    public long TotalMail { get; set; }
    public long FinishedMail { get; set; }
    public int ContactCount { get; set; }
    public string CurrentFolder { get; set; } = "";
    /// <summary>
    /// 每秒扫描数量
    /// </summary>
    public double Speed { get; set; }
    /// <summary>
    /// 百分比
    /// </summary>
    public double Progress
    {
        get
        {
            if (TotalMail <= 0)
                return 0;
            return
                FinishedMail * 100.0
                /
                TotalMail;
        }
    }
    /// <summary>
    /// 预计剩余秒数
    /// </summary>
    public int RemainingSeconds
    {
        get
        {
            if (Speed <= 0)
                return 0;
            var remain =
                TotalMail - FinishedMail;
            return (int)
            (
                remain / Speed
            );
        }
    }
}
//namespace EmailAnalyzer.Models;
//public class ScanStatus
//{
//    public long TotalMail { get; set; }
//    public long FinishedMail { get; set; }
//    public int ContactCount { get; set; }
//    public string CurrentFolder { get; set; } = "";
//    public double Speed { get; set; }
//    public int Progress
//    {
//        get
//        {
//            if (TotalMail == 0)
//                return 0;
//            return (int)
//            (
//                FinishedMail * 100.0 / TotalMail
//            );
//        }
//    }
//}