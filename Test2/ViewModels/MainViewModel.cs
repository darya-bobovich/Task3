using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Test2.Data;
using Test2.Helpers;
using Test2.Model;
using Test2.Resources;
using Test2.Services;

namespace Test2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<TaskModel> _repository;
        private readonly ILocalizationService _localizationService;
        private ObservableCollection<TaskModel> _allTasks;
        private ObservableCollection<TaskModel> _displayedTasks; 
        private FilterViewModel _filter;
        private string _statusText;

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
        public ICommand ChangeLanguageCommand { get; }

        public MainViewModel()
        {
            _repository = new SqlTaskRepository();
            _localizationService = new LocalizationService();
            // Подписываемся на смену языка
            _localizationService.CultureChanged += OnCultureChanged;

            _filter = new FilterViewModel();
            _allTasks = new ObservableCollection<TaskModel>();
            _displayedTasks = new ObservableCollection<TaskModel>();  

            ImportCommand = new AsyncDelegateCommand(async _ => await ImportAsync());
            ExportToXmlCommand = new AsyncDelegateCommand(async _ => await ExportToXmlAsync());
            ExportToExcelCommand = new AsyncDelegateCommand(async _ => await ExportToExcelAsync());
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            ResetFilterCommand = new DelegateCommand(ResetFilter);
            ChangeLanguageCommand = new DelegateCommand(ChangeLanguage);

            // Инициализация статуса с локализацией
            StatusText = Resource1.Ready;
            LoadTasksAsync();
        }

        private void OnCultureChanged()
        {
            //Обновляем все строки при смене языка
            StatusText = Resource1.Ready;

            if (_allTasks != null && _allTasks.Any())
            {
                ApplyFilter();
            }

            //Обновляем заголовок окна
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Title = Resource1.MainWindowTitle;
            }

            OnPropertyChanged(nameof(DateLabel));
            OnPropertyChanged(nameof(NameLabel));
            OnPropertyChanged(nameof(LastNameLabel));
            OnPropertyChanged(nameof(MiddleNameLabel));
            OnPropertyChanged(nameof(CityLabel));
            OnPropertyChanged(nameof(CountryLabel));
            OnPropertyChanged(nameof(FilterButton));
            OnPropertyChanged(nameof(ResetButton));
            OnPropertyChanged(nameof(ImportButton));
            OnPropertyChanged(nameof(ExportXmlButton));
            OnPropertyChanged(nameof(ExportExcelButton));
            OnPropertyChanged(nameof(StatusText));
        }
        public string DateLabel => Resource1.DateLabel;
        public string NameLabel => Resource1.NameLabel;
        public string LastNameLabel => Resource1.LastNameLabel;
        public string MiddleNameLabel => Resource1.MiddleNameLabel;
        public string CityLabel => Resource1.CityLabel;
        public string CountryLabel => Resource1.CountryLabel;
        public string FilterButton => Resource1.FilterButton;
        public string ResetButton => Resource1.ResetButton;
        public string ImportButton => Resource1.ImportButton;
        public string ExportXmlButton => Resource1.ExportXmlButton;
        public string ExportExcelButton => Resource1.ExportExcelButton;

        private void ChangeLanguage()
        {
            //Переключаем между русским и английским
            var current = _localizationService.CurrentCulture;
            var newCulture = current.StartsWith("ru") ? "en-US" : "ru-RU";
            _localizationService.SetCulture(newCulture);
        }


        //Принцип SRP
        private void UpdateCollections(IEnumerable<TaskModel> tasks)
        {
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
        }

        private async Task LoadTasksAsync()
        {
            StatusText = Resource1.Loading;
            var tasks = await _repository.GetAllAsync();
            //Принцип SRP
            UpdateCollections(tasks);
            StatusText = string.Format(Resource1.RecordsTotal, _allTasks.Count);
        }

        //Принцип SRP
        private IEnumerable<TaskModel> GetFilteredTasks()
        {
            var filtered = _allTasks.AsEnumerable();

            if (Filter.Date.HasValue)
                filtered = filtered.Where(t => t.Date.Date == Filter.Date.Value.Date);

            if (!string.IsNullOrWhiteSpace(Filter.Name))
                filtered = filtered.Where(t => t.Name != null &&
                    t.Name.Contains(Filter.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.LastName))
                filtered = filtered.Where(t => t.LastName != null &&
                    t.LastName.Contains(Filter.LastName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.MiddleName))
                filtered = filtered.Where(t => t.MiddleName != null &&
                    t.MiddleName.Contains(Filter.MiddleName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.City))
                filtered = filtered.Where(t => t.City != null &&
                    t.City.Contains(Filter.City, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.Country))
                filtered = filtered.Where(t => t.Country != null &&
                    t.Country.Contains(Filter.Country, StringComparison.OrdinalIgnoreCase));

            return filtered;
        }
        //Принцип SRP
        private void UpdateDisplayedTasks(IEnumerable<TaskModel> tasks)
        {
            DisplayedTasks.Clear();
            foreach (var item in tasks)
            {
                DisplayedTasks.Add(item);
            }
        }
        //Принцип SRP
        private void UpdateStatusAfterFilter(int displayedCount, int totalCount)
        {
            if (Filter.HasFilters)
            {
                StatusText = string.Format(Resource1.FilterApplied, displayedCount, totalCount);
            }
            else
            {
                StatusText = string.Format(Resource1.RecordsTotal, displayedCount);
            }
        }
        //Принцип SRP
        private void ApplyFilter()
        {
            StatusText = Resource1.Filtering;
            var filtered = GetFilteredTasks();
            UpdateDisplayedTasks(filtered);
            UpdateStatusAfterFilter(DisplayedTasks.Count, _allTasks.Count);
        }

        private void ResetFilter()
        {
            Filter.Reset();                         
            UpdateDisplayedTasks(_allTasks);         
            UpdateStatusAfterFilter(_allTasks.Count, _allTasks.Count); 
        }

        //Открытие диалога 
        private string GetImportFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                Title = Resource1.ImportTitle
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        //Импорт и сохранение в БД 
        private async Task<int> ImportAndSaveAsync(string filePath)
        {
            var importer = new Import();
            var newTasks = await importer.ParseAsync(filePath);

            if (newTasks.Any())
            {
                await _repository.AddRangeAsync(newTasks);
                await _repository.SaveAsync();
                return newTasks.Count;
            }
            return 0;
        }

        //Показ сообщения об успехе
        private void ShowImportSuccess(int count)
        {
            MessageBox.Show(
                string.Format(Resource1.ImportSuccess, count),
                Resource1.Success,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        //Показ сообщения об ошибке
        private void ShowImportError(Exception ex)
        {
            MessageBox.Show(
                string.Format(Resource1.ImportError, ex.Message),
                Resource1.Error,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            StatusText = Resource1.Error;
        }
        // Принцип SRP
        private async Task ImportAsync()
        {
            var filePath = GetImportFilePath();
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                StatusText = Resource1.Loading;
                var count = await ImportAndSaveAsync(filePath);

                if (count > 0)
                {
                    await LoadTasksAsync();  
                    ShowImportSuccess(count);
                }
            }
            catch (Exception ex)
            {
                ShowImportError(ex);
            }
        }

        // Проверка данных
        private bool HasDataToExport()
        {
            if (!DisplayedTasks.Any())
            {
                MessageBox.Show(
                    Resource1.NoDataToExport,
                    Resource1.Warning,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // Получение пути для сохранения
        private string GetExportFilePath(string format, string extension)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = $"{format} файлы (*.{extension})|*.{extension}|Все файлы (*.*)|*.*",
                Title = string.Format(Resource1.ExportTitle, format),
                DefaultExt = extension,
                FileName = $"TasksExport_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}"
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        // Экспорт в XML
        private async Task ExportToXmlFileAsync(string filePath)
        {
            var xmlExporter = new XmlExporter();
            await xmlExporter.ExportAsync(DisplayedTasks, filePath);
        }

        // Показ сообщения об успехе экспорта
        private void ShowExportSuccess(string format, int count, string filePath)
        {
            MessageBox.Show(
                string.Format(Resource1.ExportSuccess, count, format) + $"\n{filePath}",
                Resource1.Success,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            StatusText = string.Format(Resource1.ExportSuccess, count, format);
        }

        // Показ ошибки экспорта
        private void ShowExportError(string format, Exception ex)
        {
            MessageBox.Show(
                string.Format(Resource1.ExportError, format, ex.Message),
                Resource1.Error,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            StatusText = string.Format(Resource1.ExportError, format, "");
        }

        // Принцип SRP
        private async Task ExportToXmlAsync()
        {
            if (!HasDataToExport()) return;

            var filePath = GetExportFilePath("XML", "xml");
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                StatusText = string.Format(Resource1.Loading, "XML");
                await ExportToXmlFileAsync(filePath);
                ShowExportSuccess("XML", DisplayedTasks.Count, filePath);
            }
            catch (Exception ex)
            {
                ShowExportError("XML", ex);
            }
        }
        // Экспорт в Exel
        private async Task ExportToExcelFileAsync(string filePath)
        {
            var excelExporter = new ExcelExporter();
            await excelExporter.ExportAsync(DisplayedTasks, filePath);
        }

        // Принцип SRP
        private async Task ExportToExcelAsync()
        {
            if (!HasDataToExport()) return;

            var filePath = GetExportFilePath("Excel", "xlsx");
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                StatusText = string.Format(Resource1.Loading, "Excel");
                await ExportToExcelFileAsync(filePath);
                ShowExportSuccess("Excel", DisplayedTasks.Count, filePath);
            }
            catch (Exception ex)
            {
                ShowExportError("Excel", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void SetTestData(IEnumerable<TaskModel> tasks)
        {
            _allTasks = new ObservableCollection<TaskModel>(tasks);
            _displayedTasks = new ObservableCollection<TaskModel>(tasks);
        }
    }
}