using System.Collections.ObjectModel;
using Test2.Data;
using Test2.Helpers;
using Test2.Model;
using Test2.Resources;
using Test2.Services;

namespace Test2.ViewModels
{
    public sealed partial class MainViewModel : BaseViewModel
    {
        private readonly IRepository<TaskModel> _repository;
        private readonly ILocalizationService _localization;
        private readonly IExporter _xmlExporter;
        private readonly IExporter _excelExporter;
        private readonly IImporter _importer;

        private ObservableCollection<TaskModel> _allTasks = new();
        private ObservableCollection<TaskModel> _displayedTasks = new();
        private string _statusText = "";

        public ObservableCollection<TaskModel> DisplayedTasks
        {
            get => _displayedTasks;
            private set => SetProperty(ref _displayedTasks, value);
        }

        public FilterViewModel Filter { get; }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public MainViewModel(
            IRepository<TaskModel> repository,
            ILocalizationService localization,
            IExporter xmlExporter,
            IExporter excelExporter,
            IImporter importer)
        {
            _repository = repository;
            _localization = localization;
            _xmlExporter = xmlExporter;
            _excelExporter = excelExporter;
            _importer = importer;

            // Подписываемся на смену языка
            _localization.CultureChanged += OnCultureChanged;

            Filter = new FilterViewModel();
            InitializeCommands();

            // Инициализация статуса с локализацией
            StatusText = Resource1.Ready;
            _ = LoadTasksAsync();
        }

        public MainViewModel()
            : this(
                new SqlTaskRepository(),
                new LocalizationService(),
                new XmlExporter(),
                new ExcelExporter(),
                new Import())
        {
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

        public void SetTestData(IEnumerable<TaskModel> tasks)
        {
            _allTasks = new ObservableCollection<TaskModel>(tasks);
            _displayedTasks = new ObservableCollection<TaskModel>(tasks);
        }
    }
}