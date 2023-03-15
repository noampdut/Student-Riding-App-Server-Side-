using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class AddressService: IAddress
    {
        private IMongoCollection<Address> addressesCollection;
        public AddressService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            addressesCollection = mongoDB.GetCollection<Address>("Addresses");
        }
        public async Task AddAddress(Address address)
        {
            try
            {
                if (address == null)
                {
                    throw new ArgumentNullException(nameof(address), "The address object is null.");
                }

                await addressesCollection.InsertOneAsync(address);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error adding address to database", ex);
            }
        }

        public async Task DeleteAddress(Guid id)
        {
            try
            {
                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                await addressesCollection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting address from database", ex);
            }
        }

        public async Task<List<Address>> GetAllAddresses()
        {
            try
            {
                var addresses = await addressesCollection.Find(u => true).ToListAsync();
                return addresses;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all addresses from database", ex);
            }
        }

        public async Task<Address> GetAddressById(Guid id)
        {
            try
            {
                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                var address = await addressesCollection.Find(filter).FirstOrDefaultAsync();
                return address;
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting address by id from database", ex);
            }
        }

        public async Task UpdateAddress(Address address, Guid id)
        {
            try
            {
                if (address == null)
                {
                    throw new ArgumentNullException(nameof(address), "The address object is null.");
                }

                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                await addressesCollection.ReplaceOneAsync(filter, address);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating address in database", ex);
            }
        }

    }
}

