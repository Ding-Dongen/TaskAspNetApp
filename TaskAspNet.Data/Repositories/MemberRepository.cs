using Microsoft.EntityFrameworkCore;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Entities;
using TaskAspNet.Data.Interfaces;

namespace TaskAspNet.Data.Repositories;

public class MemberRepository(AppDbContext context) : BaseRepository<MemberEntity>(context), IMemberRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<MemberEntity>> GetMembersWithJobTitleAsync()
    {
        return await _context.Members
            .Include(m => m.JobTitle)
            .Include(m => m.Phones)
            .Include(m => m.Addresses)
            .ToListAsync();
    }

    public async Task<List<MemberEntity>> SearchMembersAsync(string searchTerm)
    {
        return await _context.Members
            .Where(m => m.FirstName.Contains(searchTerm)
                     || m.LastName.Contains(searchTerm)
                     || m.Email.Contains(searchTerm))
            .ToListAsync();
    }
    public async Task<List<JobTitleEntity>> GetAllJobTitlesAsync()
    {
        return await _context.JobTitles.ToListAsync();
    }
    public async Task<JobTitleEntity> GetJobTitleByIdAsync(int id)
    {
        return await _context.JobTitles.FindAsync(id);
    }

    public async Task<MemberEntity> GetMemberByUserIdAsync(string userId)
    {
        return await _context.Members
            .Include(m => m.JobTitle)
            .Include(m => m.Phones)
            .Include(m => m.Addresses)
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<MemberEntity> GetMemberByIdWithDetailsAsync(int id)
    {
        return await _context.Members
            .Include(m => m.JobTitle)
            .Include(m => m.Phones)
            .Include(m => m.Addresses)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public void DetachEntity(object entity)
    {
        _context.Entry(entity).State = EntityState.Detached;
    }

}
