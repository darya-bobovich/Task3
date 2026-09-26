using System.IO;
using System.Runtime.CompilerServices;
using Test2.Model;

namespace Test2.Helpers
{
    public sealed class Import : IImporter
    {
        // поток записей отдаём по 1
        public async IAsyncEnumerable<TaskModel> ParseAsync(
            string filePath,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            using var reader = new StreamReader(filePath);

            string? line;
            // асинхронное чтение 1 строки
            while ((line = await reader.ReadLineAsync(ct)) != null)
            {
                ct.ThrowIfCancellationRequested();

                var parts = line.Split(';');
                if (parts.Length < 6) continue;

                if (!DateTime.TryParse(parts[0], out var date)) continue;

                // отдаём 1 запись наружу и замираем до следующего запроса
                yield return new TaskModel
                {
                    Date = date,
                    Name = parts[1],
                    LastName = parts[2],
                    MiddleName = parts[3],
                    City = parts[4],
                    Country = parts[5]
                };
            }
        }
    }
}