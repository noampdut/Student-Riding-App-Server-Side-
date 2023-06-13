using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IRide
    {
        Task<(bool IsSuccess, List<Ride> Ride, string ErrorMessage)> GetAllRides();
        Task<(bool IsSuccess, Ride Ride, string ErrorMessage)> GetRideById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddRide(Ride ride);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateRide(Ride ride, Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteRide(Guid id);
        Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetPotentialRides(Date uDate, bool ToUniversity, Guid client_id);
        Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetActiveOrHistoryRides(string id, bool getActive);

        Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetRidesByPassenger(string id);

    }
}
