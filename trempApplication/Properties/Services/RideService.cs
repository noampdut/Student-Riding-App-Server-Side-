using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class RideService : IRide
    {
        private IMongoCollection<Ride> ridesCollection;
        public RideService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            ridesCollection = mongoDB.GetCollection<Ride>("Rides");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRide(Ride ride)
        {
            try
            {
                if (ride == null)
                {
                    return (false, "ride is null object");
                }

                await ridesCollection.InsertOneAsync(ride);
                return (true,ride.Id.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding ride to database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteRide(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                var ride = await ridesCollection.DeleteOneAsync(filter);
                if (ride == null)
                {
                    return (false, "No ride was found to be deleted");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting ride from database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Ride> Ride, string ErrorMessage)> GetAllRides()
        {
            try
            {
                var rides = await ridesCollection.Find(u => true).ToListAsync();
                if (rides != null)
                {
                    return (true, rides, null);
                }
                return (false, null, "No rides found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all rides from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Ride Ride, string ErrorMessage)> GetRideById(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                var ride = await ridesCollection.Find(filter).FirstOrDefaultAsync();
                if (ride != null)
                {
                    return (true, ride, null);
                }
                return (false, null, "No ride found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting ride by id from database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateRide(Ride ride, Guid id)
        {
            try
            {
                if (ride == null)
                {
                    return (false, "The ride object is null.");
                }

                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                await ridesCollection.ReplaceOneAsync(filter, ride);
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ride in database", ex);
            }
        }

    }
}
