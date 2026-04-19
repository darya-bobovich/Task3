using Microsoft.EntityFrameworkCore;
using Test2.Model;

namespace Test2.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskModel> TaskModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=TestDb;Trusted_Connection=True;");
        }
    }
  
}
