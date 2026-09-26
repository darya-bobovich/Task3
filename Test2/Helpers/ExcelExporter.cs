using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Test2.Model;

namespace Test2.Helpers
{
    public sealed class ExcelExporter : IExporter
    {
        public string Format => "Excel";
        public string Extension => "xlsx";

        public Task ExportAsync(
            IAsyncEnumerable<TaskModel> tasks,
            string filePath,
            CancellationToken ct = default)
        {
            using var document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = new Stylesheet(
                new Fonts(new Font()),
                new Fills(new Fill()),
                new Borders(new Border()),
                new CellFormats(new CellFormat()));
            stylesPart.Stylesheet.Save();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var writer = OpenXmlWriter.Create(worksheetPart);

            writer.WriteStartElement(new Worksheet());
            writer.WriteStartElement(new SheetData());

            writer.WriteStartElement(new Row());
            WriteCell(writer, "ID");
            WriteCell(writer, "Дата");
            WriteCell(writer, "Имя");
            WriteCell(writer, "Фамилия");
            WriteCell(writer, "Отчество");
            WriteCell(writer, "Город");
            WriteCell(writer, "Страна");
            writer.WriteEndElement();

            var enumerator = tasks.GetAsyncEnumerator(ct);
            try
            {
                while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
                {
                    var task = enumerator.Current;
                    writer.WriteStartElement(new Row());
                    WriteCell(writer, task.Id.ToString());
                    WriteCell(writer, task.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    WriteCell(writer, task.Name ?? "");
                    WriteCell(writer, task.LastName ?? "");
                    WriteCell(writer, task.MiddleName ?? "");
                    WriteCell(writer, task.City ?? "");
                    WriteCell(writer, task.Country ?? "");
                    writer.WriteEndElement();
                }
            }
            finally
            {
                enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Tasks"
            });

            workbookPart.Workbook.Save();
            return Task.CompletedTask;
        }

        private static void WriteCell(OpenXmlWriter writer, string value)
        {
            writer.WriteStartElement(new Cell { DataType = CellValues.InlineString });
            writer.WriteElement(new InlineString(new Text(value)));
            writer.WriteEndElement();
        }
    }
}