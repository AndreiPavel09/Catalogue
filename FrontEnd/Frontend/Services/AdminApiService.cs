// FrontendAdmin/Services/AdminApiService.cs
using Frontend.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Might require NuGet Package
using System.Threading.Tasks;
using System.Diagnostics;
using Frontend.Services;

namespace Frontend.Services // Ensure namespace is correct
{
    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBasePath = "api/admin";

        public AdminApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // --- Teacher Management ---
        public async Task<List<UserDto>?> GetTeachersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/teachers");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return new List<UserDto>();
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>();
                }
                // Basic error logging
                Debug.WriteLine($"Error fetching teachers: {response.StatusCode}");
                Console.WriteLine($"Error fetching teachers: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching teachers: {ex.Message}");
                Console.WriteLine($"Exception fetching teachers: {ex.Message}");
                return null;
            }
        }

        public async Task<UserDto?> AddTeacherAsync(CreateUserDto teacherDto)
        {
            if (string.IsNullOrWhiteSpace(teacherDto.Role) || !teacherDto.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                teacherDto.Role = "Teacher"; // Ensure role
            }
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/teachers", teacherDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                Debug.WriteLine($"Error adding teacher: {response.StatusCode}");
                Console.WriteLine($"Error adding teacher: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception adding teacher: {ex.Message}");
                Console.WriteLine($"Exception adding teacher: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteTeacherAsync(int teacherId) // Return tuple
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/teachers/{teacherId}");

                if (response.IsSuccessStatusCode) // Handles 204 No Content
                {
                    return (true, null); // Success, no error message
                }
                else
                {
                    // Attempt to read the error message from the response body
                    string errorMessage = "An unknown error occurred."; // Default message
                    if (response.Content != null)
                    {
                        try
                        {
                            // Assuming the backend returns JSON like { "message": "Error text" }
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                // Fallback if JSON doesn't match or is empty
                                errorMessage = await response.Content.ReadAsStringAsync();
                                if (string.IsNullOrWhiteSpace(errorMessage)) // Handle empty response body
                                {
                                    errorMessage = $"API returned status code {response.StatusCode}.";
                                }
                            }
                        }
                        catch // Handle cases where response is not JSON or reading fails
                        {
                            errorMessage = $"API returned status code {response.StatusCode}. Could not read error details.";
                        }
                    }

                    Debug.WriteLine($"Error deleting teacher {teacherId}: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error deleting teacher {teacherId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage); // Failure, return the message
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception deleting teacher {teacherId}: {ex.Message}");
                Console.WriteLine($"Exception deleting teacher {teacherId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}"); // Failure due to exception
            }
        }

        // Helper class to deserialize backend error responses
        private class ErrorResponse
        {
            public string? Message { get; set; }
        }

        public async Task<List<CourseDto>?> GetCoursesAsync()
        {
            try
            {
                // GET api/admin/courses
                var response = await _httpClient.GetAsync($"{ApiBasePath}/courses");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return new List<CourseDto>();
                    }
                    // Optional: Enhance this to include Teacher Name if needed
                    // You might need a different backend endpoint or modify the existing one
                    // to return CourseDto including teacher details.
                    return await response.Content.ReadFromJsonAsync<List<CourseDto>>();
                }
                else
                {
                    Debug.WriteLine($"Error fetching courses: {response.StatusCode}");
                    Console.WriteLine($"Error fetching courses: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching courses: {ex.Message}");
                Console.WriteLine($"Exception fetching courses: {ex.Message}");
                return null;
            }
        }

        public async Task<CourseDto?> AddCourseAsync(CreateCourseDto courseDto)
        {
            try
            {
                // POST api/admin/courses
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/courses", courseDto);
                if (response.IsSuccessStatusCode) // Expect 201 Created
                {
                    return await response.Content.ReadFromJsonAsync<CourseDto>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error adding course: {response.StatusCode} - {errorContent}");
                    Console.WriteLine($"Error adding course: {response.StatusCode} - {errorContent}");
                    // Consider returning the error message if needed by the UI
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception adding course: {ex.Message}");
                Console.WriteLine($"Exception adding course: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteCourseAsync(int courseId)
        {
            try
            {
                // DELETE api/admin/courses/{courseId}
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/courses/{courseId}");

                if (response.IsSuccessStatusCode) // Expect 204 No Content
                {
                    return (true, null);
                }
                else
                {
                    // Try to get error message from backend
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Using helper class from previous step
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText;
                            }
                        }
                        catch { /* Ignore if content is not expected JSON */ }
                    }

                    // Check specifically for 404 which means "Not Found"
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        errorMessage = $"Course with ID {courseId} not found.";
                    }
                    // Add checks for other specific errors if the backend returns them (e.g., 400 Bad Request for dependencies)

                    Debug.WriteLine($"Error deleting course {courseId}: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error deleting course {courseId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception deleting course {courseId}: {ex.Message}");
                Console.WriteLine($"Exception deleting course {courseId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<List<UserDto>?> GetStudentsAsync()
        {
            // GET api/admin/students (This endpoint returns UserDto based on your AdminController)
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/students");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return new List<UserDto>();
                    }
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>();
                }
                else
                {
                    Debug.WriteLine($"Error fetching students: {response.StatusCode}");
                    Console.WriteLine($"Error fetching students: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching students: {ex.Message}");
                Console.WriteLine($"Exception fetching students: {ex.Message}");
                return null;
            }
        }


        // --- Grade Management ---

        public async Task<List<GradeDto>?> GetGradesAsync()
        {
            // GET api/admin/grades
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBasePath}/grades");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return new List<GradeDto>();
                    }
                    return await response.Content.ReadFromJsonAsync<List<GradeDto>>();
                }
                else
                {
                    Debug.WriteLine($"Error fetching grades: {response.StatusCode}");
                    Console.WriteLine($"Error fetching grades: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching grades: {ex.Message}");
                Console.WriteLine($"Exception fetching grades: {ex.Message}");
                return null;
            }
        }

        public async Task<GradeDto?> AddGradeAsync(CreateGradeDto gradeDto)
        {
            // POST api/admin/grades
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/grades", gradeDto);
                if (response.IsSuccessStatusCode) // Expect 201 Created
                {
                    return await response.Content.ReadFromJsonAsync<GradeDto>();
                }
                else
                {
                    // Attempt to read specific error (e.g., student/course not found, invalid value)
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Using helper class
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText;
                            }
                        }
                        catch { /* Ignore if content is not expected JSON */ }
                    }
                    Debug.WriteLine($"Error adding grade: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error adding grade: {response.StatusCode} - {errorMessage}");
                    // You might want the AddGrade method in the Razor page to show this errorMessage
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception adding grade: {ex.Message}");
                Console.WriteLine($"Exception adding grade: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteGradeAsync(int gradeId)
        {
            // DELETE api/admin/grades/{gradeId}
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/grades/{gradeId}");

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
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Using helper class
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText;
                            }
                        }
                        catch { /* Ignore */ }
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        errorMessage = $"Grade with ID {gradeId} not found.";
                    }

                    Debug.WriteLine($"Error deleting grade {gradeId}: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error deleting grade {gradeId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception deleting grade {gradeId}: {ex.Message}");
                Console.WriteLine($"Exception deleting grade {gradeId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<UserDto?> AddStudentAsync(CreateUserDto studentDto)
        {
            // Ensure the role is set correctly before sending
            if (string.IsNullOrWhiteSpace(studentDto.Role) || !studentDto.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                studentDto.Role = "Student"; // Force the role for this method
            }

            try
            {
                // POST api/admin/students
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/students", studentDto);

                if (response.IsSuccessStatusCode) // Expect 201 Created
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                else
                {
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Using helper class
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText; // e.g., "Username already exists" might be plain text
                            }
                        }
                        catch { /* Ignore */ }
                    }
                    Debug.WriteLine($"Error adding student: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error adding student: {response.StatusCode} - {errorMessage}");
                    // Consider returning the error message if needed by the UI
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception adding student: {ex.Message}");
                Console.WriteLine($"Exception adding student: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteStudentAsync(int studentId)
        {
            try
            {
                // DELETE api/admin/students/{studentId}
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/students/{studentId}");

                if (response.IsSuccessStatusCode) // Expect 204 No Content
                {
                    return (true, null);
                }
                else
                {
                    // Try to get error message from backend
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>(); // Using helper class
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText;
                            }
                        }
                        catch { /* Ignore */ }
                    }

                    // Check specifically for 404 which means "Not Found"
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        errorMessage = $"Student with ID {studentId} not found.";
                    }
                    // Check for errors related to dependencies if backend uses Restrict (e.g., if deleting student was restricted by Grades or StudentCourses)
                    // Based on your backend schema (Grade.StudentId -> Restrict, StudentCourse.StudentId -> Restrict), deletes *will fail* if dependencies exist.
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // Or maybe 500 depending on how backend handles constraint violations
                    {
                        // Provide a more helpful message if possible based on backend response
                        errorMessage += " This student might have existing grades or course enrollments that prevent deletion.";
                    }


                    Debug.WriteLine($"Error deleting student {studentId}: {response.StatusCode} - {errorMessage}");
                    Console.WriteLine($"Error deleting student {studentId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception deleting student {studentId}: {ex.Message}");
                Console.WriteLine($"Exception deleting student {studentId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<GradeDto?> GetGradeByIdAsync(int gradeId)
        {
            // Reuse the existing GetGradesAsync and filter client-side
            // Inefficient for many grades, but avoids adding another backend endpoint for now
            // Assumes GetGradesAsync returns the DTO with StudentName/CourseName populated
            try
            {
                var grades = await GetGradesAsync();
                return grades?.FirstOrDefault(g => g.Id == gradeId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching grade {gradeId}: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateGradeAsync(int gradeId, UpdateGradeDto gradeDto)
        {
            try
            {
                // PUT api/admin/grades/{gradeId}
                var response = await _httpClient.PutAsJsonAsync($"{ApiBasePath}/grades/{gradeId}", gradeDto);

                if (response.IsSuccessStatusCode) // Expect 204 No Content
                {
                    return (true, null);
                }
                else
                {
                    // Handle errors (Not Found, Bad Request, Conflict, Server Error)
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                            {
                                errorMessage = errorResponse.Message;
                            }
                            else
                            {
                                var plainText = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(plainText)) errorMessage = plainText;
                            }
                        }
                        catch { /* Ignore */ }
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) errorMessage = "Grade not found.";
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) errorMessage += " Invalid grade value provided.";


                    Debug.WriteLine($"Error updating grade {gradeId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception updating grade {gradeId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<List<EnrollmentDto>?> GetEnrollmentsAsync()
        {
            try
            {
                // GET api/admin/enrollments
                var response = await _httpClient.GetAsync($"{ApiBasePath}/enrollments");
                response.EnsureSuccessStatusCode(); // Throw exception for non-2xx codes
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return new List<EnrollmentDto>();
                }
                return await response.Content.ReadFromJsonAsync<List<EnrollmentDto>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception fetching enrollments: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> EnrollStudentAsync(EnrollmentDto enrollmentDto)
        {
            try
            {
                // POST api/admin/enrollments
                var response = await _httpClient.PostAsJsonAsync($"{ApiBasePath}/enrollments", enrollmentDto);

                if (response.IsSuccessStatusCode) // Expect 200 OK or 204 No Content maybe
                {
                    return (true, null);
                }
                else
                {
                    string errorMessage = $"API returned status code {response.StatusCode}.";
                    // Extract message from body (assuming { message: "..."})
                    if (response.Content != null)
                    {
                        try
                        {
                            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                            if (!string.IsNullOrWhiteSpace(errorResponse?.Message)) errorMessage = errorResponse.Message;
                        }
                        catch { /* Ignore */ }
                    }
                    Debug.WriteLine($"Error enrolling student: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception enrolling student: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UnenrollStudentAsync(int studentId, int courseId)
        {
            try
            {
                // DELETE api/admin/enrollments?studentId={studentId}&courseId={courseId}
                var response = await _httpClient.DeleteAsync($"{ApiBasePath}/enrollments?studentId={studentId}&courseId={courseId}");

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
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) errorMessage = "Enrollment not found.";

                    Debug.WriteLine($"Error unenrolling student {studentId} from course {courseId}: {response.StatusCode} - {errorMessage}");
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception unenrolling student {studentId}: {ex.Message}");
                return (false, $"An exception occurred: {ex.Message}");
            }
        }
    }
}