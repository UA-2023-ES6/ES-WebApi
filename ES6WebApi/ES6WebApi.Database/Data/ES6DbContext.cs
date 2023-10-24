using ES6WebApi.Database.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ES6WebApi.Database.Data;

public class ES6DbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoleInstitution> UserRoleInstitution { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ES6DbContext()
    {
    }

    public ES6DbContext(DbContextOptions<ES6DbContext> options) : base(options)
    {
    }
}
