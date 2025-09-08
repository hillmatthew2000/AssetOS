using ITAM.ConsoleApp.Services;

namespace ITAM.ConsoleApp.Managers;

public class MenuManager
{
    //Manager dependencies for asset and user operations
    private readonly AssetManager _assetManager;
    private readonly UserManager _userManager;

    //Constructor to inject required managers
    public MenuManager(AssetManager assetManager, UserManager userManager)
    {
        _assetManager = assetManager;
        _userManager = userManager;
    }

    //Display and handle main menu navigation
    public async Task ShowMainMenuAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Asset Management");
            Console.WriteLine("2. List Users");
            Console.WriteLine("3. Warranty Expiry Report");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Select an option");

            switch (Console.ReadLine())
            {
                case "1":
                    await ShowAssetMenuAsync();
                    break;
                case "2":
                    await _userManager.ListUsersAsync();
                    break;
                case "3":
                    await _assetManager.WarrantyExpiryReportAsync();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Thank you for using AssetOS!");
    }

    //Display and handle asset management submenu
    private async Task ShowAssetMenuAsync()
    {
        Console.WriteLine("\n=== Asset Management ===");
        Console.WriteLine("1. Add New Asset");
        Console.WriteLine("2. Update Entire Asset");
        Console.WriteLine("3. Update Specific Asset Info");
        Console.WriteLine("4. Delete Asset");
        Console.WriteLine("5. List All Assets");
        Console.WriteLine("6. Return to Main Menu");
        Console.WriteLine("Select an option:");

        switch (Console.ReadLine())
        {
            case "1":
                await _assetManager.AddNewAssetAsync();
                break;
            case "2":
                await _assetManager.UpdateEntireAssetAsync();
                break;
            case "3":
                await _assetManager.UpdateSpecificAssetInfoAsync();
                break;
            case "4":
                await _assetManager.DeleteAssetAsync();
                break;
            case "5":
                await _assetManager.ListAllAssetsAsync();
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
