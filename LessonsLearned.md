# IT Asset Management System: Technical Study Notes

## 1. Project Overview

### Objective
Build a console-based IT Asset Management (ITAM) system tracking hardware assets (laptops, desktops, servers)

### Technology Stack
- **C#** with .NET
- **Entity Framework Core (EF Core)** for data access
- **SQLite** for persistent storage
- **Console UI** for user interaction

---

## 2. Asynchronous Programming in C#

### Key Concepts

#### Async/Await Pattern
- Used for I/O-bound operations (database access, file operations)
- Prevents blocking the main thread
- Improves application responsiveness

#### Task-Based Asynchronous Pattern (TAP)
- Methods return `Task` or `Task<T>`
- `async` modifier enables `await` keyword
- `await` pauses method execution until task completes

### Best Practices
- Use async all the way down (avoid mixing sync and async)
- Avoid `async void` (except for event handlers)
- Configure await with `ConfigureAwait(false)` in library code
- Handle exceptions with try/catch blocks

---

## 3. Microsoft Entity Framework Core (EF Core)

### Overview
- **ORM (Object-Relational Mapper)**: Maps C# objects to database tables
- **DbContext**: Central class for database operations
- **DbSet<TEntity>**: Represents collections of entities
- **Change Tracking**: Automatically tracks modifications to entities

### Configuration
- **DbContext Setup**: Defines database connection and entity mappings
- **Database Initialization**: Creates database schema if it doesn't exist

### Relationships and Navigation Properties

#### One-to-Many Relationship Configuration
- **Fluent API**: Explicitly defines relationships between entities
- **Foreign Keys**: Properties that link to related entities
- **Navigation Properties**: Enable entity graph traversal

#### Delete Behaviors
- **SetNull**: Sets foreign key to null (for optional relationships)
- **Restrict**: Prevents deletion of referenced entity
- **Cascade**: Automatically deletes dependent entities

### Database Providers

#### In-Memory Database
- **Pros**: Fast, no setup required, ideal for testing
- **Cons**: Data lost on application restart

#### SQLite
- **Pros**: Persistent storage, file-based, zero configuration
- **Cons**: Limited concurrency support, no server mode

---

## 4. Database Design

### Entity Models
- **Asset Entity**: Represents hardware assets with properties like AssetTag, SerialNumber, Type, etc.
- **User Entity**: Represents users with properties like Name, Email, Department, etc.

### Relationships
- **One-to-Many**: One User can have many Assets
- **Foreign Keys**: Properties that link entities (AssignedUserId, LastUpdatedById)
- **Navigation Properties**: Enable entity graph traversal

### Data Annotations vs Fluent API
- **Data Annotations**: Attributes applied directly to model properties
- **Fluent API**: Programmatic configuration in DbContext

---

## 5. Error Handling

### Common Issues

#### Relationship Configuration Error
- **Cause**: EF Core couldn't automatically determine relationships between entities
- **Solution**: Explicitly configure relationships using Fluent API

#### Database Provider Issues
- **Cause**: Missing NuGet packages or incorrect connection strings
- **Solution**: Install correct EF Core provider package and verify connection string

### Solutions
- **Explicit Relationship Configuration**: Define relationships in DbContext using Fluent API
- **Proper Exception Handling**: Use try/catch blocks for database operations

---

## 6. Migration from In-Memory to SQLite

### Steps
1. **Update NuGet Packages**: Remove in-memory provider, add SQLite provider
2. **Modify DbContext Configuration**: Update connection string to use SQLite
3. **Update Database Initialization**: Ensure database is created on startup

### Considerations
- **Data Persistence**: SQLite maintains data between application runs
- **File Location**: Store database in user-writable location
- **Concurrency**: SQLite has simpler concurrency than SQL Server
- **Data Types**: SQLite stores dates as TEXT, decimals as TEXT

---

## 7. Best Practices

### Code Organization
- **Separation of Concerns**: Organize code into Models, Data, Services, and UI layers
- **Dependency Injection**: Inject dependencies into services for better testability

### EF Core Best Practices
- **Async Operations**: Use async methods for database access
- **Change Tracking**: EF Core automatically tracks entity changes
- **Performance**: Use `AsNoTracking()` for read-only operations
- **Validation**: Validate entities before saving to database

### Database Design
- **Normalization**: Organize data to reduce redundancy
- **Relationships**: Clearly define entity relationships
- **Indexes**: Add indexes for frequently queried columns
- **Constraints**: Use foreign keys to maintain referential integrity

---

## 8. Key Learnings and Takeaways

### Asynchronous Programming
- Essential for I/O operations in modern applications
- Improves scalability and responsiveness
- Requires careful exception handling

### Entity Framework Core
- Powerful ORM that simplifies data access
- Requires explicit relationship configuration for complex scenarios
- Supports multiple database providers with minimal code changes

### Database Design
- Proper relationship configuration is crucial
- Foreign keys maintain data integrity
- Navigation properties enable object-oriented data access

### Error Handling
- EF Core errors often relate to relationship configuration
- Explicit configuration prevents runtime errors
- Delete behavior settings impact data integrity

### Migration Strategies
- Switching database providers is straightforward with EF Core
- Business logic remains unchanged during migration
- SQLite provides a good balance between simplicity and persistence

---

## 9. Next Steps

### Immediate Enhancements
- Implement additional asset properties (MAC Address, IP Address, etc.)
- Add software license tracking
- Implement maintenance history tracking

### Advanced Features
- Develop web API for broader access
- Add authentication and authorization
- Implement reporting and analytics features

---

## Quick Reference

### Essential EF Core Commands
```csharp
// Async operations
await context.SaveChangesAsync();
await context.Assets.ToListAsync();

// Relationships configuration (Fluent API)
modelBuilder.Entity<Asset>()
    .HasOne(a => a.AssignedUser)
    .WithMany(u => u.Assets)
    .HasForeignKey(a => a.AssignedUserId)
    .OnDelete(DeleteBehavior.SetNull);
```

### Common Patterns
- Always use async methods for database operations
- Configure relationships explicitly in `OnModelCreating`
- Handle exceptions properly with try/catch blocks
- Use `ConfigureAwait(false)` in library code

---

*Study Notes - IT Asset Management System Project*  
*Generated: [Current Date]*  
*Technology Stack: C#, .NET, Entity Framework Core, SQLite*