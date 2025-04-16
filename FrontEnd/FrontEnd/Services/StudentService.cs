using Frontend.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Frontend.Services
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public StudentService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient; 
            _authenticationStateProvider = authenticationStateProvider;
        }
        private class AverageGradeResponse { public decimal? average { get; set; } }

        public async Task<List<CourseGradeDto>?> GetMyCourseGradesAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!(user.Identity?.IsAuthenticated ?? false)) return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int studentId))
            {
                Console.WriteLine("StudentService: Could not find or parse User ID claim.");
                return null;
            }

            Console.WriteLine($"StudentService: Getting course grades for student ID: {studentId}");

            try
            {
                var courseGrades = await _httpClient.GetFromJsonAsync<List<CourseGradeDto>>($"api/students/{studentId}/coursegrades");
                
                return courseGrades ?? new List<CourseGradeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting student course grades: {ex.Message}");
                return null;
            }
        }
        public async Task<decimal?> GetMyAverageGradeAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                Console.WriteLine("StudentService (Avg): User not authenticated.");
                return null;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int studentId))
            {
                Console.WriteLine("StudentService (Avg): Could not find or parse User ID claim.");
                return null; 
            }

            Console.WriteLine($"StudentService: Requesting average grade for student ID: {studentId}");
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AverageGradeResponse>($"api/students/{studentId}/averagegrade"); // Endpoint API pt medie
                Console.WriteLine($"StudentService: Received average grade response. Average: {response?.average}");
                return response?.average; 
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error getting student average grade (HTTP): {httpEx.Message} (StatusCode: {httpEx.StatusCode})");
                return null;
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                Console.WriteLine($"Error getting student average grade (JSON): {jsonEx.Message}");
                return null;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error getting student average grade (General): {ex.Message}");
                return null;
            }
        }
    }
}
