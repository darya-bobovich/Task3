using System.IO;
using Test2.Model;

namespace Test2.Helpers
{
    public class Import
    {
        public async Task<List<TaskModel>> ParseAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var tasks = new List<TaskModel>();

            foreach (var line in lines)
            {
                var parts = line.Split(';');

                if (parts.Length >= 6)
                {
                    tasks.Add(new TaskModel
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

            return tasks;
        }
    }
}