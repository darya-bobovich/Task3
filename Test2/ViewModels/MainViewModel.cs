using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Test2.Data;
using Test2.Helpers;
using Test2.Model;

namespace Test2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }

        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }

        public MainViewModel()
        {
            LoadTasks();
            ImportCommand = new DelegateCommand(Import);
            ExportCommand = new DelegateCommand(Export);
        }

        public void LoadTasks()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
                var tasksFromDb = db.TaskModels.ToList();
                Tasks = new ObservableCollection<TaskModel>(tasksFromDb);
            }
        }

        private void Import()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Выберите CSV файл для импорта";

            if (openFileDialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);
                var newTasks = new List<TaskModel>();

                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length >= 6)
                    {
                        newTasks.Add(new TaskModel
                        {
                            Date = DateTime.Parse(parts[0]),
                            Name = parts[1],
                            LastName = parts[2],
                            MiddleName = parts[3],
                            City = parts[4],
                            Country = parts[5]
                        });
                    }
                }

                using (var db = new AppDbContext())
                {
                    db.TaskModels.AddRange(newTasks);
                    db.SaveChanges();
                }

                LoadTasks();
                System.Windows.MessageBox.Show($"Импортировано {newTasks.Count} записей");
            }
        }

        private void Export()
        {
            if (Tasks == null || Tasks.Count == 0)
            {
                System.Windows.MessageBox.Show("Нет данных для экспорта", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*";
            saveFileDialog.Title = "Сохранить XML файл";
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.FileName = $"TasksExport_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    XElement root = new XElement("Tasks");

                    foreach (var task in Tasks)
                    {
                        XElement taskElement = new XElement("Task",
                            new XElement("Id", task.Id),
                            new XElement("Date", task.Date.ToString("yyyy-MM-dd HH:mm:ss")),
                            new XElement("Name", task.Name ?? ""),
                            new XElement("LastName", task.LastName ?? ""),
                            new XElement("MiddleName", task.MiddleName ?? ""),
                            new XElement("City", task.City ?? ""),
                            new XElement("Country", task.Country ?? "")
                        );
                        root.Add(taskElement);
                    }

                    XDocument xmlDoc = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        root
                    );

                    xmlDoc.Save(saveFileDialog.FileName);

                    System.Windows.MessageBox.Show($"Экспортировано {Tasks.Count} записей в файл:\n{saveFileDialog.FileName}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}