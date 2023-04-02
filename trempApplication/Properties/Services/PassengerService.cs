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
        public async Task<(bool IsSuccess, string ErrorMessage)> AddPassenger(Passenger passenger)
        {
            try
            {
                if (passenger == null)
                {
                    return (false, "passenger is null object");
                }

                await passengersCollection.InsertOneAsync(passenger);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error adding passenger to database", ex);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeletePassenger(Guid id)
        {
            try
            {
                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                var passenger = await passengersCollection.DeleteOneAsync(filter);
                if (passenger == null)
                {
                    return (false, "No passenger was found to be deleted");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting passenger from database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Passenger> Passenger, string ErrorMessage)> GetAllPassengers()
        {
            try
            {
                var passengers = await passengersCollection.Find(u => true).ToListAsync();
                if (passengers != null)
                {
                    return (true, passengers, null);
                }
                return (false, null, "No passengers found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all passengers from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Passenger Passenger, string ErrorMessage)> GetPassengerById(Guid id)
        {
            try
            {
                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                var passenger = await passengersCollection.Find(filter).FirstOrDefaultAsync();
                if (passenger != null)
                {
                    return (true, passenger, null);
                }
                return (false, null, "No passenger found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting passenger by id from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Passenger Passenger, string ErrorMessage)> GetPassengerByIdNumber(string IdNumber)
        {
            try
            {
                var filter = Builders<Passenger>.Filter.Eq(u => u.IdNumber, IdNumber);
                var passenger = await passengersCollection.Find(filter).FirstOrDefaultAsync();
                if (passenger != null)
                {
                    return (true, passenger, null);
                }
                return (false, null, "No passenger found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting passenger by id from database", ex);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdatePassenger(Passenger passenger, Guid id)
        {
            try
            {
                if (passenger == null)
                {
                    return (false, "The passenger object is null.");
                }

                var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
                await passengersCollection.ReplaceOneAsync(filter, passenger);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating passenger in database", ex);
            }
        }
    }
}