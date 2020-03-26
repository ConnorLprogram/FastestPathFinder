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
        
        public async ValueTask<List<Pathlist>> CompileList(Location startLocation, Location endLocation)
        {
            List<Pathlist> pathlists = new List<Pathlist>();


            pathlists.Add(ChangeLocationToPathList(startLocation));
            if (startLocation == endLocation)
            {
                return pathlists;
            }
            List<Location> allLocation = this.locationStorageBroker.SelectAllLocationsAsync().ToList();
            Pathlist checkedLocation = FindPath(pathlists, allLocation, startLocation, endLocation);
            pathlists.Add(checkedLocation);
            resetLocations(allLocation);

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
                checkedLocation = FindPath(pathlists, allLocation, nextLocation, endLocation);
                pathlists.Add(checkedLocation);
                resetLocations(allLocation);
            }

            return pathlists;
        }

        public Pathlist FindPath(List<Pathlist> pathList, List<Location> allLocations, Location startLocation, Location endlocation)
        {
            FilterLocations(allLocations, startLocation, endlocation);
            FindPossibleLocations(pathList, allLocations, startLocation);
            FindCloseLocations(allLocations);
            Pathlist nextLocation = FindClosest(allLocations, endlocation);

            return nextLocation;
        }

        public List<Location> FilterLocations(List<Location> allLocations, Location startLocation, Location endLocation)
        {
            foreach (Location location in allLocations)
            {
                location.possibleLocation = false;
            }
            double minLatitude = Math.Min(startLocation.latitude, endLocation.latitude) - .5;
            double minLongitude = Math.Min(startLocation.longitude, endLocation.longitude) - .5;
            double maxLatitude = Math.Max(startLocation.latitude, endLocation.latitude) + .5;
            double maxLongitude = Math.Max(startLocation.longitude, endLocation.longitude) + .5;

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
        
        public List<Location> FindPossibleLocations(List<Pathlist> pathList, List<Location> allLocations, Location startLocation)
        {
            foreach (Location location in allLocations)
            {
                if (location.possibleLocation == true)
                {
                    location.distanceFromLocation = calculateDistance(location, startLocation);
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
            for (int counter = 0; counter < 5; counter++)
            {
                double closestValue = double.MaxValue;
                foreach (Location location in allLocations)
                {
                    if (location.closePlace == false && location.possibleLocation == true
                        && location.distanceFromLocation < closestValue
                        && location.distanceFromLocation > 0)
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

        public Pathlist FindClosest(List<Location> allLocations, Location endLocation)
        {
            foreach(Location location in allLocations)
            {
                location.distanceFromLocation = -1;
            }
            foreach (Location location in allLocations)
            {
                if (location.closePlace == true)
                {
                    location.distanceFromLocation = calculateDistance(location, endLocation);
                }
            }
            double closestValue = double.MaxValue;
            foreach (Location location in allLocations)
            {
                if (location.closePlace == true && location.distanceFromLocation < closestValue && location.distanceFromLocation != -1)
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

        public async ValueTask<List<Pathlist>> CompileAlternateList(Location startLocation, Location endLocation, int alternateNum)
        {
            List<Pathlist> pathlists = new List<Pathlist>();


            pathlists.Add(ChangeLocationToPathList(startLocation));
            if (startLocation == endLocation)
            {
                return pathlists;
            }
            List<Location> allLocation = this.locationStorageBroker.SelectAllLocationsAsync().ToList();
            Pathlist checkedLocation = FindAlternatePath(pathlists, allLocation, startLocation, endLocation, alternateNum);
            pathlists.Add(checkedLocation);
            resetLocations(allLocation);

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
                checkedLocation = FindAlternatePath(pathlists, allLocation, nextLocation, endLocation, alternateNum);
                pathlists.Add(checkedLocation);
                resetLocations(allLocation);
            }

            return pathlists;
        }
        public Pathlist FindAlternatePath(List<Pathlist> pathList, List<Location> allLocations, Location startLocation, Location endlocation, int alternateNum)
        {
            FilterLocations(allLocations, startLocation, endlocation);
            FindPossibleLocations(pathList, allLocations, startLocation);
            FindCloseLocations(allLocations);
            Pathlist nextLocation = FindAlternate(allLocations, endlocation, alternateNum);

            return nextLocation;
        }
        public Pathlist FindAlternate(List<Location> allLocations, Location endLocation, int alternateNum)
        {
            foreach (Location location in allLocations)
            {
                location.distanceFromLocation = -1;
            }
            foreach (Location location in allLocations)
            {
                if (location.closePlace == true)
                {
                    location.distanceFromLocation = calculateDistance(location, endLocation);
                }
            }
            double closestValue = double.MaxValue;
            for (int townNum = 0; townNum < alternateNum; townNum++)
            {
                closestValue = double.MaxValue;
                foreach (Location location in allLocations)
                {
                    if (location.possibleLocation = true && location.closePlace == true &&
                        location.distanceFromLocation < closestValue && location.distanceFromLocation != -1)
                    {
                        closestValue = location.distanceFromLocation;
                    }
                }
                Location notAlternateLocation = allLocations
                .Where(location => location.distanceFromLocation == closestValue)
                .FirstOrDefault();
                if (notAlternateLocation == endLocation && townNum == 0)
                {
                    Pathlist endingLocation = ChangeLocationToPathList(endLocation);
                    return endingLocation;
                }
                notAlternateLocation.closePlace = false;
            }
            closestValue = double.MaxValue;
            foreach (Location location in allLocations)
            {
                if (location.possibleLocation = true && location.closePlace == true &&
                    location.distanceFromLocation < closestValue && location.distanceFromLocation != -1)
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

        public Pathlist ChangeLocationToPathList(Location location)
        {

            Pathlist pathLocation = new Pathlist();
            pathLocation.Id = Guid.NewGuid();
            pathLocation.latitude = location.latitude;
            pathLocation.longitude = location.longitude;
            pathLocation.name = location.name;

            return pathLocation;
        }

        public List<Location> resetLocations(List<Location> allLocations)
        {
            foreach (Location location in allLocations)
            {
                location.closePlace = false;
                location.possibleLocation = false;
            }
            return allLocations;
        }

        public double calculateDistance(Location firstLocation, Location secondLocation)
        {
            double distance = Math.Sqrt((Math.Pow((firstLocation.latitude - secondLocation.latitude), 2) +
                        Math.Pow((firstLocation.longitude - secondLocation.longitude), 2)));

            return distance;
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


        /*public object PrintLocationsToConsole()
        {
            List<Location> allLocations = this.locationStorageBroker.SelectAllLocationsAsync().ToList();

            File.WriteAllLines("locations.txt", allLocations
                .Select(location => $"{{lat: {location.latitude}, lng: {location.longitude} }}").ToArray());
            return null;
        }*/
    }
}
