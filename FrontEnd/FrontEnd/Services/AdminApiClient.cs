using FrontEnd.DTOs;
using System.Net.Http;
using System.Net.Http.Json; // Requires Microsoft.Extensions.Http NuGet package (usually included)
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FrontEnd.Services
{
    public class AdminApiClient
    {
        private readonly HttpClient _httpClient;

        // Inject HttpClient configured in Program.cs
        public AdminApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // --- Teacher Methods ---
        public async Task<List<UserDto>?> GetTeachersAsync() =>
            await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Admin/teachers");

        public async Task<HttpResponseMessage> AddTeacherAsync(CreateUserDto teacher) =>
            await _httpClient.PostAsJsonAsync("api/Admin/teachers", teacher);

        public async Task<HttpResponseMessage> DeleteTeacherAsync(int id) =>
            await _httpClient.DeleteAsync($"api/Admin/teachers/{id}");

        // --- Course Methods ---
        public async Task<List<CourseDto>?> GetCoursesAsync() =>
            await _httpClient.GetFromJsonAsync<List<CourseDto>>("api/Admin/courses");

        public async Task<HttpResponseMessage> AddCourseAsync(CreateCourseDto course) =>
            await _httpClient.PostAsJsonAsync("api/Admin/courses", course);

        public async Task<HttpResponseMessage> DeleteCourseAsync(int id) =>
            await _httpClient.DeleteAsync($"api/Admin/courses/{id}");

        // --- Student Methods ---
        public async Task<List<UserDto>?> GetStudentsAsync() =>
            await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Admin/students");

        public async Task<HttpResponseMessage> AddStudentAsync(CreateUserDto student) =>
            await _httpClient.PostAsJsonAsync("api/Admin/students", student);

        public async Task<HttpResponseMessage> DeleteStudentAsync(int id) =>
            await _httpClient.DeleteAsync($"api/Admin/students/{id}");

        // --- Grade Methods ---
        public async Task<List<GradeDto>?> GetGradesAsync() =>
            await _httpClient.GetFromJsonAsync<List<GradeDto>>("api/Admin/grades");

        public async Task<HttpResponseMessage> AddGradeAsync(CreateGradeDto grade) =>
            await _httpClient.PostAsJsonAsync("api/Admin/grades", grade);

        public async Task<HttpResponseMessage> DeleteGradeAsync(int id) =>
            await _httpClient.DeleteAsync($"api/Admin/grades/{id}");
    }
}
