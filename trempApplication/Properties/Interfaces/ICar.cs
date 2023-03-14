using trempApplication.Properties.Models;

namespace trempApplication.Properties.Interfaces
{
    public interface ICar
    {
        Task<List<Car>> GetAllCars();
        Task<Car> GetCarById(Guid id);
        Task AddCar(Car car);
        Task UpdateCar(Car car, Guid id);
        Task DeleteCar(Guid id);
    }
}
