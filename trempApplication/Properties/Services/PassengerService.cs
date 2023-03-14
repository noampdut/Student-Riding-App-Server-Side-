using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class PassengerService : IPassenger
    {
        private IMongoCollection<Passenger> passengersCollection;
        public PassengerService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            passengersCollection = mongoDB.GetCollection<Passenger>("Passengers");
        }
        public async Task AddPassenger(Passenger passenger)
        {
            try
            {
                if (passenger == null)
                {
                    throw new ArgumentNullException(nameof(passenger), "The passenger object is null.");
                }

                await passengersCollection.InsertOneAsync(passenger);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error adding passenger to database", ex);
            }
        }

        public async Task DeletePassenger(Guid id)
        {
            try
            {
                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                await passengersCollection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting passenger from database", ex);
            }
        }

        public async Task<List<Passenger>> GetAllPassengers()
        {
            try
            {
                var passengers = await passengersCollection.Find(u => true).ToListAsync();
                return passengers;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all passengers from database", ex);
            }
        }

        public async Task<Passenger> GetPassengerById(Guid id)
        {
            try
            {
                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                var passenger = await passengersCollection.Find(filter).FirstOrDefaultAsync();
                return passenger;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting passenger by id from database", ex);
            }
        }

        public async Task UpdatePassenger(Passenger passenger, Guid id)
        {
            try
            {
                if (passenger == null)
                {
                    throw new ArgumentNullException(nameof(passenger), "The passenger object is null.");
                }

                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                await passengersCollection.ReplaceOneAsync(filter, passenger);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating passenger in database", ex);
            }
        }
    }
}