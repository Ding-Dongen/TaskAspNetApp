
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Interfaces;


namespace TaskAspNet.Data.Repositories;

public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
{
    private readonly AppDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var result = await _dbSet.ToListAsync();
        return result;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity ?? throw new Exception($"Entity with ID {id} not found.");
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<T> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return entity;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}