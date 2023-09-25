using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Appuser> Users { get; set; }
}
