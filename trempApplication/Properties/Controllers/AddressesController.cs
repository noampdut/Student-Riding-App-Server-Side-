using Microsoft.AspNetCore.Mvc;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {

        private IAddress _addressService;

        public AddressesController(IAddress addressService)
        {
            _addressService = addressService;
        }

        // GET: api/<AddressesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _addressService.GetAllAddresses();
            if (result.IsSuccess)
            {
                return Ok(result.Address);
            }
            return NotFound(result.ErrorMessage);
        }

        // GET api/<AddressesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _addressService.GetAddressById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Address);
            }
            return NotFound(result.ErrorMessage);
        }

        // POST api/<AddressesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Address address)
        {
            var result = await _addressService.AddAddress(address);
            if (result.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest(result.ErrorMessage);
        }

        // PUT api/<AddressesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Address address)
        {
            var result = await _addressService.UpdateAddress(address, id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        // DELETE api/<AddressesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _addressService.DeleteAddress(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
