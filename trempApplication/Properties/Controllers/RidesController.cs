using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Directions.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RidesController : ControllerBase
    {
        private IRide _rideService;
        private IPassenger _passengerService;
        private INotificationService _notificationService;

        public RidesController(IRide rideService, IPassenger passengerService, INotificationService notificationService)
        {
            _rideService = rideService;
            _passengerService = passengerService;
            _notificationService = notificationService;
        }

        // GET: api/<RidesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _rideService.GetAllRides();
            if (result.IsSuccess)
            {
                return Ok(result.Ride);
            }
            return NotFound(result.ErrorMessage);
        }

        // GET api/<RidesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _rideService.GetRideById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Ride);
            }
            return NotFound(result.ErrorMessage);
        }

        // GET api/<RidesController>
        [HttpGet("{id}/{getActiveRides}")]
        public async Task<IActionResult> GetRides(string id, bool getActiveRides)
        {
            var result = await _rideService.GetActiveOrHistoryRides(id, getActiveRides);
            if (result.IsSuccess)
            {
                return Ok(result.Rides);
            }
            return NotFound(result.ErrorMessage);
        }

        // POST api/<RidesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ride ride)
        {
            List<string> mapR_waypoints = new List<string>();
            var mapRequest = new MapRequest
            {
                Origin = ride.Source,
                Destination = ride.Dest,
                Date = ride.Date,
                ToUniversity = ride.ToUniversity
            };

            foreach (var pickUpPoint in ride.pickUpPoints)
            {
                string waypoint = pickUpPoint.Address;
                mapR_waypoints.Add(waypoint);
            }
            mapRequest.Waypoints = mapR_waypoints;

            ride.Duration = CalculateRoute(mapRequest).Result;
            var result = await _rideService.AddRide(ride);
            if (result.IsSuccess)
            {
                Guid Id = Guid.Parse(result.ErrorMessage);
                var new_ride = await _rideService.GetRideById(Id);
                return Ok(new_ride.Ride);
            }
            return BadRequest(result.ErrorMessage);

        }

        // PUT api/<RidesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] SuggestedRide suggestedRide)
        {
            var result_old_ride = await _rideService.GetRideById(suggestedRide.RideId);
            var old_ride = result_old_ride.Ride;

            // Make sure the passenger is not alreay signed - Do not let a passenger to reserve double sits 
            foreach (var point in old_ride.pickUpPoints)
            {
                if (point.PassengerId == id)
                {
                    return Ok(2); // passenger is alreay signed 
                }
            }
            var new_ride = new Ride
            {
                Id = old_ride.Id,
                Driver = old_ride.Driver,
                CarId = old_ride.CarId,
                Capacity = old_ride.Capacity - 1,
                Source = old_ride.Source,
                Dest = old_ride.Dest,
                ToUniversity = old_ride.ToUniversity,
                Date = old_ride.Date,
                pickUpPoints = suggestedRide.pickUpPoints,
                Duration = suggestedRide.Duration
            };

            foreach (var pickPoint in new_ride.pickUpPoints)
            {

                if (pickPoint.PassengerId == "Unknown Yet")
                {
                    pickPoint.PassengerId = id;
                    NotificationNewPassenger(new_ride, id, pickPoint);
                    break;
                }

            }
            var result = await _rideService.UpdateRide(new_ride, new_ride.Id);
            if (result.IsSuccess)
            {
                return Ok(0); // success
            }
            return Ok(1); // cant update
        }

        // DELETE api/<RidesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rideService.DeleteRide(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.ErrorMessage);
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<double> CalculateRoute([FromBody] MapRequest mapRequest)
        {

            DirectionsRequest request = new DirectionsRequest();

            request.Key = "AIzaSyB5po3YPH779Mj38ut1Bc_ULPWEkO9V5pc";

            request.Origin = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Origin));
            request.Destination = new LocationEx(new GoogleApi.Entities.Common.Address(mapRequest.Destination));
            request.WayPoints = mapRequest.Waypoints?.Select(w => new GoogleApi.Entities.Maps.Directions.Request.WayPoint(new LocationEx(new GoogleApi.Entities.Common.Address(w))));
            request.OptimizeWaypoints = true;

            var response = await GoogleApi.GoogleMaps.Directions.QueryAsync(request);

            var duration = Math.Ceiling(response.Routes.First().Legs.Sum(leg => leg.DurationInTraffic?.Value ?? leg.Duration.Value / 60.0));

            return duration;
        }


        // PUT api/<RidesController>/5
        [HttpPut("{id}/{driveId}")]
        public async Task<IActionResult> DeleteFromRide(string id, Guid driveId)
        {
            var person = await _passengerService.GetPassengerByIdNumber(id);
            var ride = await _rideService.GetRideById(driveId);

            if (ride.Ride.Driver.IdNumber == id)
            {
                NotificationCanceledDrive(ride.Ride);
                await _rideService.DeleteRide(driveId);
                return Ok();
            }
            else
            {
                
                foreach (var point in ride.Ride.pickUpPoints)
                {
                    if (point.PassengerId == id)
                    {
                        ride.Ride.pickUpPoints.Remove(point);
                        ride.Ride.Capacity +=1;
                        await _rideService.UpdateRide(ride.Ride, driveId);
                        NotificationPassengerCanceled(id, ride.Ride);
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async void NotificationCanceledDrive(Ride ride)
        {
            foreach(var point in ride.pickUpPoints)
            {
                var person = await _passengerService.GetPassengerByIdNumber(point.PassengerId);
                var token = person.Passenger.Token;
                string title = "Drive was cancelled";
                string body = "The drive on " + ride.Date.Day +"/" + ride.Date.Month+ " at " +ride.Date.Hour+ ":" +ride.Date.Minute + " to " + ride.Dest + " has been cancelled";
                await _notificationService.sendNotification(token, title, body);
            }
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async void NotificationPassengerCanceled(string pass_id, Ride ride)
        {
            var person = await _passengerService.GetPassengerByIdNumber(pass_id);
            string title = "Passenger canceled participation in your drive";
            string body = person.Passenger.UserName + " canceled participation in your drive" + " on the " + ride.Date.Day + "/" + ride.Date.Month + " at " + ride.Date.Hour + ":" + ride.Date.Minute;
            await _notificationService.sendNotification(ride.Driver.Token, title, body);
        }

        [Route("internal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async void NotificationNewPassenger(Ride ride, string pass_id, PickUpPoint point)
        {
            var person = await _passengerService.GetPassengerByIdNumber(pass_id);
            string title = person.Passenger.UserName + " joined your drive";
            string body = person.Passenger.UserName + " joined your drive" + " on the " + ride.Date.Day + "/" + ride.Date.Month + " to " + ride.Dest + ".\tPickup address: " + point.Address + " at " + point.Time;
            await _notificationService.sendNotification(ride.Driver.Token, title, body);
        }
    }
}

