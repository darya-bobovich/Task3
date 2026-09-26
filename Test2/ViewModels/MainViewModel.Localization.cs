using System.Windows;
using Test2.Resources;

namespace Test2.ViewModels
{
    public sealed partial class MainViewModel
    {
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

        private void ChangeLanguage()
        {
            //Переключаем между русским и английским
            var current = _localization.CurrentCulture;
            var newCulture = current.StartsWith("ru") ? "en-US" : "ru-RU";
            _localization.SetCulture(newCulture);
        }
    }
}