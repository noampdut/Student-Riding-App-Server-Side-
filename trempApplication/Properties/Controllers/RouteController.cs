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
        
        /*
        // for client 
        [HttpPost]
        public async Task<ActionResult> CalculateRoute1([FromBody] MapRequest mapRequest)
        {

            DirectionsRequest request = new DirectionsRequest();

            request.Key = "AIzaSyB5po3YPH779Mj38ut1Bc_ULPWEkO9V5pc";

            request.Origin = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Origin));
            request.Destination = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Destination));
            request.WayPoints = mapRequest.Waypoints?.Select(w => new GoogleApi.Entities.Maps.Directions.Request.WayPoint(new LocationEx(new GoogleApi.Entities.Common.Address(w))));
            request.OptimizeWaypoints = true;
            
            var response = await GoogleApi.GoogleMaps.Directions.QueryAsync(request);
            var route = new OptionalRoute
            {
                Distance = Math.Round(response.Routes.First().Legs.Sum(leg => leg.Distance.Value / 1000.0),1),
                Duration = Math.Ceiling(response.Routes.First().Legs.Sum(leg => leg.DurationInTraffic?.Value ?? leg.Duration.Value / 60.0)),
                Instructions = response.Routes.First().Legs.SelectMany(leg => leg.Steps.Select(step => step.HtmlInstructions)).ToList()
            };

            return Ok(route);
        }

        */

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public OptionalRoute CalculateOptionalRoute(string origin, string destination, List<string> waypoints)
        {

            DirectionsRequest request = new DirectionsRequest();

            request.Key = "AIzaSyB5po3YPH779Mj38ut1Bc_ULPWEkO9V5pc";
            request.Origin = new LocationEx(new GoogleApi.Entities.Common.Address(origin));
            request.Destination = new LocationEx(new GoogleApi.Entities.Common.Address(destination));
            request.WayPoints = waypoints?.Select(w => new GoogleApi.Entities.Maps.Directions.Request.WayPoint(new LocationEx(new GoogleApi.Entities.Common.Address(w))));
            request.OptimizeWaypoints = true;

            var response = GoogleApi.GoogleMaps.Directions.Query(request);

            var route = new OptionalRoute
            {
                Distance = Math.Round(response.Routes.First().Legs.Sum(leg => leg.Distance.Value / 1000.0), 1),
                Duration = Math.Ceiling(response.Routes.First().Legs.Sum(leg => leg.DurationInTraffic?.Value ?? leg.Duration.Value / 60.0)),
                Instructions = response.Routes.First().Legs.SelectMany(leg => leg.Steps.Select(step => step.HtmlInstructions)).ToList(),
                Waypoints = waypoints
            };
            return route;

        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private Tuple<OptionalRoute, double> CalculateRelevance(Ride drive, string origin, string destination)
        {
            double originWeight =  0.1;
            double waypointCountWeight = 0.1;
            double driveDurationWeightFactor =0.1;
            double waypointCountWeightFactor = 0.1;
            double relevance = 0;

            List<string> waypoints = new List<string>();

            // calculate distance between the origin and dest of both driver and passenger
            if (CalculateOptionalRoute(drive.Source, origin, waypoints).Distance <= 3)
            {
                relevance += originWeight;
            }
            else
            {
                relevance -= originWeight;
            }

            if (CalculateOptionalRoute(drive.Dest, destination, waypoints).Distance <= 3)
            {
                relevance += originWeight;
            }
            else
            {
                relevance -= originWeight;
            }

            // checking the direction and than add the client as a new wayPoint
            if (drive.ToUniversity)
            {
                waypoints.Add(origin);
            }
            else
            {
                waypoints.Add(destination);
            }

            foreach (var waypoint in drive.Stations)
            { 
                waypoints.Add(waypoint);
            }



             // create a new route base on the client wayPoint
             OptionalRoute newRoute = CalculateOptionalRoute(drive.Source, drive.Dest, waypoints);

            // calculate the time was added to the original drive 
            double addTimeToDrive = newRoute.Duration - drive.Duration;
            if (addTimeToDrive > 7)
            {
                relevance = -1;
                //return new Tuple<OptionalRoute, double>(newRoute, relevance); 
            }
            else
            {
                relevance -= addTimeToDrive * driveDurationWeightFactor;
            }

            
            int totalWaypoints = newRoute.Waypoints.Count;
            int maxWaypointsThreshold = drive.Capacity;

            // Check if the total number of waypoints exceeds the threshold
            if (totalWaypoints > maxWaypointsThreshold)
            {
                relevance = -1;
                //return new Tuple<OptionalRoute, double>(newRoute, relevance); 
            }
            else
            {
                // Reduce the relevance by a factor based on the number of waypoints
                relevance -= totalWaypoints / (waypointCountWeight * waypointCountWeightFactor);
            }

            return new Tuple<OptionalRoute, double>(newRoute, relevance);
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<Ride> FilterRoutes(List<Ride> routes, string userOrigin, string userDestination, double threshold)
        {
            List<Ride> relevantRoutes = new List<Ride>();
            var relevance = 0.0;

            foreach (var route in routes)
            {
                var result = CalculateRelevance(route, userOrigin, userDestination);
                relevance = result.Item2;
                
                if (relevance >= threshold)
                {
                    var newRoute = result.Item1; 
                    route.Duration = newRoute.Duration;
                    route.Stations = newRoute.Waypoints;
                    relevantRoutes.Add(route);
                }

            }
            // need to change to route and order by relevance
            return relevantRoutes;
        }


        [HttpPost]
        public async Task<ActionResult> CalculateRoute([FromBody] MapRequest mapRequest)
        {
            List<Ride> Routes = new List<Ride>();

            var Route1 = new Ride
            {
                Source = "Hasar Moshe 9, Ramat Gan",
                Dest = "Ayalon Mall",
                Duration = 25,
                Capacity = 4,
                ToUniversity = false,
                Stations = new List<string>()
        };

            var Route2 = new Ride
            {
                Source = "Hasar Moshe 9, Ramat Gan",
                Dest = "Gindi Mall, Tel Aviv",
                Duration = 30,
                Capacity = 4,
                ToUniversity = false,
                Stations = new List<string>()
            };

            Routes.Add(Route1);
            Routes.Add(Route2);
            //var relevants = FilterRoutes(Routes, "Uziel 103, Ramat Gan", "Gindi Mall, Tel Aviv", 1.0);
            var relevants = FilterRoutes(Routes, mapRequest.Origin, mapRequest.Destination, 1.0);

            return Ok(relevants);

        }



    }



}


