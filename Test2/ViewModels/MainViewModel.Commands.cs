using System.Windows.Input;
using Test2.Helpers;

namespace Test2.ViewModels
{
    public sealed partial class MainViewModel
    {
        public ICommand ImportCommand { get; private set; } = null!;
        public ICommand ExportToXmlCommand { get; private set; } = null!;
        public ICommand ExportToExcelCommand { get; private set; } = null!;
        public ICommand ApplyFilterCommand { get; private set; } = null!;
        public ICommand ResetFilterCommand { get; private set; } = null!;
        public ICommand ChangeLanguageCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            ImportCommand = new AsyncDelegateCommand(_ => ImportAsync());
            ExportToXmlCommand = new AsyncDelegateCommand(_ => ExportAsync(_xmlExporter));
            ExportToExcelCommand = new AsyncDelegateCommand(_ => ExportAsync(_excelExporter));
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            ResetFilterCommand = new DelegateCommand(ResetFilter);
            ChangeLanguageCommand = new DelegateCommand(ChangeLanguage);
        }
    }
}