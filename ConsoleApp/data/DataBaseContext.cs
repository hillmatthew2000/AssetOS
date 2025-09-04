using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.ConsoleApp.Data;

//Database context class inheriting from Entity Framework's DbContext
public class ITAMDbContext : DbContext
{
    //DbSet properties representing database tables that can be queried and modified
    public DbSet<Asset> Asset => Set<Asset>();
    public DbSet<User> User => Set<User>();

    //Constructor to accept DbContextOptions
    public ITAMDbContext(DbContextOptions<ITAMDbContext> options) : base(options) { }

    //Configures the database connection and options
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Checks to see if optionsBuilder is already configured
        if (!optionsBuilder.IsConfigured)
        {
            //Define the path for the SQLite database file
            var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConsoleApp",
            "Itam.db");

            //Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            //Uses a SQLite database for permanent storage
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    //Configures entity relationships and seeds initial data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Configure Asset -> AssignedUser relationship
        modelBuilder.Entity<Asset>()
        .HasOne(a => a.AssignedUser)
        .WithMany(u => u.AssignedAssets)
        .HasForeignKey(a => a.AssignedUserId)
        .OnDelete(DeleteBehavior.SetNull);

        //Configure Asset -> LastUpdatedBy relationship
        modelBuilder.Entity<Asset>()
        .HasOne(a => a.LastUpdatedBy)
        .WithMany(u => u.UpdatedAssets)
        .HasForeignKey(a => a.LastUpdatedById)
        .OnDelete(DeleteBehavior.Restrict);

        //Seed initial user data for testing and demonstration
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Bob Johnson",
                Email = "bob@company.com",
                Department = "Engineering"
            },
            new User
            {
                Id = 2,
                Name = "Alice Smith",
                Email = "alice@company.com",
                Department = "IT Support"
            });

        //Seed initial asset data for testing and demonstration
        modelBuilder.Entity<Asset>().HasData(
            new Asset
            {
                Id = 1,
                AssetTag = "LAP-001",
                SerialNumber = "5CD92927XFL",
                Type = AssetType.Laptop,
                Manufacturer = "Dell",
                Model = "Latitude 7420",
                Cpu = "Intel i7-11800H",
                Ram = "16GB DDR4",
                Storage = "512GB NVMe SSD",
                AssignedUserId = 1,
                Site = "HQ - Floor 3",
                PhysicalLocation = "Cubicle 4B",
                Status = AssetStatus.InUse,
                PurchaseDate = new DateTime(2023, 1, 15),
                WarrantyExpiry = new DateTime(2026, 1, 15),
                PurchasePrice = 1299.99m,
                Supplier = "CDW",
                LastUpdatedById = 2,
                LastUpdated = new DateTime(2025, 8, 1)
            });
    }
}