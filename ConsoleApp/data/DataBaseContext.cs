using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.ConsoleApp.Data;

//Class to inherent from the DbContext class of MSEFCore
public class ITAMDbContext : DbContext
{
    //Creates tables for both properties that can be added to and queried
    public DbSet<Asset> Asset => Set<Asset>();
    public DbSet<User> User => Set<User>();

    //Method that configures the database connection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Uses a temporary, fake DB in RAM named ITAMDb
        optionsBuilder.UseInMemoryDatabase("ITAMDb");
    }

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

        //Seed initial data for both tables
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

        modelBuilder.Entity<Asset>().HasData(
            new Asset
            {
                Id = 1,
                AssetTag = "LAP-1001",
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
                LastUpdatedById = 2
            });
            
            
            
    }


}