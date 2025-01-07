﻿using System;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Laverie.Domain.DTOS;
using Laverie.Domain.Entities;
using Laverie.SimulationApp.Services;

namespace LaverieConsoleApp
{
    class Program
    {
        static LaundryService _laundryService;

        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7244/") };
            _laundryService = new LaundryService(httpClient);

            while (true)
            {
                try
                {
                    await ShowMainMenu();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static async Task ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Laundry Management System ===");
            Console.WriteLine("1. View Configuration");
            Console.WriteLine("0. Exit");

            Console.Write("\nEnter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ViewConfiguration();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }

        static async Task ViewConfiguration()
        {
            Console.Clear();
            Console.WriteLine("=== View Configuration ===");

            try
            {
                
                var owners = await _laundryService.GetConfigurationAsync();

                if (owners.Count == 0)
                {
                    Console.WriteLine("No data found.");
                    return;
                }

                
                Console.WriteLine("List of Users:");
                for (int i = 0; i < owners.Count; i++)
                {
                    var owner = owners[i];
                    Console.WriteLine($"{i + 1}. {owner.name ?? "Unknown"} (Email: {owner.email ?? "No Email"})");
                }

                Console.Write("\nSelect a user by number: ");
                if (!int.TryParse(Console.ReadLine(), out int selectedUserIndex) || selectedUserIndex - 1 < 0 || selectedUserIndex - 1 >= owners.Count)
                {
                    Console.WriteLine("Invalid selection. Returning to main menu...");
                    Console.ReadKey();
                    return;
                }

                var selectedOwner = owners[selectedUserIndex - 1];
                Console.WriteLine($"\nYou selected: {selectedOwner.name ?? "Unknown"} (Email: {selectedOwner.email ?? "No Email"})");


                if (selectedOwner.laundries?.Count > 0)
                {
                    Console.WriteLine("\nLaundries:");
                    for (int i = 0; i < selectedOwner.laundries.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {selectedOwner.laundries[i].nomLaverie ?? "Unnamed"}");
                    }

                    Console.Write("\nSelect a laundry by number: ");
                    if (!int.TryParse(Console.ReadLine(), out int selectedLaundryIndex) || selectedLaundryIndex - 1 < 0 || selectedLaundryIndex - 1 >= selectedOwner.laundries.Count)
                    {
                        Console.WriteLine("Invalid selection. Returning to main menu...");
                        Console.ReadKey();
                        return;
                    }

                    var selectedLaundry = selectedOwner.laundries[selectedLaundryIndex - 1];
                    Console.WriteLine($"\nYou selected: {selectedLaundry.nomLaverie ?? "Unnamed"}");

                   
                    if (selectedLaundry.machines?.Count > 0)
                    {
                        Console.WriteLine("\nMachines:");
                        for (int i = 0; i < selectedLaundry.machines.Count; i++)
                        {
                            var machine = selectedLaundry.machines[i];
                            Console.WriteLine($"{i + 1}. Machine {machine.id}: {machine.type ?? "Unknown"} - Status: {(machine.status ? "In Use" : "Available")}");
                        }

                        Console.Write("\nSelect a machine by number: ");
                        if (!int.TryParse(Console.ReadLine(), out int selectedMachineIndex) || selectedMachineIndex - 1 < 0 || selectedMachineIndex - 1 >= selectedLaundry.machines.Count)
                        {
                            Console.WriteLine("Invalid selection. Returning to main menu...");
                            Console.ReadKey();
                            return;
                        }

                        var selectedMachine = selectedLaundry.machines[selectedMachineIndex - 1];
                        Console.WriteLine($"\nYou selected Machine {selectedMachine.id}: {selectedMachine.type ?? "Unknown"}");

                        
                        if (selectedMachine.cycles?.Count > 0)
                        {
                            Console.WriteLine("\nCycles:");
                            for (int i = 0; i < selectedMachine.cycles.Count; i++)
                            {
                                var cycle = selectedMachine.cycles[i];
                                Console.WriteLine($"{i + 1}. Duration: {cycle.cycleDuration} - Price: {cycle.price}");
                            }

                            Console.Write("\nSelect a cycle by number: ");
                            if (!int.TryParse(Console.ReadLine(), out int selectedCycleIndex) || selectedCycleIndex - 1 < 0 || selectedCycleIndex - 1 >= selectedMachine.cycles.Count)
                            {
                                Console.WriteLine("Invalid selection. Returning to main menu...");
                                Console.ReadKey();
                                return;
                            }

                            Cycle selectedCycle = selectedMachine.cycles[selectedCycleIndex - 1];
                            Console.WriteLine($"\nYou selected: Duration - {selectedCycle.cycleDuration}, Price - {selectedCycle.price}");

                            
                            Console.WriteLine("\nOptions:\n1. Start Machine \n2. Stop Machine");
                            Console.Write("Select an option: ");
                            var option = Console.ReadLine();

                         

                            switch (option)
                            {
                                case "1":
                                    await _laundryService.StartMachineStateAsync(selectedCycle.machineId, selectedCycle.id);
                                    StartCycleTimer(selectedCycle.cycleDuration, selectedCycle.machineId);
                                    break;
                                case "2":
                                    await _laundryService.StopMachineStateAsync(selectedCycle.machineId);
                                    break;
                                default:
                                    Console.WriteLine("Invalid option.");
                                    break;
                            }
                        }
                        else
                        {
                            

                            
                            Console.Write("\nEnter cycle duration: ");
                            string? duration = Console.ReadLine();

                            Console.Write("Enter cycle price: ");
                            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                            {
                                Console.WriteLine("Invalid price. Returning to main menu...");
                                Console.ReadKey();
                                return;
                            }

                            
                            var newCycle = new CycleCreationDTO
                            {
                                machineId = selectedMachine.id,
                                cycleDuration = duration,
                                price = price
                            };

                            
                            int createdCycleId = await _laundryService.AddCycleAsync(newCycle);

                            if (createdCycleId > 0)
                            {
                                Console.WriteLine("\nNew cycle added successfully!");
                                Console.WriteLine($"Duration: {newCycle.cycleDuration}, Price: {newCycle.price}");

                                
                                Console.WriteLine("\nOptions:\n1. Start Machine \n2. Stop Machine");
                                Console.Write("Select an option: ");
                                var option = Console.ReadLine();

                                switch (option)
                                {
                                    case "1":
                                        
                                        await _laundryService.StartMachineStateAsync(selectedMachine.id, createdCycleId);
                                        StartCycleTimer(newCycle.cycleDuration, newCycle.machineId);
                                        break;
                                    case "2":
                                       
                                        await _laundryService.StopMachineStateAsync(selectedMachine.id);
                                        break;
                                    default:
                                        Console.WriteLine("Invalid option.");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failed to add the new cycle.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No machines available in this laundry.");
                    }
                }
                else
                {
                    Console.WriteLine("No laundries available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching data: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }

        static void StartCycleTimer(string duration, int machineId)
        {
            
            if (int.TryParse(duration, out int cycleDurationSecs))
            {
                
                var timer = new System.Timers.Timer(1000);
                timer.Elapsed += async (sender, e) =>
                {
                    cycleDurationSecs--; 

                    if (cycleDurationSecs <= 0)
                    {
                        timer.Stop(); 
                        Console.WriteLine("Cycle completed. Stopping the machine...");
                        await _laundryService.StopMachineStateAsync(machineId); 
                    }
                    else
                    {
                        Console.WriteLine($"Cycle in progress. Remaining time: {cycleDurationSecs} seconds.");
                    }
                };

                timer.Start(); 
            }
            else
            {
                Console.WriteLine("Invalid cycle duration. Unable to start the timer.");
            }
        }


    }
}
