using System.Xml.Linq;
using Test2.Helpers;
using Test2.Model;

namespace Test2.Tests.Helpers
{
    public class XmlExporterTests
    {
        private readonly XmlExporter _exporter;

        public XmlExporterTests()
        {
            _exporter = new XmlExporter();
        }

        // Тест 1 Проверка, что XML файл создается
        [Fact]
        public async Task ExportAsync_ValidTasks_CreatesXmlFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xml";
            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Иван", Date = DateTime.Now }
            };

            // Act
            await _exporter.ExportAsync(ToAsync(tasks), tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));
            Assert.True(new FileInfo(tempFile).Length > 0);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 2 Проверка структуры XML
        [Fact]
        public async Task ExportAsync_ValidTasks_CreatesValidXml()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xml";
            var tasks = new List<TaskModel>
            {
                new()
                {
                    Id = 1,
                    Name = "Иван",
                    LastName = "Иванов",
                    City = "Гомель",
                    Date = new DateTime(2026, 1, 15, 10, 30, 0)
                }
            };

            // Act
            await _exporter.ExportAsync(ToAsync(tasks), tempFile);

            // Assert
            var xml = XDocument.Load(tempFile);

            // Проверка корневого элемента
            Assert.Equal("Tasks", xml.Root.Name);

            // Проверка количества задач
            Assert.Single(xml.Root.Elements("Task"));

            // Проверка данных
            var task = xml.Root.Elements("Task").First();
            Assert.Equal("1", task.Element("Id").Value);
            Assert.Equal("Иван", task.Element("Name").Value);
            Assert.Equal("Иванов", task.Element("LastName").Value);
            Assert.Equal("Гомель", task.Element("City").Value);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 3 Проверка обработки пустого списка
        // Экспортер теперь создает пустой XML, а не кидает исключение
        [Fact]
        public async Task ExportAsync_EmptyTasks_CreatesEmptyXml()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xml";
            var tasks = new List<TaskModel>();

            // Act
            await _exporter.ExportAsync(ToAsync(tasks), tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 4 Проверка обработки null значений 
        [Fact]
        public async Task ExportAsync_TasksWithNullFields_HandlesNullValues()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xml";
            var tasks = new List<TaskModel>
            {
                new()
                {
                    Id = 1,
                    Name = null,
                    LastName = null,
                    Date = new DateTime(2026, 1, 15, 10, 30, 0)
                }
            };

            // Act
            await _exporter.ExportAsync(ToAsync(tasks), tempFile);

            // Assert
            var xml = XDocument.Load(tempFile);
            var task = xml.Root.Elements("Task").First();

            // null должен стать пустой строкой
            Assert.Equal("", task.Element("Name").Value);
            Assert.Equal("", task.Element("LastName").Value);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 5 Проверка XML декларации
        [Fact]
        public async Task ExportAsync_CreatesValidXmlDeclaration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xml";
            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Test", Date = DateTime.Now }
            };

            // Act
            await _exporter.ExportAsync(ToAsync(tasks), tempFile);

            // Assert
            var xmlContent = await File.ReadAllTextAsync(tempFile);
            Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>", xmlContent);

            // Cleanup
            File.Delete(tempFile);
        }

        // превращает список в IAsyncEnumerable для экспортера
        private static async IAsyncEnumerable<TaskModel> ToAsync(IEnumerable<TaskModel> items)
        {
            foreach (var item in items)
                yield return item;
            await Task.CompletedTask;
        }
    }
}