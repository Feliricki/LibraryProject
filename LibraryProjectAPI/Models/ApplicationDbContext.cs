using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryProjectAPI.Models;

public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Books>().HasKey(e => e.Id);
        builder.Entity<Reviews>().HasKey(e => e.Id);
    }

    public DbSet<Books> Books => Set<Books>();
    public DbSet<Reviews> Reviews => Set<Reviews>();
}