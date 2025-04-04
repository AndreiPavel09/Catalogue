using FrontEnd.DTOs;

namespace FrontEnd.Extensions
{
    public static class UserDtoExtensions
    {
        // Extension method to get the full name from a UserDto
        public static string FullName(this UserDto? user) // Make parameter nullable
        {
            // Handle null user object gracefully
            if (user == null)
            {
                return string.Empty; // Or return "N/A", or throw exception, depending on desired behavior
            }
            return $"{user.FirstName} {user.LastName}";
        }
    }
}
