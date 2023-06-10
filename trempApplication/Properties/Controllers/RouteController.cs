using Microsoft.AspNetCore.Mvc;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Entities.Maps.Common;
using trempApplication.Properties.Models;
using trempApplication.Properties.Interfaces;
using GoogleApi.Entities.Maps.Common.Enums;
using System.Diagnostics;


namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {


        private IRide _rideService;
        private IPassenger _passengerService;

        public RouteController(IRide rideService, IPassenger passengerService)
        {
            _rideService = rideService;
            _passengerService = passengerService;
        }




        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public System.DateTime ConvertToDate(Models.Date date1)
        {
            System.DateTime dateTime = new System.DateTime(Convert.ToInt32(date1.Year), Convert.ToInt32(date1.Month), Convert.ToInt32(date1.Day),
            Convert.ToInt32(date1.Hour), Convert.ToInt32(date1.Minute), 0);
            return dateTime;
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<Ride>> GetPotentialRides(Models.Date uDate, bool toUniversity, Guid client_id)
        {
            var result = await _rideService.GetPotentialRides(uDate, toUniversity, client_id);
            if (result.IsSuccess)
            {
                return result.Rides;
            }
            return null;
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public OptionalRoute CalculateOptionalRoute([FromQuery] string origin, [FromQuery] string destination, [FromQuery] List<string> waypoints, [FromQuery] Models.Date date)
        {

            DirectionsRequest request = new DirectionsRequest();

            request.Key = "AIzaSyB5po3YPH779Mj38ut1Bc_ULPWEkO9V5pc";
            request.Origin = new LocationEx(new GoogleApi.Entities.Common.Address(origin));
            request.Destination = new LocationEx(new GoogleApi.Entities.Common.Address(destination));
            request.WayPoints = waypoints?.Select(w => new GoogleApi.Entities.Maps.Directions.Request.WayPoint(new LocationEx(new GoogleApi.Entities.Common.Address(w))));
            request.OptimizeWaypoints = true;
            request.TravelMode = TravelMode.Driving;
            request.DepartureTime = ConvertToDate(date);

            var response = GoogleApi.GoogleMaps.Directions.Query(request);

            var route = new OptionalRoute
            {
                Distance = Math.Round(response.Routes.First().Legs.Sum(leg => leg.Distance.Value / 1000.0), 1),
                Duration = Math.Ceiling(response.Routes.First().Legs.Sum(leg => leg.DurationInTraffic?.Value ?? leg.Duration.Value / 60.0)),
                Instructions = response.Routes.First().Legs.SelectMany(leg => leg.Steps.Select(step => step.HtmlInstructions)).ToList(),
                Legs = response.Routes.First().Legs.ToList(),
                //  pickUpPoints = NEED TO TAKE AS AN ARG TO FUNCTION AND UPDATE HERE, FOR THE PASSENGER ID AND THE TIMEOUT POINT
            };
            // Retrieve the pick-up times for each leg
            var pickUpTimes = CalculatePickUpTimes(response.Routes.First().Legs.ToList(), route.Duration, ConvertToDate(date));
            List<PickUpPoint> PickUpPoints = new List<PickUpPoint>();
            foreach (var leg in route.Legs)
            {
                if (leg == route.Legs.Last())
                {
                    continue;
                }
                var waypoint = leg.EndAddress;
                //var convert_waypoint = new LocationEx(new GoogleApi.Entities.Common.Address(waypoint));
                var arrivalTime = GetPickUpTimeByWayPoint(response.Routes.First().Legs.ToList(), waypoint, pickUpTimes);
                // all pickup times update 
                var pickUpPoint = new PickUpPoint
                {
                    Time = arrivalTime,
                    Address = waypoint
                };

                PickUpPoints.Add(pickUpPoint);
            }

            route.pickUpPoints = PickUpPoints;

            return route;

        }


        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private Tuple<OptionalRoute, double> CalculateRelevance(Ride drive, string origin, string destination)
        {
            double originWeight = 1;
            double driveDurationWeightFactor = 0.5;
            double waypointCountWeightFactor = 3;
            double relevance = 50;

            List<string> waypoints = new List<string>();

            var distance = CalculateOptionalRoute(drive.Source, origin, waypoints, drive.Date).Distance;
            // calculate distance between the origin and dest of both driver and passenger
            if (distance <= 3)
            {
                relevance += distance * originWeight;
            }
            else
            {
                relevance -= distance * originWeight;
            }

            distance = CalculateOptionalRoute(drive.Dest, destination, waypoints, drive.Date).Distance;

            if (distance <= 3)
            {
                relevance += distance * originWeight;
            }
            else
            {
                relevance -= distance * originWeight;
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

            foreach (var pickUpPoint in drive.pickUpPoints)
            {
                var waypoint = pickUpPoint.Address;
                waypoints.Add(waypoint);
            }



            // create a new route base on the client wayPoint
            OptionalRoute newRoute = CalculateOptionalRoute(drive.Source, drive.Dest, waypoints, drive.Date);

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


            int totalWaypoints = newRoute.pickUpPoints.Count;

            // Check if the total number of waypoints exceeds the threshold
            if (drive.Capacity <= 0)
            {
                relevance = -50;
            }
            else
            {
                // Reduce the relevance by a factor based on the number of waypoints
                relevance -= totalWaypoints * (waypointCountWeightFactor);
            }

            return new Tuple<OptionalRoute, double>(newRoute, relevance);
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<SuggestedRide> FilterRoutes(List<Ride> routes, string userOrigin, string userDestination, double threshold)
        {
            List<SuggestedRide> relevantRoutes = new List<SuggestedRide>();
            var relevance = 0.0;

            if (routes == null)
            {
                return relevantRoutes;
            }

            foreach (var route in routes)
            {
                var result = CalculateRelevance(route, userOrigin, userDestination);
                relevance = result.Item2;

                if (relevance >= threshold)
                {
                    var newRoute = result.Item1; // optinal route
                    // build a suggested ride for the server 
                    var suggestedRide = new SuggestedRide
                    {
                        Distance = newRoute.Distance, // updated
                        Duration = newRoute.Duration, // updated
                        Instructions = newRoute.Instructions, // updated
                        pickUpPoints = newRoute.pickUpPoints,

                        RideId = route.Id, // old ride- if we get an approval, we will update this  
                        Driver = _passengerService.GetPassengerById(route.DriverId).Result.Passenger,
                        Relevance = result.Item2,
                        Capacity = route.Capacity
                    };

                    // update the pickup time of the client in suggestedRide object
                    foreach (var pickUpPoint in newRoute.pickUpPoints)
                    {
                        var flag = 0;
                        foreach (var Point in route.pickUpPoints)
                        {
                            // looking for the new address of the new client 
                            if (pickUpPoint.Address == Point.Address)
                            {
                                pickUpPoint.PassengerId = Point.PassengerId;
                                flag = 1;
                                continue;
                            }
                        }
                        // This is the new address
                        if (flag == 0)
                        {
                            string pickUpTime = pickUpPoint.Time;
                            suggestedRide.PickUpTime = pickUpTime;
                            suggestedRide.PickUpPoint = pickUpPoint.Address;
                            pickUpPoint.PassengerId = "Unknown Yet";

                        }
                    }
                    relevantRoutes.Add(suggestedRide);
                }
            }
            relevantRoutes = relevantRoutes.OrderByDescending(r => r.Relevance).ToList();
            return relevantRoutes;
        }



        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private string GetPickUpTimeByWayPoint(List<Leg> legs, string waypointAddress, List<string> pickUpTimes)
        {
            foreach (var leg in legs)
            {
                if (leg.EndAddress.Equals(waypointAddress))
                {
                    return pickUpTimes[legs.IndexOf(leg)];
                }
            }

            return "Unknown";
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private List<string> CalculatePickUpTimes(List<Leg> legs, double routeDuration, System.DateTime departureTime)
        {
            var pickUpTimes = new List<string>();
            double accumulatedDuration = 0;

            foreach (var leg in legs)
            {
                var estimatedArrivalTime = departureTime.AddSeconds(accumulatedDuration + (leg.Duration?.Value ?? 0));
                pickUpTimes.Add(estimatedArrivalTime.ToString("HH:mm"));

                accumulatedDuration += leg.Duration?.Value ?? 0;
            }

            return pickUpTimes;
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private static double CheckingSimilarity(string string1, string string2)
        {
            string pythonScript = "bert.py";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{pythonScript} \"{string1}\" \"{string2}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    double similarity;
                    if (double.TryParse(output, out similarity))
                    {
                        return similarity;
                    }
                }
                return 0.0; 
            }
        }

    [HttpPost]
        public async Task<IActionResult> CalculateRoute([FromBody] MapRequest mapRequest)
        {
            string client_id = mapRequest.PassengerId; // like 3152....
            var client = _passengerService.GetPassengerByIdNumber(client_id).Result.Passenger;

            var routes = GetPotentialRides(mapRequest.Date, mapRequest.ToUniversity,client.Id).Result;
          
            var relevants = FilterRoutes(routes, mapRequest.Origin, mapRequest.Destination, 30.0);

            string client_bio = client.Bio;
            foreach (var suggestedRide in relevants)
            {
                
                string driver_bio = suggestedRide.Driver.Bio;
                double decimalValue = CheckingSimilarity(driver_bio, client_bio);
                string percentage = (decimalValue * 100).ToString("0") + "%";
                suggestedRide.Similarity = percentage;
            }

            // return suggested 
            return Ok(relevants);

        }
    }
}


