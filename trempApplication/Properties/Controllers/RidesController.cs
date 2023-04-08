using Microsoft.AspNetCore.Mvc;
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

        public RidesController(IRide rideService)
        {
            _rideService = rideService;
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

        // POST api/<RidesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ride ride)
        {
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
        public async Task<IActionResult> Put(Guid id, [FromBody] Ride ride)
        {
            var result = await _rideService.UpdateRide(ride, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        // DELETE api/<RidesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rideService.DeleteRide(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
