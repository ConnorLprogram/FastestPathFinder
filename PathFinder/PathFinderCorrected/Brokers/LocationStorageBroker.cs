using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PathFinder.Models;

namespace PathFinder.Brokers
{
    public class LocationStorageBroker : DbContext, ILocationStorage
    {
        public LocationStorageBroker(DbContextOptions<LocationStorageBroker> options)
            :base(options)
        {
            this.Database.Migrate();
        }
        
        public DbSet<Location> locations { get; set; }
        public DbSet<Pathlist> pathLocations { get; set; }

        public async ValueTask<Location> InsertLocationAsync(Location location)
        {
            EntityEntry<Location> locationEntry = this.locations.Add(location);
            await this.SaveChangesAsync();

            return locationEntry.Entity;
        }

        public async ValueTask<Location> FindLocationAsync(string name)
        {
            return await this.locations.FindAsync(name);
        }

        public async ValueTask<Location> FindLocationByDistanceAsync(double distance)
        {
            return await this.locations.FindAsync(distance);
        }

        public IQueryable<Location> SelectAllLocationsAsync()
        {
            return this.locations;
        }

        public IQueryable<Pathlist> SelectAllClosestAsync()
        {
            return this.pathLocations;
        }
    }
}
