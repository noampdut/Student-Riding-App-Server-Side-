using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IRide
    {
        Task<List<Ride>> GetAllRides();
        Task<Ride> GetRideById(Guid id);
        Task AddRide(Ride ride);
        Task UpdateRide(Ride ride, Guid id);
        Task DeleteRide(Guid id);
    }
}
