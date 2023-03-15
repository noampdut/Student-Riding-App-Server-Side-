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

        public async Task AddRide(Ride ride)
        {
            try
            {
                if (ride == null)
                {
                    throw new ArgumentNullException(nameof(ride), "The ride object is null.");
                }

                await ridesCollection.InsertOneAsync(ride);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding ride to database", ex);
            }
        }


        public async Task DeleteRide(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                await ridesCollection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting ride from database", ex);
            }
        }

        public async Task<List<Ride>> GetAllRides()
        {
            try
            {
                var rides = await ridesCollection.Find(u => true).ToListAsync();
                return rides;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all rides from database", ex);
            }
        }

        public async Task<Ride> GetRideById(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                var ride = await ridesCollection.Find(filter).FirstOrDefaultAsync();
                return ride;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting ride by id from database", ex);
            }
        }


        public async Task UpdateRide(Ride ride, Guid id)
        {
            try
            {
                if (ride == null)
                {
                    throw new ArgumentNullException(nameof(ride), "The ride object is null.");
                }

                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                await ridesCollection.ReplaceOneAsync(filter, ride);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ride in database", ex);
            }
        }

    }
}
