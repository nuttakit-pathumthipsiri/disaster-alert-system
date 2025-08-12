using Core.Models;

namespace Core.Interfaces;


public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetByRegionAsync(int regionId);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}
