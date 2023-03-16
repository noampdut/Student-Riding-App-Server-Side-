
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;
namespace trempApplication.Properties.Services
{
    public class CarService : ICar
    {

        private IMongoCollection<Car> carsCollection;
        private IMongoCollection<Passenger> passengersCollection;
        public CarService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            carsCollection = mongoDB.GetCollection<Car>("Cars");
            passengersCollection = mongoDB.GetCollection<Passenger>("Passengers");

           

        }
        public async Task AddCar(Car car)
        {
            try
            {
                if (car == null)
                {
                    throw new ArgumentNullException(nameof(car), "The car object is null.");
                }

                await carsCollection.InsertOneAsync(car);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding car to database", ex);
            }
        }


        public async Task DeleteCar(Guid carId)
        {
            try
            {
                var filter = Builders<Car>.Filter.Eq(u => u.Id, carId);
                var car = await carsCollection.Find(filter).FirstOrDefaultAsync();

                // Delete the car
                await carsCollection.DeleteOneAsync(filter);

                // Update the carIds property of all passengers that own the car
                var update = Builders<Passenger>.Update.Pull(x => x.CarIds, carId);
                var result = await passengersCollection.UpdateManyAsync(p => p.CarIds.Contains(carId), update);

            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting car from database", ex);
            }
        }



        public async Task<List<Car>> GetAllCars()
        {
            try
            {
                var cars = await carsCollection.Find(u => true).ToListAsync();
                return cars;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all cars from database", ex);
            }
        }

        public async Task<Car> GetCarById(Guid id)
        {
            try
            {
                var filter = Builders<Car>.Filter.Eq(u => u.Id, id);
                var car = await carsCollection.Find(filter).FirstOrDefaultAsync();
                return car;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting car by id from database", ex);
            }
        }

        public async Task UpdateCar(Car car, Guid id)
        {
            try
            {
                if (car == null)
                {
                    throw new ArgumentNullException(nameof(car), "The car object is null.");
                }

                var filter = Builders<Car>.Filter.Eq(u => u.Id, id);
                await carsCollection.ReplaceOneAsync(filter, car);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating car in database", ex);
            }
        }

    }
}