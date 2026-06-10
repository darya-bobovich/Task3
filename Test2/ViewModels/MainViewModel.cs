using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Test2.Data;
using Test2.Helpers;
using Test2.Model;

namespace Test2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<TaskModel> _repository;
        private ObservableCollection<TaskModel> _allTasks;
        private ObservableCollection<TaskModel> _displayedTasks; 
        private FilterViewModel _filter;
        private string _statusText = "Готов к работе";

        public ObservableCollection<TaskModel> DisplayedTasks
        {
            get => _displayedTasks;
            private set
            {
                _displayedTasks = value;
                OnPropertyChanged();
            }
        }

        public FilterViewModel Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public ICommand ImportCommand { get; }
        public ICommand ExportToXmlCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }

        public MainViewModel()
        {
            _repository = new SqlTaskRepository();
            _allTasks = new ObservableCollection<TaskModel>();
            DisplayedTasks = new ObservableCollection<TaskModel>();  
            Filter = new FilterViewModel();

            ImportCommand = new AsyncDelegateCommand(async _ => await ImportAsync());
            ExportToXmlCommand = new AsyncDelegateCommand(async _ => await ExportToXmlAsync());
            ExportToExcelCommand = new AsyncDelegateCommand(async _ => await ExportToExcelAsync());
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            ResetFilterCommand = new DelegateCommand(ResetFilter);

            LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            StatusText = "Загрузка данных...";
            var tasks = await _repository.GetAllAsync();

            _allTasks.Clear();
            foreach (var task in tasks)
            {
                _allTasks.Add(task);
            }

            DisplayedTasks.Clear();
            foreach (var task in _allTasks)
            {
                DisplayedTasks.Add(task);
            }

            StatusText = $"Всего записей: {_allTasks.Count}";
        }

        private void ApplyFilter()
        {
            StatusText = "Применение фильтра...";

            var filtered = _allTasks.AsEnumerable();

            if (Filter.Date.HasValue)
            {
                filtered = filtered.Where(t => t.Date.Date == Filter.Date.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(Filter.Name))
            {
                filtered = filtered.Where(t => t.Name != null &&
                    t.Name.Contains(Filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(Filter.LastName))
            {
                filtered = filtered.Where(t => t.LastName != null &&
                    t.LastName.Contains(Filter.LastName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(Filter.MiddleName))
            {
                filtered = filtered.Where(t => t.MiddleName != null &&
                    t.MiddleName.Contains(Filter.MiddleName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(Filter.City))
            {
                filtered = filtered.Where(t => t.City != null &&
                    t.City.Contains(Filter.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(Filter.Country))
            {
                filtered = filtered.Where(t => t.Country != null &&
                    t.Country.Contains(Filter.Country, StringComparison.OrdinalIgnoreCase));
            }

            DisplayedTasks.Clear();
            foreach (var item in filtered)
            {
                DisplayedTasks.Add(item);
            }

            UpdateStatusText();
        }

        private void ResetFilter()
        {
            Filter.Reset();

            DisplayedTasks.Clear();
            foreach (var task in _allTasks)
            {
                DisplayedTasks.Add(task);
            }

            StatusText = $"Всего записей: {_allTasks.Count}";
        }

        private void UpdateStatusText()
        {
            if (Filter.HasFilters)
            {
                StatusText = $"Найдено {DisplayedTasks.Count} из {_allTasks.Count} записей (применены фильтры)";
            }
            else
            {
                StatusText = $"Всего записей: {DisplayedTasks.Count}";
            }
        }

        private async Task ImportAsync()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                Title = "Выберите CSV файл для импорта"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                StatusText = "Импорт данных...";
                var importer = new Import();
                var newTasks = await importer.ParseAsync(dialog.FileName);

                if (newTasks.Any())
                {
                    await _repository.AddRangeAsync(newTasks);
                    await _repository.SaveAsync();
                    await LoadTasksAsync();  

                    MessageBox.Show($"Импортировано {newTasks.Count} записей", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Ошибка при импорте";
            }
        }

        private async Task ExportToXmlAsync()
        {
            if (!DisplayedTasks.Any())
            {
                MessageBox.Show("Нет данных для экспорта",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*",
                Title = "Сохранить XML файл",
                DefaultExt = "xml",
                FileName = $"TasksExport_{DateTime.Now:yyyyMMdd_HHmmss}.xml"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                StatusText = "Экспорт в XML...";
                var xmlExporter = new XmlExporter();
                await xmlExporter.ExportAsync(DisplayedTasks, dialog.FileName);

                MessageBox.Show($"Экспортировано {DisplayedTasks.Count} записей в файл:\n{dialog.FileName}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusText = $"Экспортировано {DisplayedTasks.Count} записей в XML";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в XML: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Ошибка при экспорте в XML";
            }
        }

        private async Task ExportToExcelAsync()
        {
            if (!DisplayedTasks.Any())
            {
                MessageBox.Show("Нет данных для экспорта",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                Title = "Сохранить Excel файл",
                DefaultExt = "xlsx",
                FileName = $"TasksExport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                StatusText = "Экспорт в Excel...";
                var excelExporter = new ExcelExporter();
                await excelExporter.ExportAsync(DisplayedTasks, dialog.FileName);

                MessageBox.Show($"Экспортировано {DisplayedTasks.Count} записей в файл:\n{dialog.FileName}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                StatusText = $"Экспортировано {DisplayedTasks.Count} записей в Excel";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Ошибка при экспорте в Excel";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}