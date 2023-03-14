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

        public CarsController(ICar carService)
        {
            _carService = carService;
        }
        // GET: api/<CarsController>
        [HttpGet]
        public async Task<IEnumerable<Car>> Get()
        {
            return await _carService.GetAllCars();
        }

        // GET api/<CarsController>/5
        [HttpGet("{id}")]
        public async Task<Car> Get(Guid id)
        {
            return await _carService.GetCarById(id);
        }

        // POST api/<CarsController>
        [HttpPost]
        public async Task Post([FromBody] Car car)
        {
            await _carService.AddCar(car);
        }

        // PUT api/<CarsController>/5
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] Car car)
        { 
            await _carService.UpdateCar(car, id);
        }


            // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _carService.DeleteCar(id);
        }
    }
}
