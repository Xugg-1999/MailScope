using EmailAnalyzer.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
namespace EmailAnalyzer.Services;

public class ExcelService
{
    public void Export(
        List<EmailContact> list
    )
    {
        ExcelPackage.LicenseContext =
            OfficeOpenXml.LicenseContext.NonCommercial;
        using var package =
            new ExcelPackage();
        var sheet =
            package.Workbook.Worksheets
            .Add("联系人排行");
        sheet.Cells[1, 1].Value = "邮箱";
        sheet.Cells[1, 2].Value = "交流次数";
        sheet.Cells[1, 3].Value = "最近联系";
        int row = 2;
        foreach (var item in list)
        {
            sheet.Cells[row, 1].Value =item.Email;
            sheet.Cells[row, 2].Value =item.Count;
            sheet.Cells[row, 3].Value =item.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            row++;
        }
        //格式
        sheet.Cells.AutoFitColumns();
        sheet.Column(1).Width = 35;
        sheet.Column(2).Width = 15;
        sheet.Column(3).Width = 25;
        //保存桌面
        var desktop =
            Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop
            );
        var file =
            Path.Combine(
                desktop,
                "SGRS邮箱联系人统计.xlsx"
            );
        package.SaveAs(
            new FileInfo(file)
        );
    }
}