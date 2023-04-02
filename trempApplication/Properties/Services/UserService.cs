using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class UserService : IUser
    {
        private IMongoCollection<User> usersCollection;
        private IMongoCollection<Passenger> passengersCollection;

        public UserService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            usersCollection = mongoDB.GetCollection<User>("Users");
            passengersCollection = mongoDB.GetCollection<Passenger>("Passengers");

        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return (false, "user is null object");
                }

                await usersCollection.InsertOneAsync(user);
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user to database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteUser(string IdNumber)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.IdNumber, IdNumber);
                var user = await usersCollection.Find(filter).FirstOrDefaultAsync();
                if (user == null)
                {
                    return (false, "No user was found to be deleted");
                }
                // Delete the car
                await usersCollection.DeleteOneAsync(filter);
                return(true, null);

            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting user from database", ex);
            }
        }


        // The method get an IdNumber (like 315597151) and return the passenger with his props
        public async Task<(bool IsSuccess, Passenger passenger, string ErrorMessage)> GetUserById(string IdNumber, string password)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.IdNumber, IdNumber);
                var user = await usersCollection.Find(filter).FirstOrDefaultAsync();
                if (user != null)
                {
                    if(user.Password != password) { return (false, null, "password is wrong"); } 

                    var query = Builders<Passenger>.Filter.Eq(u => u.IdNumber, user.IdNumber);
                    var passenger = await passengersCollection.Find(query).FirstOrDefaultAsync();
                    if (passenger != null)
                    {
                        return (true, passenger, null);
                    }
                }
                return (false, null, "No user found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting user/passenger by id from database", ex);
            }
        }

        // The method get a user with his Id number and new password and update the password on DB. 
        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return (false, "The user object is null.");
                }

                var filter = Builders<User>.Filter.Eq(u => u.IdNumber, user.IdNumber);
                await usersCollection.ReplaceOneAsync(filter, user);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating user in database", ex);
            }
        }
    }
}
