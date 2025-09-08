using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Managers;
using ITAM.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public class Program
{
    static async Task Main(string[] args)
    {
        //Initialize database context
        var options = new DbContextOptionsBuilder<ITAMDbContext>()
            .UseSqlite($"Data Source=Itam.db").Options;

        var context = new ITAMDbContext(options);
        await context.Database.EnsureCreatedAsync();

        //Initialize services
        var assetService = new AssetService(context);
        var userService = new UserService(context);

        //Initialize managers
        var displayManager = new DisplayManager();
        var assetManager = new AssetManager(assetService, userService);
        var userManager = new UserManager(userService);
        var menuManager = new MenuManager(assetManager, userManager);

        //Display header and run application
        displayManager.DisplayAssetOSHeader();
        await menuManager.ShowMainMenuAsync();
    }
}