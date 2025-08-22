using System.Reflection.Metadata;

namespace ITAM.ConsoleApp.Models;

public enum AssetType { Laptop, Desktop, Server }
public enum AssetStatus { InUse, Avaliable, InRepair, Retired }

public class Asset
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Cpu { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;
    public int? AssignedUserId { get; set; }
    public User? AssingedUser { get; set; }
    public string Site { get; set; } = string.Empty;
    public string PhysicalLocation { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public decimal PurchasePrice { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int LastUpdatedById { get; set; }
    public User? LastUpdatedBy { get; set; }

}