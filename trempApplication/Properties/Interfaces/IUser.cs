using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IUser
    {

        //Task<(bool IsSuccess, List<User> User, string ErrorMessage)> GetAllUsers();
        Task<(bool IsSuccess, Passenger passenger, string ErrorMessage)> GetUserById(string IdNumber, string password);
        Task<(bool IsSuccess, string ErrorMessage)> AddUser(User user);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateUser(User user);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteUser(string IdNumber);
    }
}
