using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using ITAM.ConsoleApp.Services;
using Spectre.Console;

namespace ConsoleApp;

public class Program
{
    //Creates new instance of the ITAMDbContext class essentially making the "database"
    private static ITAMDbContext _context = new();
    //Creates new instance of both AssetService and UserService classes with the "database" as parameter
    private static AssetService _assetService = new(_context);
    private static UserService _userService = new(_context);
    static async Task Main(string[] args)
    {
        //Ensures that the "database" is created
        await _context.Database.EnsureCreatedAsync();

        DisplayAssetOSHeader();

        //Initialize program and display main menu
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. List All Assets");
            Console.WriteLine("2. Add New Asset");
            Console.WriteLine("3. Update Asset");
            Console.WriteLine("4. Delete Asset");
            Console.WriteLine("5. List Users");
            Console.WriteLine("6. Warranty Expiry Report");
            Console.WriteLine("7. Exit");
            Console.WriteLine("Select an option");

            switch (Console.ReadLine())
            {
                case "1":
                    await ListAllAssets();
                    break;
                case "2":
                    await AddNewAsset();
                    break;
                case "3":
                    await UpdateAsset();
                    break;
                case "4":
                    await DeleteAsset();
                    break;
                case "5":
                    await ListUsers();
                    break;
                case "6":
                    await WarrantyExpiryReport();
                    break;
                case "7":
                    running = false;
                    break;
                default:
                    Console.WriteLine("invalide option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Thank you for using AssetOS!");
    }

    //Retrieves and displays all assets with their details
    private static async Task ListAllAssets()
    {
        Console.WriteLine("\n=== All Assets ===");
        var assets = await _assetService.GetAllAssetsAsync();

        if (!assets.Any())
        {
            Console.WriteLine("No assets found");
            return;
        }

        //Display each asset's information
        foreach (var asset in assets)
        {
            Console.WriteLine($"ID: {asset.Id}");
            Console.WriteLine($"Tag: {asset.AssetTag}");
            Console.WriteLine($"Model: {asset.Manufacturer} {asset.Model}");
            Console.WriteLine($"Type: {asset.Type}");
            Console.WriteLine($"Status: {asset.Status}");
            Console.WriteLine($"Assigned to: {asset.AssignedUser?.Name ?? "Unassigned"}");
            Console.WriteLine($"Location: {asset.Site} - {asset.PhysicalLocation}");
            Console.WriteLine($"Warranty: {asset.WarrantyExpiry?.ToString("yyyy-MM-dd") ?? "N/A"}");
            Console.WriteLine($"---------------------------------");
        }
    }

    //Prompts user for asset details and creates a new asset
    private static async Task AddNewAsset()
    {
        var asset = new Asset();

        Console.Write("\nAsset Tag: ");
        asset.AssetTag = Console.ReadLine() ?? string.Empty;

        Console.Write("Serial Number: ");
        asset.SerialNumber = Console.ReadLine() ?? string.Empty;

        //Display asset type options and get user selection
        Console.WriteLine("Asset Type:");
        Console.WriteLine("1. Laptop");
        Console.WriteLine("2. Desktop");
        Console.WriteLine("3. Server");
        Console.Write("Select Type: ");
        asset.Type = (AssetType)(int.Parse(Console.ReadLine() ?? "1") - 1);

        Console.Write("Manufacturer: ");
        asset.Manufacturer = Console.ReadLine() ?? string.Empty;

        Console.Write("Model: ");
        asset.Model = Console.ReadLine() ?? string.Empty;

        Console.Write("CPU: ");
        asset.Cpu = Console.ReadLine() ?? string.Empty;

        Console.Write("Ram: ");
        asset.Ram = Console.ReadLine() ?? string.Empty;

        Console.Write("Storage: ");
        asset.Storage = Console.ReadLine() ?? string.Empty;

        Console.Write("Site: ");
        asset.Site = Console.ReadLine() ?? string.Empty;

        Console.Write("Physical Location: ");
        asset.PhysicalLocation = Console.ReadLine() ?? string.Empty;

        //Parse date inputs with validation
        Console.Write("Purchase Date (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out var purchaseDate))
            asset.PurchaseDate = purchaseDate;

        Console.Write("Warranty Expiry (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out var warrantyExpiry))
            asset.WarrantyExpiry = warrantyExpiry;

        Console.Write("Purchase Price: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal price))
            asset.PurchasePrice = price;

        Console.Write("Supplier: ");
        asset.Supplier = Console.ReadLine() ?? string.Empty;

        //Show available users for assignment
        Console.WriteLine("Available Users: ");
        var Users = await _userService.GetAllUsersAsync();

        foreach (var user in Users)
        {
            Console.WriteLine($"{user.Id} {user.Name} ({user.Department})");
        }
        Console.Write("Assign to User ID (leave blank for unassigned): ");
        var userIdInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(userIdInput) && int.TryParse(userIdInput, out var userId))
        {
            asset.AssignedUserId = userId;
            asset.Status = AssetStatus.InUse;
        }

        Console.Write("Last Updated By User ID: ");
        asset.LastUpdatedById = int.Parse(Console.ReadLine() ?? "1");

        try
        {
            var newAsset = await _assetService.CreateAssetAsync(asset);
            Console.WriteLine($"\nAsset created successfully! ID: {newAsset.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError creating asset: {ex.Message}");
        }
    }

    //Updates existing asset with new values from user input
    private static async Task UpdateAsset()
    {
        Console.WriteLine("\n=== Update Asset ===");
        Console.Write("Enter Asset ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out var assetID))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var asset = await _assetService.GetAssetByIdAsync(assetID);
        if (asset == null)
        {
            Console.WriteLine("Asset not found");
            return;
        }

        //Update asset tag if new value provided
        Console.WriteLine($"current Asset Tag: {asset.AssetTag}");
        Console.Write("New Asset Tag (leave blank to keep current): ");
        var newTag = Console.ReadLine();
        if (!string.IsNullOrEmpty(newTag))
            asset.AssetTag = newTag;

        //Update status if new value provided
        Console.WriteLine($"Current Status: {asset.Status}");
        Console.WriteLine("New Status:");
        Console.WriteLine("1. In Use");
        Console.WriteLine("2. Available");
        Console.WriteLine("3. In Repair");
        Console.WriteLine("4. Retired");
        Console.WriteLine("Select Status:");
        var statusInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(statusInput))
            asset.Status = (AssetStatus)(int.Parse(statusInput) - 1);

        //Show available users and update assignment if new value provided
        Console.WriteLine("Available Users:");
        var users = await _userService.GetAllUsersAsync();
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id} {user.Name} ({user.Department})");
        }
        Console.Write($"Assign to User ID (current: {asset.AssignedUserId}) (leave blank to keep current): ");
        var userIdInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(userIdInput) && int.TryParse(userIdInput, out var userId))
            asset.AssignedUserId = userId;

        Console.Write($"Last Updated By User ID: ");
        asset.LastUpdatedById = int.Parse(Console.ReadLine() ?? "1");

        try
        {
            bool success = await _assetService.UpdateAssetAsync(asset);
            Console.WriteLine(success ? "\nAsset updated successfully!" : "\nFailed to update asset.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError updating asset: {ex.Message}");
        }
    }

