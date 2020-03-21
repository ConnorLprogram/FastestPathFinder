using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PathFinder.Models;
using PathFinder.Services;


namespace PathFinder.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService locationService;

        public LocationController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        [HttpPost]
        public async ValueTask<Location> AddNewLocationAsync([FromBody] Location location)
        {
            Location storageLocation =
                await this.locationService.AddNewLocationAsync(location);

            return storageLocation;
        }

        [HttpGet]
        public IQueryable<Location> GiveAllLocations()
        {
            return this.locationService.GiveLocations();
        }

        [HttpGet("names/{locationName}")]
        public IQueryable<Location> SearchForLocation(string locationName)
        {
            return this.locationService.SearchLocation(locationName);
        }

        [HttpGet("{startName}&{endName}")]
        public async ValueTask<List<Pathlist>> FindThePath(string startName, string endName)
        {
            Location startLocation = this.locationService.FindLocation(startName);
            Location endLocation = this.locationService.FindLocation(endName);
            List<Pathlist> compiledList = await this.locationService.CompileList(startLocation, endLocation);

            return compiledList;
        }
        /*
        [HttpGet("converToTxt")]
        public void convertTxt()
        {
            locationService.PrintLocationsToConsole();
        } */
    }
}