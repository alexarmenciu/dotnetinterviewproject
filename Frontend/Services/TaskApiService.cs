
using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services
{
    public class TaskApiService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl;
        public TaskApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            // Prefer environment variable, fallback to config
            _baseUrl = $"http://localhost:{Environment.GetEnvironmentVariable("API_PORT") ?? "5000"}";
        }


        public async Task<List<TaskModel>?> GetTasksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TaskModel>>($"{_baseUrl}/tasks");
        }


        public async Task<TaskModel?> CreateTaskAsync(TaskModel task)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/tasks", task);
            return await response.Content.ReadFromJsonAsync<TaskModel>();
        }


        public async Task<bool> UpdateTaskAsync(TaskModel task)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/tasks/{task.Id}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/tasks/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task CompleteTaskAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/tasks/{id}/complete", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
