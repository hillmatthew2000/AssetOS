using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using ITAM.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;

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

        //Displays header for program
        Console.WriteLine("=== IT Asset Management System ===");
        Console.WriteLine("Phase 1 - Core Hardware Tracking\n");

        //Initalize program and display main menu
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
            }
        }

        Console.WriteLine("Thank you for using AssetOS!");


    }

    private static async Task ListAllAssets()
    {
        Console.WriteLine("\n=== All Assets ===");
        var assets = await _assetService.GetAllAssetsAsync();

        if (!assets.Any())
        {
            Console.WriteLine("No assets found");
            return;
        }

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

    private static async Task AddNewAsset()
    {
        var asset = new Asset();

        Console.Write("Asset Tag: ");
        asset.AssetTag = Console.ReadLine() ?? string.Empty;

        Console.Write("Serial Number: ");
        asset.SerialNumber = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Asset Type:");
        Console.WriteLine("1. Laptop");
        Console.WriteLine("2. Desktop");
        Console.WriteLine("3. Server");
        Console.WriteLine("Select Type: ");
        asset.Type = (AssetType)(int.Parse(Console.ReadLine() ?? "1") - 1);

        Console.Write("Manufacturer: ");
        asset.Manufacturer = Console.ReadLine() ?? string.Empty;

        Console.Write("Model: ");
        asset.Model = Console.ReadLine() ?? string.Empty;

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

        Console.Write("Purchase Date (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out var purchaseDate))
            asset.PurchaseDate = purchaseDate;

        Console.Write("Warranty Expiry (YYYY-MM-DD): ");
        if (DateTime.TryParse(Console.ReadLine(), out var warrantyExpiry))
            asset.WarrantyExpiry = warrantyExpiry;

        Console.WriteLine("Purchase Price: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal price))
            asset.PurchasePrice = price;

        Console.WriteLine("Supplier: ");
        asset.Supplier = Console.ReadLine() ?? string.Empty;

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
    }
}