using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using ITAM.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace ConsoleApp;

public class Program
{
    //Static fields so all static methods can access the context and services
    private static ITAMDbContext? _context;
    private static AssetService? _assetService;
    private static UserService? _userService;

    static async Task Main(string[] args)
    {
        //Creates DbContext options for SQLite
        var options = new DbContextOptionsBuilder<ITAMDbContext>()
        .UseSqlite($"Data Source=Itam.db").Options;

        //Creates new instance of the ITAMDbContext class essentially making the "database"
        _context = new ITAMDbContext(options);

        //Ensures that the "database" is created
        await _context.Database.EnsureCreatedAsync();

        //Initialize services and assign to static fields so other methods can use them
        _assetService = new AssetService(_context);
        _userService = new UserService(_context);

        //sanity check - these must be initialized for static methods to work
        if (_assetService == null || _userService == null)
        {
            Console.WriteLine("Failed to initialize services.");
            return;
        }

        DisplayAssetOSHeader();

        //Initialize program and display main menu
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
                    await ShowAssetMenu();
                    break;
                case "2":
                    await ListUsers();
                    break;
                case "3":
                    await WarrantyExpiryReport();
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

    //Displays the asset management submenu
    private static async Task ShowAssetMenu()
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
                await AddNewAsset();
                break;
            case "2":
                await UpdateEntireAsset();
                break;
            case "3":
                await UpdateSpecificAssetInfo();
                break;
            case "4":
                await DeleteAsset();
                break;
            case "5":
                await ListAllAssets();
                break;
            case "6":
                return; // Return to main menu
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }

