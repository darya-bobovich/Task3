using ClosedXML.Excel;
using Test2.Helpers;
using Test2.Model;

namespace Test2.Tests.Helpers
{
    public class ExcelExporterTests
    {
        private readonly ExcelExporter _exporter;

        public ExcelExporterTests()
        {
            _exporter = new ExcelExporter();
        }

        // Тест 1 Проверка, что Excel файл создается 
        [Fact]
        public async Task ExportAsync_ValidTasks_CreatesExcelFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xlsx";
            var tasks = new List<TaskModel>
            {
                new()
                {
                    Id = 1,
                    Name = "Иван",
                    LastName = "Петров",
                    Date = new DateTime(2026, 1, 15, 10, 30, 0)
                }
            };

            // Act
            await _exporter.ExportAsync(tasks, tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));
            Assert.True(new FileInfo(tempFile).Length > 0);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 2 Проверка, что данные записаны правильно
        [Fact]
        public async Task ExportAsync_ValidTasks_ContainsCorrectData()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xlsx";
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
            await _exporter.ExportAsync(tasks, tempFile);

            // Assert
            using (var workbook = new XLWorkbook(tempFile))
            {
                var worksheet = workbook.Worksheet(1);

                // Проверяем заголовки
                Assert.Equal("ID", worksheet.Cell(1, 1).Value);
                Assert.Equal("Имя", worksheet.Cell(1, 3).Value);

                // Проверяем данные
                Assert.Equal(1, worksheet.Cell(2, 1).Value);
                Assert.Equal("Иван", worksheet.Cell(2, 3).Value);
                Assert.Equal("Иванов", worksheet.Cell(2, 4).Value);
                Assert.Equal("Гомель", worksheet.Cell(2, 6).Value);
            }

            // Cleanup
            File.Delete(tempFile);
        }

        // Проверка на обработку пустого списка 
        [Fact]
        public async Task ExportAsync_EmptyTasks_ThrowsInvalidOperationException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xlsx";
            var tasks = new List<TaskModel>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _exporter.ExportAsync(tasks, tempFile));

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 4 Проверка на обработку null значений 
        [Fact]
        public async Task ExportAsync_TasksWithNullFields_HandlesNullValues()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".xlsx";
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
            await _exporter.ExportAsync(tasks, tempFile);

            // Assert
            using (var workbook = new XLWorkbook(tempFile))
            {
                var worksheet = workbook.Worksheet(1);
                // Name и LastName должны быть пустыми
                Assert.Equal("", worksheet.Cell(2, 3).Value); 
                Assert.Equal("", worksheet.Cell(2, 4).Value); 
            }

            // Cleanup
            File.Delete(tempFile);
        }
    }
}