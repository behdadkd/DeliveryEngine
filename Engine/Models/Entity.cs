using Microsoft.EntityFrameworkCore;

namespace Engine.Models;
public class Entity
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.;Database=DeliveryEngine;Trusted_Connection=True;Integrated Security=true;Encrypt=True;TrustServerCertificate=True");
    }
    public DbSet<Entity> Entities { get; set; }
}