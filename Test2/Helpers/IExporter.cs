using Test2.Model;

namespace Test2.Helpers
{
    public interface IExporter
    {
        string Format { get; }       
        string Extension { get; }    

        Task ExportAsync(
            IAsyncEnumerable<TaskModel> tasks,   
            string filePath,
            CancellationToken ct = default);
    }
}