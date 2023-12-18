using Microsoft.EntityFrameworkCore;
using OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Data;

public class OneCampusDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserGroupPermissions> UserGroupPermissions { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    public OneCampusDbContext()
    {
    }

    public OneCampusDbContext(DbContextOptions<OneCampusDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Institution>()
            .HasOne<Group>(g => g.Group)
            .WithOne(g => g.Institution)
            .HasForeignKey<Institution>(i => i.GroupId);

        base.OnModelCreating(modelBuilder);
    }
}