    //Deletes an asset by ID after getting user input
    private static async Task DeleteAsset()
    {
        Console.WriteLine("\n===Delete Asset ===");
        Console.Write("Enter Asset ID to delete: ");

        if (!int.TryParse(Console.ReadLine(), out var assetid))
        {
            Console.WriteLine("Invalide ID");
            return;
        }

        try
        {
            var success = await _assetService.DeleteAssetAsync(assetid);
            Console.WriteLine(success ? "\nAsset deleted successfully!" : "\nAsset not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError deleting asset: {ex.Message}");
        }
    }

    //Retrieves and displays all users in the system
    private static async Task ListUsers()
    {
        Console.WriteLine("\n=== All Users ===");
        var users = await _userService.GetAllUsersAsync();

        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        //Display each user's information
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Name: {user.Name}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Department: {user.Department}");
            Console.WriteLine("----------------------------------");
        }
    }

    //Generates report of assets with warranties expiring within specified days
    private static async Task WarrantyExpiryReport()
    {
        Console.WriteLine("Enter the number of days to check for upcoming warranty expirations:");
        int.TryParse(Console.ReadLine(), out int days);
        Console.WriteLine("\n=== Warranty Expiry Report ===");
        var assets = await _assetService.GetWarrantyExpiringAssetsAsync(days);

        if (!assets.Any())
        {
            Console.WriteLine($"No assets with expiring warranties in the next {days} days");
            return;
        }

        //Display warranty information for each expiring asset
        foreach (var asset in assets)
        {
            Console.WriteLine($"ID: {asset.Id}");
            Console.WriteLine($"Tag: {asset.AssetTag}");
            Console.WriteLine($"Model: {asset.Manufacturer} {asset.Model}");
            Console.WriteLine($"Warranty Expires: {asset.WarrantyExpiry?.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"Days Remaining: {(asset.WarrantyExpiry - DateTime.UtcNow)?.Days}");
            Console.WriteLine("----------------------------------");
        }
    }

    //Method that creates a beautiful AssetOS header using Spectre.Console
    private static void DisplayAssetOSHeader()
    {
        //Clear console for clean display
        Console.Clear();

        //Create a beautiful panel with AssetOS title
        var panel = new Panel(
            new FigletText("AssetOS")
                .Centered()
                .Color(Spectre.Console.Color.Cyan1))
        {
            Border = BoxBorder.Double,
            BorderStyle = new Style(foreground: Spectre.Console.Color.Cyan1),
            Header = new PanelHeader("[bold yellow]IT Asset Management System[/]"),
            Padding = new Padding(2, 1, 2, 1)
        };

        AnsiConsole.Write(panel);

        //Add a stylized subtitle
        var subtitle = new Rule("[bold blue]Phase 1 - Core Hardware Tracking[/]")
        {
            Style = Style.Parse("cyan1"),
            Justification = Justify.Center
        };

        AnsiConsole.Write(subtitle);

        //Add some spacing
        AnsiConsole.WriteLine();

        //Set console foreground color to yellow for subsequent menu text
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
        
    }
