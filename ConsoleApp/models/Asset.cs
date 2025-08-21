namespace ConsoleApp.Models;

{
public enum AssetType { Laptop, Desktop, Server }
public enum AssetStatus { InUser, Avaliable, InRepair, Retired }

public class Asset
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string SerioalNumber { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public 
}

}