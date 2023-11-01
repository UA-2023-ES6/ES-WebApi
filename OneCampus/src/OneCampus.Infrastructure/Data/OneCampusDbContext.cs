﻿using Microsoft.EntityFrameworkCore;
using OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Infrastructure.Data;

public class OneCampusDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoleInstitution> UserRoleInstitution { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Message> Messages { get; set; }

    public OneCampusDbContext()
    {
    }

    public OneCampusDbContext(DbContextOptions<OneCampusDbContext> options) : base(options)
    {
    }
}