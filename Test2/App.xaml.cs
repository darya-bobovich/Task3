using System.Configuration;
using System.Data;
using System.Windows;
using Test2.Data;
using Test2.ViewModels;

namespace Test2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализация базы данных при старте приложения
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            mainWindow.Show();
        }
    }

}
