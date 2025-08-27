namespace ITAM.ConsoleApp.Models;

//User entity representing employees who can be assigned assets
public class User
{
    //Primary key identifier for the user
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    //Navigation property for assets currently assigned to this user
    public List<Asset> AssignedAssets { get; set; } = new();
    //Navigation property for assets last updated by this user
    public List<Asset> UpdatedAssets { get; set; } = new();
}
