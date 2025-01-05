using Laverie.API.Infrastructure.repositories;
using Laverie.Domain.Entities;

namespace Laverie.API.Services
{
    public class LaundryService
    {
        private readonly LaundryRepo _laundryRepo;

        public LaundryService(LaundryRepo laundryRepo)
        {
            _laundryRepo = laundryRepo;
        }

        public List<Laundry> GetAllLaundries() => _laundryRepo.GetAll();
        public Laundry GetLaundryById(int id) => _laundryRepo.GetById(id);
        public void AddLaundry(Laundry laundry) => _laundryRepo.Create(laundry);
        public void UpdateLaundry(Laundry laundry) => _laundryRepo.Update(laundry);
        public void DeleteLaundry(int id) => _laundryRepo.Delete(id);
    }
}
