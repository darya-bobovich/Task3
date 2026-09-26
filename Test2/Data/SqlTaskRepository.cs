using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test2.Model;

namespace Test2.Data
{
    public sealed class SqlTaskRepository : IRepository<TaskModel>
    {
        private readonly AppDbContext _db;
        private bool _disposed;

        public SqlTaskRepository()
        {
            _db = new AppDbContext();
        }
        public Task<List<TaskModel>> GetAllAsync()
             => _db.TaskModels.ToListAsync();

        public ValueTask<TaskModel?> GetByIdAsync(int id)
            => _db.TaskModels.FindAsync(id);

        public Task AddAsync(TaskModel entity)
            => _db.TaskModels.AddAsync(entity).AsTask();

        public Task AddRangeAsync(IEnumerable<TaskModel> entities)
            => _db.TaskModels.AddRangeAsync(entities);

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

        public void Dispose()
        {   
            if(_disposed) return;
            _db.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}