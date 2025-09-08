using ITAM.ConsoleApp.Models;
using ITAM.ConsoleApp.Services;

namespace ITAM.ConsoleApp.Managers;

public class UserManager
{
    //Service dependency for user operations
    private readonly UserService _userService;

    //Constructor to inject required service
    public UserManager(UserService userService)
    {
        _userService = userService;
    }

    //Retrieve and display all users in the system
    public async Task ListUsersAsync()
    {
        Console.WriteLine("\n=== All Users ===");
        var users = await _userService.GetAllUsersAsync();

        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Name: {user.Name}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Department: {user.Department}");
            Console.WriteLine("----------------------------------");
        }
    }

    //Create a new user by collecting user input
    public async Task AddNewUserAsync()
    {
        var user = new User();

        Console.Write("\nUser Name: ");
        user.Name = Console.ReadLine() ?? string.Empty;

        Console.Write("Email: ");
        user.Email = Console.ReadLine() ?? string.Empty;

        Console.Write("Department: ");
        user.Department = Console.ReadLine() ?? string.Empty;

        try
        {
            var newUser = await _userService.CreateUserAsync(user);
            Console.WriteLine($"\nUser created successfully! ID: {newUser.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError creating user: {ex.Message}");
        }
    }

    //Update an existing user
    public async Task UpdateUserAsync()
    {
        Console.WriteLine("\n=== Update User ===");
        Console.Write("Enter User ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out var userId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            Console.WriteLine("User not found");
            return;
        }

        Console.WriteLine($"Current Name: {user.Name}");
        Console.Write("New Name (leave blank to keep current): ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
            user.Name = newName;

        Console.WriteLine($"Current Email: {user.Email}");
        Console.Write("New Email (leave blank to keep current): ");
        var newEmail = Console.ReadLine();
        if (!string.IsNullOrEmpty(newEmail))
            user.Email = newEmail;

        Console.WriteLine($"Current Department: {user.Department}");
        Console.Write("New Department (leave blank to keep current): ");
        var newDepartment = Console.ReadLine();
        if (!string.IsNullOrEmpty(newDepartment))
            user.Department = newDepartment;

        try
        {
            bool success = await _userService.UpdateUserAsync(user);
            Console.WriteLine(success ? "\nUser updated successfully!" : "\nFailed to update user.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError updating user: {ex.Message}");
        }
    }

    //Delete a user from the system
    public async Task DeleteUserAsync()
    {
        Console.WriteLine("\n=== Delete User ===");
        Console.Write("Enter User ID to delete: ");

        if (!int.TryParse(Console.ReadLine(), out var userId))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        try
        {
            var success = await _userService.DeleteUserAsync(userId);
            Console.WriteLine(success ? "\nUser deleted successfully!" : "\nUser not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError deleting user: {ex.Message}");
        }
    }
}
