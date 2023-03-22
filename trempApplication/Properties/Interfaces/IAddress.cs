using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface IAddress
    {
        Task<(bool IsSuccess, List<Address> Address, string ErrorMessage)> GetAllAddresses();
        Task<(bool IsSuccess, Address Address, string ErrorMessage)> GetAddressById(Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> AddAddress(Address address);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateAddress(Address address, Guid id);
        Task<(bool IsSuccess, string ErrorMessage)> DeleteAddress(Guid id);
    }
}
