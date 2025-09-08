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
}
