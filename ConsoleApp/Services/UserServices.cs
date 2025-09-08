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

    //Creates a new user in the database
    public async Task<User> CreateUserAsync(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    //Updates an existing user in the database
    public async Task<bool> UpdateUserAsync(User user)
    {
        _context.User.Update(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    //Deletes a user from the database by ID
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
            return false;

        _context.User.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}