
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
        public async Task<(bool IsSuccess, string ErrorMessage)> AddCar(Car car)
        {
            try
            {
                if (car == null)
                {
                    return (false, "Car is null object");
                }

                await carsCollection.InsertOneAsync(car);
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding car to database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteCar(Guid carId)
        {
            try
            {
                var filter = Builders<Car>.Filter.Eq(u => u.Id, carId);
                var car = await carsCollection.Find(filter).FirstOrDefaultAsync();
                if (car == null)
                {
                    return (false, "No car was found to be deleted");
                }
                // Delete the car
                await carsCollection.DeleteOneAsync(filter);

                // Update the carIds property of all passengers that own the car
                var update = Builders<Passenger>.Update.Pull(x => x.CarIds, carId);
                var result = await passengersCollection.UpdateManyAsync(p => p.CarIds.Contains(carId), update);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting car from database", ex);
            }
        }



        public async Task<(bool IsSuccess, List<Car> Car, string ErrorMessage)> GetAllCars()
        {
            try
            {
                var cars = await carsCollection.Find(u => true).ToListAsync();
                if (cars != null)
                {
                    return (true, cars, null);
                }
                return (false, null, "No cars found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all cars from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Car Car, string ErrorMessage)> GetCarById(Guid id)
        {
            try
            {
                var filter = Builders<Car>.Filter.Eq(u => u.Id, id);
                var car = await carsCollection.Find(filter).FirstOrDefaultAsync();
                if (car != null)
                {
                    return (true, car, null);
                }
                return (false, null, "No car found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting car by id from database", ex);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateCar(Car car, Guid id)
        {
            try
            {
                if (car == null)
                {
                    return (false, "The car object is null.");
                }

                var filter = Builders<Car>.Filter.Eq(u => u.Id, id);
                await carsCollection.ReplaceOneAsync(filter, car);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating car in database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Car> cars, string ErrorMessage)> GetCarsByOwner(Guid owner)
        {
            try
            {
                var filter = Builders<Car>.Filter.Eq(u => u.Owner, owner);
                var cars = await carsCollection.Find(filter).ToListAsync();
                if (cars != null && cars.Count > 0)
                {
                    return (true, cars, null);
                }
                return (false, null, "No cars found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting cars by owner from database", ex);
            }
        }

    }
}