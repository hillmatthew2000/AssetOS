using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.ConsoleApp.Services;

//Service class for managing Asset entities
public class AssetService
{
    private readonly ITAMDbContext _context;

    //Constructor injecting the database context
    public AssetService(ITAMDbContext context)
    {
        _context = context;
    }

    //Retrieves all assets including related AssignedUser and LastUpdatedBy
    public async Task<List<Asset>> GetAllAssetsAsync()
    {
        return await _context.Asset
        .Include(a => a.AssignedUser)
        .Include(a => a.LastUpdatedBy)
        .ToListAsync();
    }

    //Retrieves a single asset by its Id, including related entities
    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        return await _context.Asset
        .Include(a => a.AssignedUser)
        .Include(a => a.LastUpdatedBy)
        .FirstOrDefaultAsync(a => a.Id == id);
    }

    //Example method for retrieving an asset by its tag (commented out)
    // public async Task<Asset?> GetAssetByTagAsync(string tag)
    // {
    //     return await _context.Asset
    //     .Include(a => a.AssignedUser)
    //     .Include(a => a.LastUpdatedBy)
    //     .FirstOrDefaultAsync(a => a.AssetTag == tag);
    // }

    //Creates and saves a new asset in the database, setting default status and last updated timestamp
    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        //1. Set default values
        asset.Status = AssetStatus.Avaliable;
        asset.LastUpdated = DateTime.UtcNow;

        //2. Register the entity for insertion ex. -> its like staging a file to be tracked but no commit yet
        _context.Asset.Add(asset);

        //3. Execute the database operation ex -> this is like the commit that queries the db
        await _context.SaveChangesAsync();

        //4. Returns the created entity
        return asset;

    }

    //Updates an existing asset with all provided values and saves to database
    public async Task<bool> UpdateAssetAsync(Asset asset)
    {
        //Find the asset by its Id
        var existingAsset = await _context.Asset.FindAsync(asset.Id);
        if (existingAsset == null) return false;

        //Update all asset properties with new values
        existingAsset.AssetTag = asset.AssetTag;
        existingAsset.SerialNumber = asset.SerialNumber;
        existingAsset.Type = asset.Type;
        existingAsset.Manufacturer = asset.Manufacturer;
        existingAsset.Model = asset.Model;
        existingAsset.Cpu = asset.Cpu;
        existingAsset.Ram = asset.Ram;
        existingAsset.Storage = asset.Storage;
        existingAsset.AssignedUserId = asset.AssignedUserId;
        existingAsset.Site = asset.Site;
        existingAsset.PhysicalLocation = asset.PhysicalLocation;
        existingAsset.Status = asset.Status;
        existingAsset.PurchaseDate = asset.PurchaseDate;
        existingAsset.WarrantyExpiry = asset.WarrantyExpiry;
        existingAsset.PurchasePrice = asset.PurchasePrice;
        existingAsset.Supplier = asset.Supplier;
        existingAsset.LastUpdated = DateTime.UtcNow;
        existingAsset.LastUpdatedById = asset.LastUpdatedById;

        try
        {
            //Save changes to the database
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            //Return false if an error occurs during update
            return !_context.Asset.Any(e => e.Id == asset.Id);
        }
    }

    //Deletes an asset from the database by its ID
    public async Task<bool> DeleteAssetAsync(int id)
    {
        //Find the asset by its Id
        var asset = await _context.Asset.FindAsync(id);
        if (asset == null) return false;

        //Remove the asset from context
        _context.Asset.Remove(asset);

        //Save changes to the database
        await _context.SaveChangesAsync();
        return true;
    }

    //Retrieves assets with warranties expiring within specified number of days
    public async Task<List<Asset>> GetWarrantyExpiringAssetsAsync(int days)
    {
        var xDaysFromNow = DateTime.UtcNow.AddDays(days);
        return await _context.Asset
        .Where(a => a.WarrantyExpiry.HasValue &&
        a.WarrantyExpiry <= xDaysFromNow)
        .ToListAsync();
    }
}