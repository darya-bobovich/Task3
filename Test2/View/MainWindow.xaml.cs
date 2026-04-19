using System.Windows;
using Test2.ViewModels;

namespace Test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Привязываем ViewModel к окну
            DataContext = new MainViewModel();
        }
    }

}