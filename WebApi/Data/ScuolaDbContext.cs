using Microsoft.EntityFrameworkCore;
using Models;

namespace WebApi.Data
{
    public class ScuolaDbContext : DbContext
    {
        public ScuolaDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<StudentiTest> Studente { get; set; }
    }
}
