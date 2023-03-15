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
        public async Task<IEnumerable<Address>> Get()
        {
            return await _addressService.GetAllAddresses();
        }

        // GET api/<AddressesController>/5
        [HttpGet("{id}")]
        public async Task<Address> Get(Guid id)
        {
            return await _addressService.GetAddressById(id);
        }

        // POST api/<AddressesController>
        [HttpPost]
        public async Task Post([FromBody] Address address)
        {
            await _addressService.AddAddress(address);
        }

        // PUT api/<AddressesController>/5
        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] Address address)
        {
            await _addressService.UpdateAddress(address, id);
        }

        // DELETE api/<AddressesController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _addressService.DeleteAddress(id);
        }
    }
}
