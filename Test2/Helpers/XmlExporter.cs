using System.Text;
using System.Xml;
using Test2.Model;
using System.IO;
 
namespace Test2.Helpers
    {
        public sealed class XmlExporter : IExporter
        {
            public string Format => "XML";
            public string Extension => "xml";

            public async Task ExportAsync(
                IAsyncEnumerable<TaskModel> tasks,
                string filePath,
                CancellationToken ct = default)
            {
                await using var stream = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 64 * 1024,
                    useAsync: true);

                var settings = new XmlWriterSettings
                {
                    Async = true,
                    Indent = true,
                    Encoding = new UTF8Encoding(false),
                    CloseOutput = false
                };

                await using var writer = XmlWriter.Create(stream, settings);

                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "Tasks", null);

                await foreach (var task in tasks.WithCancellation(ct))
                {
                    await writer.WriteStartElementAsync(null, "Task", null);
                    await writer.WriteElementStringAsync(null, "Id", null, task.Id.ToString());
                    await writer.WriteElementStringAsync(null, "Date", null,
                        task.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    await writer.WriteElementStringAsync(null, "Name", null, task.Name ?? "");
                    await writer.WriteElementStringAsync(null, "LastName", null, task.LastName ?? "");
                    await writer.WriteElementStringAsync(null, "MiddleName", null, task.MiddleName ?? "");
                    await writer.WriteElementStringAsync(null, "City", null, task.City ?? "");
                    await writer.WriteElementStringAsync(null, "Country", null, task.Country ?? "");
                    await writer.WriteEndElementAsync();
                }

                await writer.WriteEndElementAsync();
                await writer.WriteEndDocumentAsync();
                await writer.FlushAsync();
            }
        }
    }