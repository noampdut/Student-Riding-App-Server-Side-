using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using static GoogleApi.GoogleMaps;
using GoogleApi.Entities.Maps.Directions.Request;
using Google.Type;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Entities.Common;
using GoogleMapsAPI.NET.API;
using GoogleApi.Entities.Maps.Common;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult> CalculateRoute([FromBody] MapRequest mapRequest)
        {

            DirectionsRequest request = new DirectionsRequest();

            request.Key = "AIzaSyB5po3YPH779Mj38ut1Bc_ULPWEkO9V5pc";

            request.Origin = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Origin));
            request.Destination = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Destination));
            request.WayPoints = mapRequest.Waypoints?.Select(w => new GoogleApi.Entities.Maps.Directions.Request.WayPoint(new LocationEx(new GoogleApi.Entities.Common.Address(w))));
            request.OptimizeWaypoints = true;
            
            var response = await GoogleApi.GoogleMaps.Directions.QueryAsync(request);
            var route = new RouteResponse
            {
                Distance = Math.Round(response.Routes.First().Legs.Sum(leg => leg.Distance.Value / 1000.0),1),
                Duration = Math.Ceiling(response.Routes.First().Legs.Sum(leg => leg.DurationInTraffic?.Value ?? leg.Duration.Value / 60.0)),
                Instructions = response.Routes.First().Legs.SelectMany(leg => leg.Steps.Select(step => step.HtmlInstructions)).ToList()
            };

            return Ok(route);
        }

    }


    public class RouteResponse
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public List<string> Instructions { get; set; } // list of step-by-step instructions
    }
}

