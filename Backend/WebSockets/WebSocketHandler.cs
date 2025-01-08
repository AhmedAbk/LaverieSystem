using System;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Laverie.API.Infrastructure.context;

namespace Laverie.API.Infrastructure.WebSockets
{
    public class WebSocketHandler
    {
        private static List<WebSocket> _sockets = new List<WebSocket>();
        private readonly IServiceProvider _serviceProvider;

        public WebSocketHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleWebSocket(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _sockets.Add(webSocket);
                Console.WriteLine("WebSocket connection accepted.");

                await ReceiveMessages(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                Console.WriteLine("WebSocket request expected.");
            }
        }

        private async Task ReceiveMessages(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {message}");
                    await HandleMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _sockets.Remove(webSocket);
                    Console.WriteLine("WebSocket connection closed.");
                }
            }
        }

        private async Task HandleMessage(string message)
        {
            try
            {
                var messageObject = JsonConvert.DeserializeObject<dynamic>(message);

                if (messageObject.action == "updateMachineStatus")
                {
                    string machineId = messageObject.machineId;
                    bool status = messageObject.status;

                    await UpdateMachineStatus(machineId, status);
                    await BroadcastMachineStatus(machineId, status);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling WebSocket message: {ex.Message}");
            }
        }

        private async Task UpdateMachineStatus(string machineId, bool status)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    using (var connection = dbContext.CreateConnection())
                    {
                        var mySqlConnection = (MySqlConnection)connection;

                        if (mySqlConnection.State != ConnectionState.Open)
                        {
                            await mySqlConnection.OpenAsync();
                        }

                        var query = "UPDATE Machines SET Status = @Status WHERE Id = @MachineId";
                        using (var command = new MySqlCommand(query, mySqlConnection))
                        {
                            command.Parameters.AddWithValue("@Status", status);
                            command.Parameters.AddWithValue("@MachineId", machineId);

                            int rowsAffected = await command.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine($"Machine {machineId} status updated to {status}");
                            }
                            else
                            {
                                Console.WriteLine($"Machine {machineId} not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating machine status: {ex.Message}");
            }
        }

        private async Task BroadcastMachineStatus(string machineId, bool status)
        {
            var message = new { machineId, status };
            var jsonMessage = JsonConvert.SerializeObject(message);

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage)), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"Broadcasted message: {jsonMessage}");
                }
            }
        }
    }
}