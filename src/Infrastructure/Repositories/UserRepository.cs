using Core.Models;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetUsersByRegionAsync(int regionId)
    {
        try
        {
            return await _context.Users
                .Where(u => u.RegionId == regionId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for region {RegionId}", regionId);
            return Enumerable.Empty<User>();
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersByRegionAsync(int regionId)
    {
        try
        {
            return await _context.Users
                .Where(u => u.RegionId == regionId && u.IsActive)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users for region {RegionId}", regionId);
            return Enumerable.Empty<User>();
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Region)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Region)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Region)
            .Where(u => u.IsActive)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRegionAsync(int regionId)
    {
        return await _context.Users
            .Include(u => u.Region)
            .Where(u => u.RegionId == regionId && u.IsActive)
            .OrderBy(u => u.RegionId)
            .ThenBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
