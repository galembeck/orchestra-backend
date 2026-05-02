using Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region .: ENTITIES :.

    public DbSet<User> Users { get; set; }
    public DbSet<UserSecurityInfo> UserSecurityInfos { get; set; }
    public DbSet<UserHistoric> UserHistorics { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyDocument> CompanyDocuments { get; set; }
    public DbSet<CompanyMember> CompanyMembers { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    #endregion .: ENTITIES :.

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Company>().HasIndex(c => c.Cnpj).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email);

        modelBuilder.Entity<CompanyMember>()
            .HasIndex(m => new { m.CompanyId, m.UserId })
            .IsUnique();

        modelBuilder.Entity<Permission>().HasIndex(p => p.Key).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => new { r.CompanyId, r.Key });
        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();

        modelBuilder.Model.SetMaxIdentifierLength(30);

        modelBuilder.Model.ToDebugString();
    }
}
