using Microsoft.EntityFrameworkCore;
using OnionCSharpPoc.Movies;

namespace OnionCSharpPoc.Infrastructure;

public class DataContext : DbContext
{
    public DbSet<MovieModel> Movies { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}
