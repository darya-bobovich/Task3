using System.Runtime.CompilerServices;
using System.Windows;
using Test2.Helpers;
using Test2.Model;
using Test2.Resources;

namespace Test2.ViewModels
{
    public sealed partial class MainViewModel
    {
        //Открытие диалога 
        private string? GetImportFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                Title = Resource1.ImportTitle
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        //Импорт и сохранение в БД порциями
        private async Task<int> ImportAndSaveAsync(string filePath)
        {
            const int batchSize = 5000;
            var buffer = new List<TaskModel>(batchSize);
            int total = 0;
            //Файл читается построчно в памяти максимум 1 запись + буфер
            await foreach (var task in _importer.ParseAsync(filePath))
            {
                buffer.Add(task);

                if (buffer.Count >= batchSize)
                {
                    await _repository.AddRangeAsync(buffer);
                    await _repository.SaveAsync();
                    total += buffer.Count;
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                await _repository.AddRangeAsync(buffer);
                await _repository.SaveAsync();
                total += buffer.Count;
            }

            return total;
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
        private string? GetExportFilePath(string format, string extension)
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
        private async Task ExportAsync(IExporter exporter)
        {
            if (!HasDataToExport()) return;

            var filePath = GetExportFilePath(exporter.Format, exporter.Extension);
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                StatusText = string.Format(Resource1.Loading, exporter.Format);
                await exporter.ExportAsync(StreamDisplayedTasksAsync(), filePath);
                ShowExportSuccess(exporter.Format, DisplayedTasks.Count, filePath);
            }
            catch (Exception ex)
            {
                ShowExportError(exporter.Format, ex);
            }
        }

        // Отдаёт DisplayedTasks по одной записи без загрузки всей коллекции в память
        private async IAsyncEnumerable<TaskModel> StreamDisplayedTasksAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken ct = default)
        {
            foreach (var task in DisplayedTasks)
            {
                // если отменили то выходим
                ct.ThrowIfCancellationRequested();
                // отдаем 1 запись
                yield return task;
            }
            await Task.CompletedTask;
        }
    }
}