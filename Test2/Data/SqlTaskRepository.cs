using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test2.Model;

namespace Test2.Data
{
    public class SqlTaskRepository : IRepository<TaskModel>
    {
        private readonly AppDbContext _db;
        private bool _disposed = false;

        public SqlTaskRepository()
        {
            _db = new AppDbContext();
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            return await _db.TaskModels.ToListAsync();
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            return await _db.TaskModels.FindAsync(id);
        }

        public async Task AddAsync(TaskModel entity)
        {
            await _db.TaskModels.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TaskModel> entities)
        {
            await _db.TaskModels.AddRangeAsync(entities);
        }

        public Task UpdateAsync(TaskModel entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.TaskModels.FindAsync(id);
            if (entity != null)
                _db.TaskModels.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _db.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}