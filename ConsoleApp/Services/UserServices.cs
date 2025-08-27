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

    //Retrieves all users from the database
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.User.ToListAsync();
    }

    //Retrieves a single user by their ID - returns null if not found
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.User.FindAsync(id);
    }
}