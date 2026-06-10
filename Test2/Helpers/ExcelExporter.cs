using Test2.Model;
using ClosedXML.Excel;

namespace Test2.Helpers
{
    public class ExcelExporter
    {
        public async Task ExportAsync(IEnumerable<TaskModel> tasks, string filePath)
        {
            await Task.Run(() =>
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Tasks");
                    var headers = new[] { "ID", "Дата", "Имя", "Фамилия", "Отчество", "Город", "Страна" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    var headerRange = worksheet.Range(1, 1, 1, headers.Length);
                    headerRange.Style.Font.Bold = true;                          
                    headerRange.Style.Font.FontColor = XLColor.Black;           
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; 
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;    

                    int row = 2;
                    foreach (var task in tasks)
                    {
                        worksheet.Cell(row, 1).Value = task.Id;                                    
                        worksheet.Cell(row, 2).Value = task.Date.ToString("yyyy-MM-dd HH:mm:ss"); 
                        worksheet.Cell(row, 3).Value = task.Name ?? "";                           
                        worksheet.Cell(row, 4).Value = task.LastName ?? "";                       
                        worksheet.Cell(row, 5).Value = task.MiddleName ?? "";                    
                        worksheet.Cell(row, 6).Value = task.City ?? "";                          
                        worksheet.Cell(row, 7).Value = task.Country ?? "";                       
                        row++;
                    }

                    var dataRange = worksheet.Range(1, 1, row - 1, headers.Length);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; 
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin; 

                    worksheet.Columns().AdjustToContents();

                    for (int i = 1; i <= headers.Length; i++)
                    {
                        if (worksheet.Column(i).Width < 15)
                        {
                            worksheet.Column(i).Width = 15;
                        }
                    }

                    worksheet.Row(1).Height = 25;
                    worksheet.SheetView.FreezeRows(1);
                    workbook.SaveAs(filePath);
                }
            });
        }
    }
}