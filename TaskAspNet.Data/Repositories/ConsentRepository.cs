// Data/Repositories/ConsentRepository.cs
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class ConsentRepository : IConsentRepository
{
    private readonly IBaseRepository<Consent> _baseRepository;
    private readonly AppDbContext _context;

    public ConsentRepository(IBaseRepository<Consent> baseRepository, AppDbContext context)
    {
        _baseRepository = baseRepository;
        _context = context;
    }

    public async Task<Consent?> GetByUserIdAsync(string userId)
    {
        return await _context
            .Set<Consent>()
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Consent> AddAsync(Consent entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Console.WriteLine($"ConsentRepository: Adding new consent for user {entity.UserId}");
            await _context.Set<Consent>().AddAsync(entity);
            await _context.SaveChangesAsync();
            Console.WriteLine("ConsentRepository: New consent added successfully"); 
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ConsentRepository Add Error: {ex.Message}");
            Console.WriteLine($"ConsentRepository Add Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    public Task<IEnumerable<Consent>> FindAsync(Expression<Func<Consent, bool>> predicate)
        => _baseRepository.FindAsync(predicate);

    public Task<IEnumerable<Consent>> GetAllAsync()
        => _baseRepository.GetAllAsync();

    public Task<Consent> GetByIdAsync(int id)
        => _baseRepository.GetByIdAsync(id);

    public async Task<Consent> UpdateAsync(Consent entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Console.WriteLine($"ConsentRepository: Updating consent for user {entity.UserId}");
            _context.Set<Consent>().Update(entity);
            await _context.SaveChangesAsync();
            Console.WriteLine("ConsentRepository: Consent updated successfully");
            return entity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ConsentRepository Update Error: {ex.Message}");
            Console.WriteLine($"ConsentRepository Update Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    public Task<Consent> DeleteAsync(Consent entity)
        => _baseRepository.DeleteAsync(entity);

    public Task SaveAsync()
        => _baseRepository.SaveAsync();
}
