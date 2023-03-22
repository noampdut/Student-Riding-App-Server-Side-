using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IPassenger
    {
        Task<(bool IsSuccess, List<Passenger> Passenger, string ErrorMessage)> GetAllPassengers();
        Task<(bool IsSuccess, Passenger Passenger, string ErrorMessage)> GetPassengerById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddPassenger(Passenger passenger);
        Task<(bool IsSuccess, string ErrorMessage)> UpdatePassenger(Passenger passenger, Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> DeletePassenger(Guid id);

    }
}
