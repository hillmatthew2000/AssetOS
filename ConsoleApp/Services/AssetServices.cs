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
        return await _context.Assets
        .Include(a => a.AssignedUser)
        .Include(a => a.LastUpdatedBy)
        .ToListAsync();
    }

}