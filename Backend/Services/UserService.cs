using Laverie.API.Infrastructure.repositories;
using Laverie.Domain.DTOS;
using Laverie.Domain.Entities;
using Laverie.Domain.Entities;

namespace Laverie.API.Services
{
    public class UserService
    {
        private readonly UserRepo _repo;

        public UserService(UserRepo repo)
        {
            _repo = repo;
        }

        public List<User> GetAll() => _repo.GetAll();

        public User GetById(int id) => _repo.GetById(id);

        public void Create(UserCreationDTO proprietaire) => _repo.Create(proprietaire);

        public void Update(UserCreationDTO proprietaire, int id) => _repo.Update(proprietaire, id);

        public bool Delete(int id) => _repo.Delete(id);

        public User Login(UserLoginDTO user) => _repo.Login(user);
    }
}