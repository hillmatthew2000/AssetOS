namespace ConsoleApp.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public List<Asset> AssignedAssets { get; set; } = new();
    public List<Asset> UpdatedAssets { get; set; } = new();
}
