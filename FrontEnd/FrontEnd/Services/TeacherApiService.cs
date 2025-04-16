using Frontend.DTOs;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Frontend.Services
{
    public class TeacherApiService : ITeacherApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBasePath = "api/teacher"; // Matches refactored controller route

        // Helper class to deserialize backend error responses (reuse if defined elsewhere)
        private class ErrorResponse { public string? Message { get; set; } }


        public TeacherApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<CourseDto>?> GetMyCoursesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/courses");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return new List<CourseDto>();
                    return await response.Content.ReadFromJsonAsync<List<CourseDto>>();
                }
                // Handle non-success (e.g., 401 Unauthorized, 403 Forbidden)
                Debug.WriteLine($"Error fetching teacher courses: {response.StatusCode}");
                return null;
            }
            catch (Exception ex) { Debug.WriteLine($"Exception fetching teacher courses: {ex.Message}"); return null; }
        }

        public async Task<List<UserDto>?> GetStudentsInCourseAsync(int courseId) // Assuming backend returns UserDto
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/courses/{courseId}/students");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return new List<UserDto>();
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>();
                }
                Debug.WriteLine($"Error fetching students for course {courseId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex) { Debug.WriteLine($"Exception fetching students for course {courseId}: {ex.Message}"); return null; }
        }

        public async Task<List<GradeDto>?> GetGradesForCourseAsync(int courseId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/courses/{courseId}/grades");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return new List<GradeDto>();
                    return await response.Content.ReadFromJsonAsync<List<GradeDto>>();
                }
                Debug.WriteLine($"Error fetching grades for course {courseId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex) { Debug.WriteLine($"Exception fetching grades for course {courseId}: {ex.Message}"); return null; }
        }

        public async Task<(bool Success, GradeDto? ResultGrade, string? ErrorMessage)> AssignOrUpdateGradeAsync(AssignGradeDto gradeDto)
        {
            try
            {
                // POST api/teacher/grades
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/grades", gradeDto);

                if (response.IsSuccessStatusCode) // Expect 200 OK with grade data
                {
                    var resultGrade = await response.Content.ReadFromJsonAsync<GradeDto>();
                    return (true, resultGrade, null);
                }
                else
                {
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message)) errorMessage = errorResponse.Message;
                        }
                        catch { /* Ignore */ }
                    }
                    Debug.WriteLine($"Error assigning grade: {response.StatusCode} - {errorMessage}");
                    return (false, null, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception assigning grade: {ex.Message}");
                return (false, null, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> AddStudentToCourseAsync(int courseId, int studentId)
        {
            try
            {
                // POST api/teacher/courses/{courseId}/students/{studentId}
                var response = await _httpClient.PostAsync($"{ApiBasePath}/courses/{courseId}/students/{studentId}", null); // No body needed

                if (response.IsSuccessStatusCode) // Expect 200 OK or 204 NoContent
                {
                    return (true, null);
                }
                else
                {
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message)) errorMessage = errorResponse.Message;
                        }
                        catch { /* Ignore */ }
                    }
                    // Add specific messages based on status code if needed (403, 404, 400)
                    Debug.WriteLine($"Error adding student {studentId} to course {courseId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception adding student {studentId} to course {courseId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveStudentFromCourseAsync(int courseId, int studentId)
        {
            try
            {
                // DELETE api/teacher/courses/{courseId}/students/{studentId}
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/courses/{courseId}/students/{studentId}");

                if (response.IsSuccessStatusCode) // Expect 204 No Content
                {
                    return (true, null);
                }
                else
                {
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message)) errorMessage = errorResponse.Message;
                        }
                        catch { /* Ignore */ }
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) errorMessage = "Student enrollment not found.";

                    Debug.WriteLine($"Error removing student {studentId} from course {courseId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception removing student {studentId} from course {courseId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        // --- Method to get all students ---
        // Option A: Inject IAdminApiService here (Quickest if already setup)
        // private readonly IAdminApiService _adminApiService;
        // public TeacherApiService(HttpClient httpClient, IAdminApiService adminApiService) { /* ... */ _adminApiService = adminApiService; }
        // public async Task<List<UserDto>?> GetAllStudentsAsync() => await _adminApiService.GetStudentsAsync();

        // Option B: Re-implement call to admin endpoint (if you prefer keeping services separate)
        public async Task<List<UserDto>?> GetAllStudentsAsync()
        {
            // GET api/admin/students (Assuming this endpoint still exists and returns needed data)
            // Warning: This bypasses teacher authorization for this specific call
            try
            {
                // Use admin path explicitly here, or create a dedicated student endpoint
                var response = await _httpClient.GetAsync($"api/Admin/students");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return new List<UserDto>();
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>();
                }
                return null;
            }
            catch { return null; }
        }

        public async Task<BulkCreateGradesResponseDto?> AddGradesBulkAsync(int courseId, List<StudentGradeEntryDto> gradeEntries)
        {
            try
            {
                // POST api/teacher/courses/{courseId}/grades/bulk
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/courses/{courseId}/grades/bulk", gradeEntries);

                // Expect 200 OK or 400 Bad Request (with details)
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    try
                    {
                        // Attempt to deserialize the response DTO in both success and error cases
                        // as it contains success count and error list
                        return await response.Content.ReadFromJsonAsync<BulkCreateGradesResponseDto>();
                    }
                    catch (Exception deserEx)
                    {
                        Debug.WriteLine($"Error deserializing bulk grade response: {deserEx.Message}");
                        return new BulkCreateGradesResponseDto { Errors = new List<string> { "Failed to parse response from server." } };
                    }
                }
                else
                {
                    // Handle other unexpected statuses (401, 403, 404, 500)
                    Debug.WriteLine($"Error submitting bulk grades for course {courseId}: {response.StatusCode}");
                    return new BulkCreateGradesResponseDto { Errors = new List<string> { $"API request failed with status {response.StatusCode}." } };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception submitting bulk grades for course {courseId}: {ex.Message}");
                return new BulkCreateGradesResponseDto { Errors = new List<string> { $"An exception occurred: {ex.Message}" } };
            }
        }
    }
}
