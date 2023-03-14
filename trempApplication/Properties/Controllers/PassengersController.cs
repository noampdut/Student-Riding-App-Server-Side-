using Microsoft.AspNetCore.Mvc;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengersController : ControllerBase
    {

        private IPassenger _passengerService;

        public PassengersController(IPassenger passengerService)
        {
            _passengerService = passengerService;
        }

        // GET: api/<PassengersController>
        [HttpGet]
        public async Task<IEnumerable<Passenger>> Get()
        {
            return await _passengerService.GetAllPassengers();
        }

        // GET api/<PassengersController>/5
        [HttpGet("{id}")]
        public async Task<Passenger> Get(Guid id)
        {
            return await _passengerService.GetPassengerById(id);
        }

        // POST api/<PassengersController>
        [HttpPost]
        public async Task Post([FromBody] Passenger passenger)
        {
            await _passengerService.AddPassenger(passenger);
        }

        // PUT api/<PassengersController>/5
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] Passenger passenger)
        {
            await _passengerService.UpdatePassenger(passenger, id);
        }

        // DELETE api/<PassengersController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _passengerService.DeletePassenger(id);
        }
    }
}
