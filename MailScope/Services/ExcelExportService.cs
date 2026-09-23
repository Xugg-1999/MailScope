using EmailAnalyzer.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace EmailAnalyzer.Services;
public class ExcelExportService
{
    public void Export(
        string filePath,
        IEnumerable<EmailContact> contacts
    )
    {
        ExcelPackage.LicenseContext =LicenseContext.NonCommercial;
        using var package =new ExcelPackage();
        CreateContactSheet(package,contacts);
        CreateDomainSheet(package,contacts);
        package.SaveAs(new FileInfo(filePath));
    }
    private void CreateContactSheet( ExcelPackage package, IEnumerable<EmailContact> contacts )
    {
        var sheet =package.Workbook.Worksheets.Add("联系人");
        sheet.Cells[1, 1].Value = "邮箱";
        sheet.Cells[1, 2].Value = "次数";
        sheet.Cells[1, 3].Value = "最近联系";
        sheet.Cells[1, 4].Value = "域名";
        sheet.Cells[1, 5].Value = "邮箱类型";
        int row = 2;

        foreach (var item in contacts)
        {
            sheet.Cells[row, 1].Value =item.Email;
            sheet.Cells[row, 2].Value =item.Count;
            // 最近联系时间
            sheet.Cells[row, 3].Value =item.LastTime;
            sheet.Cells[row, 3].Style.Numberformat.Format ="yyyy-mm-dd hh:mm:ss";
            sheet.Cells[row, 4].Value =item.Domain;
            sheet.Cells[row, 5].Value =item.ProviderType;
            row++;
        }

        FormatSheet(sheet);
    }
   
    private void CreateDomainSheet (ExcelPackage package,IEnumerable<EmailContact> contacts )
    {
        var sheet =package.Workbook.Worksheets.Add("域名统计");
        sheet.Cells[1, 1].Value ="域名";
        sheet.Cells[1, 2].Value ="数量";
        var groups =contacts
            .GroupBy(x => x.Domain)
            .Select(x => new
            {
                Domain = x.Key,
                Count = x.Count()
            })
            .OrderByDescending(
                x => x.Count
            );
        int row = 2;
        foreach (var item in groups)
        {
            sheet.Cells[row, 1].Value =
                item.Domain;
            sheet.Cells[row, 2].Value =
                item.Count;
            row++;
        }
        FormatSheet(sheet);
    }
    private void FormatSheet(ExcelWorksheet sheet)
    {
        sheet.Cells.AutoFitColumns();
        using var range =
            sheet.Cells[1, 1, 1, sheet.Dimension.Columns];
        range.Style.Font.Bold =
            true;
    }
}