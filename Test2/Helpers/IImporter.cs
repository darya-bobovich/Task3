using Test2.Model;

namespace Test2.Helpers
{
    public interface IImporter
    {
        IAsyncEnumerable<TaskModel> ParseAsync(
            string filePath,
            CancellationToken ct = default);
    }
}