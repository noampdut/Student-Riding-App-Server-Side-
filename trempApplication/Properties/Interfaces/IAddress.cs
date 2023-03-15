using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IAddress
    {
        Task<List<Address>> GetAllAddresses();
        Task<Address> GetAddressById(Guid id);
        Task AddAddress(Address address);
        Task UpdateAddress(Address address, Guid id);
        Task DeleteAddress(Guid id);
    }
}
