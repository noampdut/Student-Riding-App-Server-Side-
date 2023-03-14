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
            await passengersCollection.InsertOneAsync(passenger);
        }

        public async Task DeletePassenger(Guid id)
        {
            var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
            await passengersCollection.DeleteOneAsync(filter);
        }

        public async Task<List<Passenger>> GetAllPassengers()
        {
            var passengers = await passengersCollection.Find(u => true).ToListAsync();
            return passengers;
        }


        public async Task<Passenger> GetPassengerById(Guid id)
        {
            var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
            var passenger = await passengersCollection.Find(filter).FirstOrDefaultAsync();
            return passenger;
        }

        public async Task UpdatePassenger(Passenger passenger, Guid id)
        {
            var filter = Builders<Passenger>.Filter.Eq(u => u.Id, id);
            await passengersCollection.ReplaceOneAsync(filter, passenger);
        }
    }
}
