using Laverie.API.Infrastructure.repositories;
using Laverie.Domain.DTOS;
using Laverie.Domain.Entities;

namespace Laverie.API.Services
{
    public class MachineService
    {
        private readonly MachineRepo _machineRepo;

        public MachineService(MachineRepo machineRepo)
        {
            _machineRepo = machineRepo;
        }

        // Get all machines
        public  List<Machine> GetAllMachinesAsync()
        {
            return  _machineRepo.GetAll();
        }

        // Get machine by ID
        public  Machine GetMachineByIdAsync(int id)
        {
            return  _machineRepo.GetById(id);
        }

        // Add a new machine
        public bool AddMachineAsync(MachineCreationDTO machine)
        {
            return  _machineRepo.Create(machine);
        }

        // Update machine details
        public bool UpdateMachineAsync(MachineUpdateDTO machine, int id)
        {
            return  _machineRepo.Update(machine, id);
        }

        // Delete machine
        public bool DeleteMachineAsync(int id)
        {
            return  _machineRepo.Delete(id);
        }
    }
}
