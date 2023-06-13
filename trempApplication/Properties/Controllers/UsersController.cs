using Microsoft.AspNetCore.Mvc;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace trempApplication.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private IUser _userService;
        private IPassenger _passengerService;
        private INotificationService _notificationService;

        public UsersController(IUser userService, IPassenger passengerService, INotificationService notificationService)
        {
            _userService = userService;
            _passengerService = passengerService;
            _notificationService = notificationService;
        }

        // POST api/<UsersController>/5
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _userService.GetUserById(user.IdNumber, user.Password);
            if (result.IsSuccess)
            {
                await _notificationService.sendNotification("cZiJvNx9RgyJESOXXGFcjG:APA91bGZzsoCTH68GgRO-prVq7d12FR1Ow2m1U4F5VfF_8TrGVZl2hmAPtdmg_zAi41N9Bfoueu_Z3WHhPmBSdO4Ci5ioPo0xHsHLExRMyLuuemSnOskuB7hDOVRF8YnOkHOi73EDJp5");
                return Ok(result.passenger);
            }
            return NotFound(result.ErrorMessage);
        }

        // POST api/<UsersController>
        [HttpPost("{IdNumber}/{UserName}/{Faculty}/{PhoneNumber}")]
        public async Task<IActionResult> Register(string IdNumber, string UserName, string Faculty, string PhoneNumber, [FromBody] string password)
        {
            var user = new User
            {
                IdNumber = IdNumber,
                Password = password 
            };
            var passenger = new Passenger
            {
                IdNumber = IdNumber,
                UserName = UserName,
                Faculty = Faculty,
                PhoneNumber = PhoneNumber,
                CarIds = new List<Guid> { },
                Bio = ""
            };
            var result_user = await _userService.AddUser(user);
            var result_passenger = await _passengerService.AddPassenger(passenger);
            // changed
            if (result_user.IsSuccess && result_passenger.IsSuccess)
            {
                var new_passenger = await _passengerService.GetPassengerByIdNumber(user.IdNumber);
                return Ok(new_passenger.Passenger);
            }
            return BadRequest(result_passenger.ErrorMessage);

        }

        // PUT api/<UsersController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] User user)
        {
            var result = await _userService.UpdateUser(user);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{IdNumber}")]
        public async Task<IActionResult> Delete(string IdNumber)
        {
            var result = await _userService.DeleteUser(IdNumber);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
