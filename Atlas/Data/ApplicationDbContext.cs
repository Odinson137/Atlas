using Atlas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Atlas.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        if (Database.GetService<IDatabaseCreator>() is not RelationalDatabaseCreator databaseCreator) return;
        if (!databaseCreator.CanConnect()) databaseCreator.Create();
        if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
    }

    public DbSet<TelegramChat> TelegramChats { get; set; }
}