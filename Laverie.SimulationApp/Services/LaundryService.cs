using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Laverie.Domain.Entities;
using System.Net.Http.Json;
using System.Transactions;
using Laverie.Domain.DTOS;

namespace Laverie.SimulationApp.Services
{
    public class LaundryService
    {
        private readonly HttpClient _httpClient;

        
        public LaundryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       
        public async Task<List<User>> GetConfigurationAsync()
        {
            try
            {
               
                var response = await _httpClient.GetAsync("api/Configuration");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                   
                    var users = JsonSerializer.Deserialize<List<User>>(responseBody);

                    return users ?? new List<User>(); 
                }
                else
                {
                   
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
              
                Console.WriteLine($"An error occurred while fetching data: {ex.Message}");
                return new List<User>(); 
            }
        }



        public async Task<bool> StartMachineStateAsync(int machineId, int idCycle)
        {
            try
            { 
                string url = $"api/Configuration/startMachine";
                 
                var requestBody = new { MachineId = machineId, IdCycle = idCycle };
                 
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Machine started successfully.");
                    return true;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode} - {errorResponse}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while starting the machine: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> StopMachineStateAsync(int machineId)
        {
            try
            { 
                string url = $"api/Configuration/stopMachine";
                 
                var response = await _httpClient.PostAsJsonAsync(url, machineId);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Machine stopped successfully.");
                    return true;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode} - {errorResponse}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while stopping the machine: {ex.Message}");
                return false;
            }
        }

        public async Task<int> AddCycleAsync(CycleCreationDTO cycle)
        {
            try
            {
                // Construct the URL to call the backend API for adding a cycle
                string url = "api/Configuration/addCycle"; // Adjust this URL based on your API route

                // Send a POST request to insert the cycle with the provided cycle object
                var response = await _httpClient.PostAsJsonAsync(url, cycle);

                // If the response is successful, read the content (cycle ID) from the response body
                var responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the response to get the CycleId
                var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);

                // Check if cycleId exists and convert it from string (or number)
                if (responseData != null && responseData.ContainsKey("cycleId"))
                {
                    var cycleIdElement = responseData["cycleId"];

                    // Try parsing the cycleId as an integer, handle both string and integer cases
                    if (cycleIdElement.ValueKind == JsonValueKind.String)
                    {
                        // If cycleId is a string, attempt to parse it
                        if (int.TryParse(cycleIdElement.GetString(), out int cycleId))
                        {
                            Console.WriteLine($"Cycle added successfully. Cycle ID: {cycleId}");
                            return cycleId;
                        }
                    }
                    else if (cycleIdElement.ValueKind == JsonValueKind.Number)
                    {
                        // If cycleId is a number, directly cast it
                        int cycleId = cycleIdElement.GetInt32();
                        Console.WriteLine($"Cycle added successfully. Cycle ID: {cycleId}");
                        return cycleId;
                    }
                }

                Console.WriteLine("Failed to add the cycle.");
                return 0; // Return 0 if cycleId is not found or can't be parsed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the cycle: {ex.Message}");
                return 0; // Return 0 if an error occurs
            }
        }



    }
}