    //Retrieves and displays all assets with their details
    private static async Task ListAllAssets()
    {
        Console.WriteLine("\n=== All Assets ===");
    var assets = await _assetService!.GetAllAssetsAsync();

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
            var Users = await _userService!.GetAllUsersAsync();

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
                var newAsset = await _assetService!.CreateAssetAsync(asset);
            Console.WriteLine($"\nAsset created successfully! ID: {newAsset.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError creating asset: {ex.Message}");
        }
    }

    //Updates existing asset with new values from user input
    private static async Task UpdateEntireAsset()
    {
        Console.WriteLine("\n=== Update Asset ===");
        Console.Write("Enter Asset ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out var assetID))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

    var asset = await _assetService!.GetAssetByIdAsync(assetID);
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
    var users = await _userService!.GetAllUsersAsync();
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

    //Update specific property within an existing asset based on user input
    private static async Task UpdateSpecificAssetInfo()
    {
        Console.WriteLine("\n=== Update Asset ===");
        Console.Write("Enter Asset ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out var assetID))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

    var asset = await _assetService!.GetAssetByIdAsync(assetID);
        if (asset == null)
        {
            Console.WriteLine("Asset not found");
            return;
        }

        //Get user input to determine which property to update
        Console.WriteLine("What information would you like to change?");
        Console.WriteLine("1. Asset Tag");
        Console.WriteLine("2. Serial Number");
        Console.WriteLine("3. Type");
        Console.WriteLine("4. Manufacturer");
        Console.WriteLine("5. Model");
        Console.WriteLine("6. CPU");
        Console.WriteLine("7. RAM");
        Console.WriteLine("8. Storage");
        Console.WriteLine("9. Site");
        Console.WriteLine("10. Physical Location");
        Console.WriteLine("11. Purchase Date");
        Console.WriteLine("12. Warranty Expiry");
        Console.WriteLine("13. Purchase Price");
        Console.WriteLine("14. Supplier");
        Console.WriteLine("15. Status");
        Console.WriteLine("16. Assigned User");
        Console.WriteLine("17. Last Updated By");
        Console.Write("Select property number to update: ");

        switch (Console.ReadLine())
        {
            case "1":
            {
            Console.WriteLine($"Current Asset Tag: {asset.AssetTag}");
            Console.Write("New Asset Tag (leave blank to keep current): ");
            var newTag = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTag))
                asset.AssetTag = newTag;
            break;
            }
            case "2":
            {
            Console.WriteLine($"Current Serial Number: {asset.SerialNumber}");
            Console.Write("New Serial Number (leave blank to keep current): ");
            var newSerial = Console.ReadLine();
            if (!string.IsNullOrEmpty(newSerial))
                asset.SerialNumber = newSerial;
            break;
            }
            case "3":
            {
            Console.WriteLine($"Current Type: {asset.Type}");
            Console.WriteLine("New Type:");
            Console.WriteLine("1. Laptop");
            Console.WriteLine("2. Desktop");
            Console.WriteLine("3. Server");
            Console.Write("Select Type: ");
            var typeInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(typeInput) && int.TryParse(typeInput, out var typeVal))
                asset.Type = (AssetType)(typeVal - 1);
            break;
            }
            case "4":
            {
            Console.WriteLine($"Current Manufacturer: {asset.Manufacturer}");
            Console.Write("New Manufacturer (leave blank to keep current): ");
            var newManufacturer = Console.ReadLine();
            if (!string.IsNullOrEmpty(newManufacturer))
                asset.Manufacturer = newManufacturer;
            break;
            }
            case "5":
            {
            Console.WriteLine($"Current Model: {asset.Model}");
            Console.Write("New Model (leave blank to keep current): ");
            var newModel = Console.ReadLine();
            if (!string.IsNullOrEmpty(newModel))
                asset.Model = newModel;
            break;
            }
            case "6":
            {
            Console.WriteLine($"Current CPU: {asset.Cpu}");
            Console.Write("New CPU (leave blank to keep current): ");
            var newCpu = Console.ReadLine();
            if (!string.IsNullOrEmpty(newCpu))
                asset.Cpu = newCpu;
            break;
            }
            case "7":
            {
            Console.WriteLine($"Current RAM: {asset.Ram}");
            Console.Write("New RAM (leave blank to keep current): ");
            var newRam = Console.ReadLine();
            if (!string.IsNullOrEmpty(newRam))
                asset.Ram = newRam;
            break;
            }
            case "8":
            {
            Console.WriteLine($"Current Storage: {asset.Storage}");
            Console.Write("New Storage (leave blank to keep current): ");
            var newStorage = Console.ReadLine();
            if (!string.IsNullOrEmpty(newStorage))
                asset.Storage = newStorage;
            break;
            }
            case "9":
            {
            Console.WriteLine($"Current Site: {asset.Site}");
            Console.Write("New Site (leave blank to keep current): ");
            var newSite = Console.ReadLine();
            if (!string.IsNullOrEmpty(newSite))
                asset.Site = newSite;
            break;
            }
            case "10":
            {
            Console.WriteLine($"Current Physical Location: {asset.PhysicalLocation}");
            Console.Write("New Physical Location (leave blank to keep current): ");
            var newLocation = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLocation))
                asset.PhysicalLocation = newLocation;
            break;
            }
            case "11":
            {
            Console.WriteLine($"Current Purchase Date: {asset.PurchaseDate:yyyy-MM-dd}");
            Console.Write("New Purchase Date (YYYY-MM-DD, leave blank to keep current): ");
            var newPurchaseDateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPurchaseDateInput) && DateTime.TryParse(newPurchaseDateInput, out var newPurchaseDate))
                asset.PurchaseDate = newPurchaseDate;
            break;
            }
            case "12":
            {
            Console.WriteLine($"Current Warranty Expiry: {asset.WarrantyExpiry:yyyy-MM-dd}");
            Console.Write("New Warranty Expiry (YYYY-MM-DD, leave blank to keep current): ");
            var newWarrantyInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(newWarrantyInput) && DateTime.TryParse(newWarrantyInput, out var newWarranty))
                asset.WarrantyExpiry = newWarranty;
            break;
            }
            case "13":
            {
            Console.WriteLine($"Current Purchase Price: {asset.PurchasePrice}");
            Console.Write("New Purchase Price (leave blank to keep current): ");
            var newPriceInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPriceInput) && decimal.TryParse(newPriceInput, out var newPrice))
                asset.PurchasePrice = newPrice;
            break;
            }
            case "14":
            {
            Console.WriteLine($"Current Supplier: {asset.Supplier}");
            Console.Write("New Supplier (leave blank to keep current): ");
            var newSupplier = Console.ReadLine();
            if (!string.IsNullOrEmpty(newSupplier))
                asset.Supplier = newSupplier;
            break;
            }
            case "15":
            {
            Console.WriteLine($"Current Status: {asset.Status}");
            Console.WriteLine("New Status:");
            Console.WriteLine("1. In Use");
            Console.WriteLine("2. Available");
            Console.WriteLine("3. In Repair");
            Console.WriteLine("4. Retired");
            Console.Write("Select Status: ");
            var specificStatusInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(specificStatusInput) && int.TryParse(specificStatusInput, out var statusVal))
                asset.Status = (AssetStatus)(statusVal - 1);
            break;
            }
            case "16":
            {
            Console.WriteLine("Available Users:");
            var specificUsers = await _userService!.GetAllUsersAsync();
            foreach (var user in specificUsers)
            {
                Console.WriteLine($"{user.Id} {user.Name} ({user.Department})");
            }
            Console.Write($"Assign to User ID (current: {asset.AssignedUserId}) (leave blank to keep current): ");
            var specificUserIdInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(specificUserIdInput) && int.TryParse(specificUserIdInput, out var specificUserId))
                asset.AssignedUserId = specificUserId;
            break;
            }
            case "17":
            {
            Console.Write($"Last Updated By User ID (current: {asset.LastUpdatedById}): ");
            var lastUpdatedByInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(lastUpdatedByInput) && int.TryParse(lastUpdatedByInput, out var lastUpdatedById))
                asset.LastUpdatedById = lastUpdatedById;
            break;
            }
            default:
            {
            Console.WriteLine("Invalid option. Please try again.");
            break;
            }
        }
        
        try
        {
            bool success = await _assetService!.UpdateAssetAsync(asset);
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
            var success = await _assetService!.DeleteAssetAsync(assetid);
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
    var users = await _userService!.GetAllUsersAsync();

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
    var assets = await _assetService!.GetWarrantyExpiringAssetsAsync(days);

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
        var subtitle = new Rule("[bold blue]Phase 2 - SQLite Database Backend[/]")
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