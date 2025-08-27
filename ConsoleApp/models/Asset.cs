namespace ITAM.ConsoleApp.Models;

//Enumeration defining different types of assets
public enum AssetType { Laptop, Desktop, Server }
//Enumeration defining various states an asset can be in
public enum AssetStatus { InUse, Avaliable, InRepair, Retired }

//Asset entity representing hardware devices being tracked
public class Asset
{
    //Primary key identifier for the asset
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Cpu { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public string Storage { get; set; } = string.Empty;

    //Foreign key to User who currently has this asset assigned
    public int? AssignedUserId { get; set; }
    //Foreign key to User who last modified this record
    public int LastUpdatedById { get; set; }

    //Navigation property to the assigned user
    public User? AssignedUser { get; set; }
    //Navigation property to the user who last updated this asset
    public User? LastUpdatedBy { get; set; }

    public string Site { get; set; } = string.Empty;
    public string PhysicalLocation { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
    public decimal PurchasePrice { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}