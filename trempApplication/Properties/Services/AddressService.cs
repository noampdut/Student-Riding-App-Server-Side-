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
        public async Task<(bool IsSuccess, string ErrorMessage)> AddAddress(Address address)
        {
            try
            {
                if (address == null)
                {
                    return (false, "address is null object");
                }

                await addressesCollection.InsertOneAsync(address);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error adding address to database", ex);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteAddress(Guid id)
        {
            try
            {
                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                var address =  await addressesCollection.DeleteOneAsync(filter);
                if (address == null)
                {
                    return (false, "No address was found to be deleted");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error deleting address from database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Address> Address, string ErrorMessage)> GetAllAddresses()
        {
            try
            {
                var addresses = await addressesCollection.Find(u => true).ToListAsync();
                if (addresses != null)
                {
                    return (true, addresses, null);
                }
                return (false, null, "No addresses found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting all addresses from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Address Address, string ErrorMessage)> GetAddressById(Guid id)
        {
            try
            {
                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                var address = await addressesCollection.Find(filter).FirstOrDefaultAsync();
                if (address != null)
                {
                    return (true, address, null);
                }
                return (false, null, "No address found");
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error getting address by id from database", ex);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateAddress(Address address, Guid id)
        {
            try
            {
                if (address == null)
                {
                    return (false, "The address object is null.");
                }

                var filter = Builders<Address>.Filter.Eq(u => u.Id, id);
                await addressesCollection.ReplaceOneAsync(filter, address);
                return (true, null);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                throw new Exception("Error updating address in database", ex);
            }
        }

    }
}

