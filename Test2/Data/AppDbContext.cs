using Microsoft.EntityFrameworkCore;
using Test2.Model;
using System.Configuration;

namespace Test2.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskModel> TaskModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
  
}
