using System.Windows;
using Test2.ViewModels;
using MahApps.Metro.Controls;

namespace Test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Привязываем ViewModel к окну
            DataContext = new MainViewModel();
        }
    }

}