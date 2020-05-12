using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NatureRecorder.Entities.Db;

namespace NatureRecorder.Data
{
    [ExcludeFromCodeCoverage]
    public class NatureRecorderDbContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Species> Species { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Sighting> Sightings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public NatureRecorderDbContext(DbContextOptions<NatureRecorderDbContext> options) : base(options)
        {
        }
    }
}
