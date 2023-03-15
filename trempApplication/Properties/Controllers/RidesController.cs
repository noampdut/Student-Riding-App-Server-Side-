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
        public async Task<IEnumerable<Ride>> Get()
        {
            return await _rideService.GetAllRides();
        }

        // GET api/<RidesController>/5
        [HttpGet("{id}")]
        public async Task<Ride> Get(Guid id)
        {
            return await _rideService.GetRideById(id);
        }

        // POST api/<RidesController>
        [HttpPost]
        public async Task Post([FromBody] Ride ride)
        {
            await _rideService.AddRide(ride);
        }

        // PUT api/<RidesController>/5
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] Ride ride)
        {
            await _rideService.UpdateRide(ride, id);
        }

        // DELETE api/<RidesController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _rideService.DeleteRide(id);
        }
    }
}
