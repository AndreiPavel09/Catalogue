﻿namespace FrontEnd.DTOs
{

        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty; // e.g., "Student", "Teacher"
        }
}
