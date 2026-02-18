using ContactManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.DAL;

public class ContactManagerDbContext : DbContext
{
    public DbSet<Contacts> Contacts { get; set; }

    public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactManagerDbContext).Assembly);
    }
}
