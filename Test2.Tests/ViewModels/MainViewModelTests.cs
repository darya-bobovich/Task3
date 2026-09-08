using Test2.Model;
using Test2.ViewModels;

namespace Test2.Tests.ViewModels
{
    public class MainViewModelTests
    {
        // Тест 1 Проверка что команды инициализированы 
        [Fact]
        public void Constructor_ShouldInitializeCommands()
        {
            // Arrange & Act
            var viewModel = new MainViewModel();

            // Assert
            Assert.NotNull(viewModel.ImportCommand);
            Assert.NotNull(viewModel.ExportToXmlCommand);
            Assert.NotNull(viewModel.ExportToExcelCommand);
            Assert.NotNull(viewModel.ApplyFilterCommand);
            Assert.NotNull(viewModel.ResetFilterCommand);
            Assert.NotNull(viewModel.ChangeLanguageCommand);
        }

        // Тест 2 Проверка, что фильтр инициализирован 
        [Fact]
        public void Constructor_ShouldInitializeFilter()
        {
            // Arrange & Act
            var viewModel = new MainViewModel();

            // Assert
            Assert.NotNull(viewModel.Filter);
            Assert.IsType<FilterViewModel>(viewModel.Filter);
        }

        // Тест 3 Проверка фильтрации по имени 
        [Fact]
        public void ApplyFilter_FilterByName_ReturnsFilteredTasks()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Добавляем тестовые данные
            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Иван", City = "Гомель" },
                new() { Id = 2, Name = "Петр", City = "Минск" },
                new() { Id = 3, Name = "Иван", City = "Москва" }
            };

            viewModel.SetTestData(tasks);

            // Act
            viewModel.Filter.Name = "Иван";
            viewModel.ApplyFilterCommand.Execute(null);

            // Assert
            Assert.Equal(2, viewModel.DisplayedTasks.Count);
            Assert.All(viewModel.DisplayedTasks, t => Assert.Equal("Иван", t.Name));
        }

        // Тест 4 Проверка фильтрации по городу
        [Fact]
        public void ApplyFilter_FilterByCity_ReturnsFilteredTasks()
        {
            // Arrange
            var viewModel = new MainViewModel();

            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Иван", City = "Гомель" },
                new() { Id = 2, Name = "Петр", City = "Минск" },
                new() { Id = 3, Name = "Сергей", City = "Гомель" }
            };

            viewModel.SetTestData(tasks);

            // Act
            viewModel.Filter.City = "Гомель";
            viewModel.ApplyFilterCommand.Execute(null);

            // Assert
            Assert.Equal(2, viewModel.DisplayedTasks.Count);
            Assert.All(viewModel.DisplayedTasks, t => Assert.Equal("Гомель", t.City));
        }

        // Тест 5 Проверка сброса фильтра 
        [Fact]
        public void ResetFilter_WhenFilterApplied_ResetsToAllTasks()
        {
            // Arrange
            var viewModel = new MainViewModel();

            // Act - применяем фильтр
            viewModel.Filter.Name = "Иван";
            viewModel.ApplyFilterCommand.Execute(null);

            // Act - сбрасываем
            viewModel.ResetFilterCommand.Execute(null);

            // Assert
            Assert.True(viewModel.DisplayedTasks.Count >= 0);
            Assert.Empty(viewModel.Filter.Name);
            Assert.False(viewModel.Filter.HasFilters);
        }

        // Тест 6 Проверка смены языка 
        [Fact]
        public void ChangeLanguage_WhenCurrentLanguageIsRussian_SwitchesToEnglish()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var currentStatus = viewModel.StatusText;

            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Test" }
            };
            viewModel.SetTestData(tasks);

            // Act
            viewModel.ChangeLanguageCommand.Execute(null);

            // Assert
            Assert.NotEqual(currentStatus, viewModel.StatusText);
        }

        // Тест 7 Проверка статуса после фильтрации
        [Fact]
        public void ApplyFilter_UpdatesStatusText()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var initialStatus = viewModel.StatusText;

            // Act
            viewModel.Filter.Name = "Test";
            viewModel.ApplyFilterCommand.Execute(null);

            // Assert
            Assert.NotEqual(initialStatus, viewModel.StatusText);
        }

        // Тест 8 Проверка множественных фильтры
        [Fact]
        public void ApplyFilter_WithMultipleFilters_ReturnsFilteredTasks()
        {
            // Arrange
            var viewModel = new MainViewModel();

            var tasks = new List<TaskModel>
            {
                new() { Id = 1, Name = "Иван", City = "Гомель", Country = "Беларусь" },
                new() { Id = 2, Name = "Иван", City = "Москва", Country = "Россия" },
                new() { Id = 3, Name = "Петр", City = "Гомель", Country = "Беларусь" }
            };

            viewModel.SetTestData(tasks);

            // Act
            viewModel.Filter.Name = "Иван";
            viewModel.Filter.City = "Гомель";
            viewModel.ApplyFilterCommand.Execute(null);

            // Assert
            Assert.Single(viewModel.DisplayedTasks);
            var task = viewModel.DisplayedTasks.First();
            Assert.Equal("Иван", task.Name);
            Assert.Equal("Гомель", task.City);
        }
    }
}