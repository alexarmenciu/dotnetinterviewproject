using Microsoft.AspNetCore.SignalR.Client;
using Frontend.Models;
using System.Text.Json;

namespace Frontend.Services
{
    public class TaskSignalRService : IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<TaskSignalRService> _logger;

        public TaskSignalRService(IConfiguration configuration, ILogger<TaskSignalRService> logger)
        {
            _logger = logger;
            var apiBaseUrl = $"http://localhost:{Environment.GetEnvironmentVariable("API_PORT") ?? "5000"}";
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUrl}/taskhub")
                .WithAutomaticReconnect()
                .Build();

            SetupEventHandlers();
        }

        public event Action<TaskModel>? TaskCreated;
        public event Action<TaskModel>? TaskUpdated;
        public event Action<TaskModel>? TaskCompleted;
        public event Action<Guid>? TaskDeleted;

        private void SetupEventHandlers()
        {
            _hubConnection.On<JsonElement>("TaskCreated", (taskElement) =>
            {
                try
                {
                    // Print the raw JSON to the browser console for debugging
                    Console.WriteLine($"TaskCreated raw JSON: {taskElement.GetRawText()}");
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var task = JsonSerializer.Deserialize<TaskModel>(taskElement.GetRawText(), options);
                    if (task != null)
                    {
                        TaskCreated?.Invoke(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deserializing TaskCreated event. Raw JSON: {taskElement.GetRawText()}");
                }
            });

            _hubConnection.On<JsonElement>("TaskUpdated", (taskElement) =>
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var task = JsonSerializer.Deserialize<TaskModel>(taskElement.GetRawText(), options);
                    if (task != null)
                    {
                        TaskUpdated?.Invoke(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing TaskUpdated event");
                }
            });

            _hubConnection.On<JsonElement>("TaskCompleted", (taskElement) =>
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var task = JsonSerializer.Deserialize<TaskModel>(taskElement.GetRawText(), options);
                    if (task != null)
                    {
                        TaskCompleted?.Invoke(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing TaskCompleted event");
                }
            });

            _hubConnection.On<Guid>("TaskDeleted", (taskId) =>
            {
                TaskDeleted?.Invoke(taskId);
            });
        }

        public async Task StartAsync()
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("SignalR connection started successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting SignalR connection");
                throw;
            }
        }

        public async Task StopAsync()
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.StopAsync();
                    _logger.LogInformation("SignalR connection stopped");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
            }
        }

        public HubConnectionState ConnectionState => _hubConnection.State;

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
