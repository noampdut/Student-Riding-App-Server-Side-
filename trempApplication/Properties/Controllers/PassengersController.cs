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
        public async Task<IActionResult> Get()
        {
            var result = await _passengerService.GetAllPassengers();
            if (result.IsSuccess)
            {
                return Ok(result.Passenger);
            }
            return NotFound(result.ErrorMessage);
        }

        // GET api/<PassengersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _passengerService.GetPassengerById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Passenger);
            }
            return NotFound(result.ErrorMessage);
        }
        
        [HttpPut]
        public async Task<IActionResult> GetPassenergs(List<string> ids)
        {
            List<Passenger> passengers = new List<Passenger>();
            
            foreach (var id in ids)
            {
                var result = await _passengerService.GetPassengerByIdNumber(id);
                passengers.Add(result.Passenger);
            }
            
            return Ok(passengers);
        }

        // POST api/<PassengersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Passenger passenger)
        {
            var result = await _passengerService.AddPassenger(passenger);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        // PUT api/<PassengersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Passenger passenger)
        {
            var result = await _passengerService.UpdatePassenger(passenger, id);
            if (result.IsSuccess)
            {
                return Ok(result.Passenger);
            }
            return BadRequest(result.ErrorMessage);
        }

        // DELETE api/<PassengersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _passengerService.DeletePassenger(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
