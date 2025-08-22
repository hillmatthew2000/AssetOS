using System.Data;
using System.Security.Cryptography;
using ITAM.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITAM.ConsoleApp.Data;

public class ITAMDbContext : DbContext
{
    public DbSet<Asset> Asset => Set<Asset>();
    public DbSet<User> User => Set<User>();
    
}