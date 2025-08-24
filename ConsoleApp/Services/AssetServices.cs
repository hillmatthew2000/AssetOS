using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

    //Creates a new asset in the database
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

    //Update a new asset in the database
    public async Task<bool> UpdateAssetAsync(Asset asset)
    {
        var existingAsset = await _context.Asset.FindAsync();
        if (existingAsset == null) return false;

        existingAsset.AssetTag = asset.AssetTag;
        existingAsset.SerialNumber = asset.SerialNumber;
        existingAsset.Type = asset.Type;
    }
}