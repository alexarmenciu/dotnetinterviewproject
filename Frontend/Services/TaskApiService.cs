
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/tasks");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TaskModel>>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to get tasks. Status: {response.StatusCode}, Error: {errorContent}");
        }


        public async Task<TaskModel?> CreateTaskAsync(TaskModel task)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/tasks", task);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskModel>();
            }
            
            // If the request failed, throw an exception with the error details
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create task. Status: {response.StatusCode}, Error: {errorContent}");
        }


        public async Task<bool> UpdateTaskAsync(TaskModel task)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/tasks/{task.Id}", task);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to update task. Status: {response.StatusCode}, Error: {errorContent}");
            }
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/tasks/{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to delete task. Status: {response.StatusCode}, Error: {errorContent}");
            }
            
            return response.IsSuccessStatusCode;
        }

        public async Task CompleteTaskAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/tasks/{id}/complete", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to complete task. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }
    }
}
