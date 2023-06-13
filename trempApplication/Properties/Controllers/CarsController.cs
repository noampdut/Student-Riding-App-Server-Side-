using Microsoft.AspNetCore.Mvc;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {

        private ICar _carService;
        private IPassenger _passengerService;

        public CarsController(ICar carService, IPassenger passengerService)
        {
            _carService = carService;
            _passengerService = passengerService;
        }

        // GET: api/<CarsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _carService.GetAllCars();
            if(result.IsSuccess)
            {
                return Ok(result.Car);
            }
            return NotFound(result.ErrorMessage);
        }   

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByOwner(Guid id)
        {
            var result = await _carService.GetCarById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Car);
            }
            return NotFound(result.ErrorMessage);
        }

        // POST api/<CarsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Car car)
        {
            var result = await _carService.AddCar(car);
            if (result.IsSuccess)
            {
                Guid Id = Guid.Parse(result.ErrorMessage);
                var new_car = await _carService.GetCarById(Id);
                var owner = await _passengerService.GetPassengerById(new_car.Car.Owner);
                owner.Passenger.CarIds.Add(Id);
                await _passengerService.UpdatePassenger(owner.Passenger, new_car.Car.Owner);
                return Ok(new_car.Car);
            }
            return BadRequest(result.ErrorMessage);
        }

        // PUT api/<CarsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Car car)
        { 
            var result = await _carService.UpdateCar(car, id);
            if (result.IsSuccess)
            {
                Guid Id = Guid.Parse(result.ErrorMessage);
                var new_car = await _carService.GetCarById(Id);
                return Ok(new_car.Car);
            }
            return BadRequest(result.ErrorMessage);
        }

        
        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _carService.DeleteCar(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
        
    }
}
