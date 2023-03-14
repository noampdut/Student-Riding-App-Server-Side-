using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IPassenger
    {
        Task<List<Passenger>> GetAllPassengers();
        Task<Passenger> GetPassengerById(Guid id);
        Task AddPassenger(Passenger passenger);
        Task UpdatePassenger(Passenger passenger, Guid id);
        Task DeletePassenger(Guid id);

    }
}
