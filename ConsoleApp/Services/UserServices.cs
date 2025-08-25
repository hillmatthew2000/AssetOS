using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.ConsoleApp.Services;

//Service class for managing User entities
public class UserService
{
    private readonly ITAMDbContext _context;

    //Constructor injecting the database context
    public UserService(ITAMDbContext context)
    {
        _context = context;
    }

    //Retrieves all users including related
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.User.ToListAsync();
    }

    //Retrieves a single User by its Id
    public async Task<Asset?> GetUserByIdAsync(int id)
    {
        return await _context.Asset.FindAsync(id);
    }
}