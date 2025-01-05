using Laverie.API.Infrastructure.repositories;
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
        public async Task<List<Machine>> GetAllMachinesAsync()
        {
            return await _machineRepo.GetAllMachinesAsync();
        }

        // Get machine by ID
        public async Task<Machine> GetMachineByIdAsync(int id)
        {
            return await _machineRepo.GetMachineByIdAsync(id);
        }

        // Add a new machine
        public async Task<Machine> AddMachineAsync(Machine machine)
        {
            return await _machineRepo.AddMachineAsync(machine);
        }

        // Update machine details
        public async Task<bool> UpdateMachineAsync(Machine machine)
        {
            return await _machineRepo.UpdateMachineAsync(machine);
        }

        // Delete machine
        public async Task<bool> DeleteMachineAsync(int id)
        {
            return await _machineRepo.DeleteMachineAsync(id);
        }
    }
}
