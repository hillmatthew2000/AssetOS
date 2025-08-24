using ITAM.ConsoleApp.Data;
using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ITAM.ConsoleApp.Services;

public class AssetService
{
    private readonly ITAMDbContext _context;

    public AssetService(ITAMDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asset>> GetAllAssetsAsync()
    {
        return await _context.Asset
        .Include(a => a.AssignedUser)
        .Include(a => a.LastUpdatedBy)
        .ToListAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        return await _context.Asset
        .Include(a => a.AssignedUser)
        .Include(a => a.LastUpdatedBy)
        .FirstOrDefaultAsync(a => a.Id == id);
    }

    // //Made this for fun to test different ways to pull a single asset
    // public async Task<Asset?> GetAssetByTagAsync(string tag)
    // {
    //     return await _context.Asset
    //     .Include(a => a.AssignedUser)
    //     .Include(a => a.LastUpdatedBy)
    //     .FirstOrDefaultAsync(a => a.AssetTag == tag);
    // }

    

}