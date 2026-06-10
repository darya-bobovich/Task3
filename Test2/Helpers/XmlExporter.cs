using System.Xml.Linq;
using Test2.Model;

namespace Test2.Helpers
{
    public class XmlExporter
    {
        public async Task ExportAsync(IEnumerable<TaskModel> tasks, string filePath)
        {
            await Task.Run(() =>
            {
                var root = new XElement("Tasks");

                foreach (var task in tasks)
                {
                    root.Add(new XElement("Task",
                        new XElement("Id", task.Id),
                        new XElement("Date", task.Date.ToString("yyyy-MM-dd HH:mm:ss")),
                        new XElement("Name", task.Name ?? ""),
                        new XElement("LastName", task.LastName ?? ""),
                        new XElement("MiddleName", task.MiddleName ?? ""),
                        new XElement("City", task.City ?? ""),
                        new XElement("Country", task.Country ?? "")
                    ));
                }

                var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), root);
                doc.Save(filePath);
            });
        }
    }
}