using Laverie.API.Infrastructure.repositories;
using Laverie.Domain.DTOS;
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
        public bool AddLaundry(LaundryCreationDTO laundry) => _laundryRepo.Create(laundry);
        public bool UpdateLaundry(LaundryUpdateDTO laundry, int id) => _laundryRepo.Update(laundry, id);
        public bool DeleteLaundry(int id) => _laundryRepo.Delete(id);
    }
}
