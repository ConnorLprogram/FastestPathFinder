using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PathFinder.Brokers;
using PathFinder.Models;

namespace PathFinder.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationStorage locationStorageBroker;

        public LocationService(ILocationStorage storage)
        {
            this.locationStorageBroker = storage;
        }

        public IQueryable<Location> GiveLocations()
        {
            return this.locationStorageBroker.SelectAllLocationsAsync();
        }

        public IQueryable<Location> SearchLocation(string name)
        {
            IQueryable<Location> storageLocation = this.locationStorageBroker.SelectAllLocationsAsync();

            IQueryable<Location> foundLocations = storageLocation.Where(location => location.name.Contains(name));

            return foundLocations;
        }
        public async ValueTask<Location> AddNewLocationAsync(Location location)
        {
            Location storageLocation = await this.locationStorageBroker.InsertLocationAsync(location);

            return storageLocation;
        }

        public Location FindLocation(string locationName)
        {
            Location foundLocation = this.locationStorageBroker.SelectAllLocationsAsync()
                .Where(location => location.name == locationName)
                .FirstOrDefault();
            
            return foundLocation;
        }

        public Location FindLocationByDistance(double distance)
        {
            Location foundLocation = this.locationStorageBroker.SelectAllLocationsAsync()
                .Where(location => location.distanceFromLocation == distance)
                .FirstOrDefault();

            return foundLocation;
        }
        public async ValueTask<Pathlist> FindPath(List<Pathlist> pathList, List<Location> allLocations, Location startLocation, Location endlocation)
        {
            FilterLocations(allLocations, startLocation, endlocation);
            FindPossibleLocations(pathList, allLocations, startLocation);
            FindCloseLocations(allLocations);
            Pathlist nextLocation = FindClosest(allLocations, endlocation.latitude, endlocation.longitude);

            return nextLocation;
        }
        public List<Location> FindPossibleLocations(List<Pathlist> pathList, List<Location> allLocations, Location startLocation)
        {
            double startLatitude = startLocation.latitude;
            double startLongitude = startLocation.longitude;

            foreach (Location location in allLocations)
            {
                if (location.possibleLocation == true)
                {
                    location.distanceFromLocation = Math.Sqrt((Math.Pow((startLatitude - location.latitude), 2) +
                        Math.Pow((startLongitude - location.longitude), 2)));
                }
                if (location.distanceFromLocation == 0 && location.possibleLocation== true)
                {
                    location.possibleLocation = false;
                    location.distanceFromLocation = -1;
                }
                foreach(Pathlist pathLocation in pathList)
                {
                    if(location.longitude == pathLocation.longitude && location.latitude == pathLocation.latitude)
                    {
                        location.possibleLocation = false;
                        location.distanceFromLocation = -1;
                    }
                }
            }
            return allLocations;
        }

        public List<Location> FindCloseLocations(List<Location> allLocations)
        {
            for (int counter = 0; counter < 3; counter++)
            {
                double closestValue = 10000000;
                foreach (Location location in allLocations)
                {
                    if (location.closePlace == false && location.possibleLocation == true
                        && location.distanceFromLocation < closestValue
                        && location.distanceFromLocation != 0)
                    {
                        closestValue = location.distanceFromLocation;
                    }
                }
                foreach (Location location in allLocations)
                {
                    if (location.distanceFromLocation == closestValue)
                    {
                        Location closeLocation = allLocations
                                        .Where(possibleLocation => possibleLocation.distanceFromLocation == closestValue)
                                        .FirstOrDefault();
                        closeLocation.closePlace = true;
                    }
                }
            }

            return allLocations;
        }

        public Pathlist FindClosest(List<Location> allLocations, double endLatitude, double endLongitude)
        {
            foreach(Location location in allLocations)
            {
                location.distanceFromLocation = -1;
            }
            foreach (Location location in allLocations)
            {
                if (location.closePlace == true)
                {
                    location.distanceFromLocation = Math.Sqrt((Math.Pow((endLatitude - location.latitude), 2) +
                         Math.Pow((endLongitude - location.longitude), 2)));
                }
            }
            double closestValue = 10000000;
            foreach (Location location in allLocations)
            {
                if (location.closePlace == true && location.distanceFromLocation < closestValue)
                {
                    closestValue = location.distanceFromLocation;
                }
            }
            Location closeLocation = allLocations
                .Where(location => location.distanceFromLocation == closestValue)
                .FirstOrDefault();
            Pathlist pathLocation = new Pathlist();
            pathLocation = ChangeLocationToPathList(closeLocation);

            return pathLocation;
        }
        


        public List<Location> FilterLocations(List<Location> allLocations, Location startLocation, Location endLocation)
        {
            foreach (Location location in allLocations)
            {
                location.possibleLocation = false;
            }
            double minLatitude = Math.Min(startLocation.latitude, endLocation.latitude)-.5;
            double minLongitude = Math.Min(startLocation.longitude, endLocation.longitude)-.5;
            double maxLatitude = Math.Max(startLocation.latitude, endLocation.latitude)+.5;
            double maxLongitude = Math.Max(startLocation.longitude, endLocation.longitude+.5);

            foreach (Location location in allLocations)
            {
                if (location.latitude <= maxLatitude && location.latitude >= minLatitude &&
                    location.longitude <= maxLongitude && location.longitude >= minLongitude)
                {
                    location.possibleLocation = true;
                }
            }

            return allLocations;
        }

        public Pathlist ChangeLocationToPathList(Location location)
        {

            Pathlist pathLocation = new Pathlist();
            pathLocation.Id = Guid.NewGuid();
            pathLocation.latitude = location.latitude;
            pathLocation.longitude = location.longitude;
            pathLocation.name = location.name;

            return pathLocation;
        }

        public async ValueTask<List<Pathlist>> CompileList(Location startLocation, Location endLocation)
        {
            List<Pathlist> pathlists = new List<Pathlist>();

            pathlists.Add(ChangeLocationToPathList(startLocation));
            if (startLocation == endLocation)
            {
                return pathlists;
            }
            List<Location> allLocation = this.locationStorageBroker.SelectAllLocationsAsync().ToList();
            Pathlist checkedLocation = await FindPath(pathlists, allLocation, startLocation, endLocation);
            pathlists.Add(checkedLocation);
            foreach (Location location in allLocation)
            {
                location.closePlace = false;
                location.possibleLocation = false;
            }
            
            Location nextLocation = new Location();
            while (checkedLocation.latitude != endLocation.latitude || checkedLocation.longitude != endLocation.longitude)
            {
                double nextLongitude = checkedLocation.longitude;
                double nextLatitude = checkedLocation.latitude;
                foreach (Location location in allLocation)
                {
                    if (location.longitude == nextLongitude && location.latitude == nextLatitude)
                    {
                        nextLocation = location;
                    }
                }
                checkedLocation = await FindPath(pathlists, allLocation, nextLocation, endLocation);
                pathlists.Add(checkedLocation);
                foreach (Location location in allLocation)
                {
                    location.closePlace = false;
                    location.possibleLocation = false;
                }
            }
            
            return pathlists;
        }
        /*
        public void PrintLocationsToConsole()
        {
            List<Location> allLocations = this.locationStorageBroker.SelectAllLocationsAsync().ToList();

            File.WriteAllLines("locations.txt", allLocations
                .Select(location => $"{{lat: {location.latitude}, lng: {location.longitude} }}").ToArray());
        }*/
    }
}
