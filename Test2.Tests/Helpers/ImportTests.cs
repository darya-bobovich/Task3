using Test2.Helpers;

namespace Test2.Tests.Helpers
{
    public class ImportTests
    {
        private readonly Import _import;

        public ImportTests()
        {
            _import = new Import();
        }

        // Тест 1 Проверка правильного чтения файла
        [Fact]
        public async Task ParseAsync_ValidCsvFile_ReturnsCorrectNumberOfTasks()
        {
            // Arrange - подготовка
            var tempFile = Path.GetTempFileName() + ".csv";
            var lines = new[]
            {
                "2026-01-15 10:30:00;Иван;Петров;Иванович;Витебск;Беларусь",
                "2026-01-16 11:00:00;Петр;Сидоров;Алексеевич;Гомель;Беларусь"
            };
            await File.WriteAllLinesAsync(tempFile, lines);

            // Act - действие
            var result = await _import.ParseAsync(tempFile);

            // Assert - проверка
            Assert.Equal(2, result.Count);

            // Cleanup - очистка
            File.Delete(tempFile);
        }

        // Тест 2 Проверка на правильность данных
        [Fact]
        public async Task ParseAsync_ValidCsvFile_ReturnsCorrectData()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".csv";
            var lines = new[]
            {
                "2026-01-15 10:30:00;Иван;Иванов;Иванович;Минск;Беларусь"
            };
            await File.WriteAllLinesAsync(tempFile, lines);

            // Act
            var result = await _import.ParseAsync(tempFile);

            // Assert
            var task = result.First();
            Assert.Equal(new DateTime(2026, 1, 15, 10, 30, 0), task.Date);
            Assert.Equal("Иван", task.Name);
            Assert.Equal("Иванов", task.LastName);
            Assert.Equal("Иванович", task.MiddleName);
            Assert.Equal("Минск", task.City);
            Assert.Equal("Беларусь", task.Country);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 3 Проверка на обработку пустого файла
        [Fact]
        public async Task ParseAsync_EmptyFile_ReturnsEmptyList()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".csv";
            await File.WriteAllLinesAsync(tempFile, Array.Empty<string>());

            // Act
            var result = await _import.ParseAsync(tempFile);

            // Assert
            Assert.Empty(result);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 4 Проверка что строки с неверным форматом игнорируются 
        [Fact]
        public async Task ParseAsync_FileWithInvalidLines_IgnoresInvalidLines()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".csv";
            var lines = new[]
            {
                "2026-01-15 10:30:00;Иван;Петров;Сергеевич;Витебск;Беларусь", 
                "2026-01-16;Петр;Сидоров", 
                "2026-01-17 12:00:00;Анна;Иванова;Сергеевна;Гомель;Беларусь" 
            };
            await File.WriteAllLinesAsync(tempFile, lines);

            // Act
            var result = await _import.ParseAsync(tempFile);

            // Assert
            Assert.Equal(2, result.Count);

            // Cleanup
            File.Delete(tempFile);
        }

        // Тест 5 Проверка обработки неверного формата даты 
        [Fact]
        public async Task ParseAsync_InvalidDateFormat_ThrowsFormatException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName() + ".csv";
            var lines = new[]
            {
                "invalid-date;Иван;Иванов;Иванович;Витебск;Беларусь"
            };
            await File.WriteAllLinesAsync(tempFile, lines);

            // Act & Assert
            await Assert.ThrowsAsync<FormatException>(() => _import.ParseAsync(tempFile));

            // Cleanup
            File.Delete(tempFile);
        }
    }
}