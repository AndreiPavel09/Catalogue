namespace Backend.DTOs
{
    using System.ComponentModel.DataAnnotations;

    namespace Backend.DTOs
    {
        public class StudentDTO
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Password { get; set; }
        }

        public class CreateStudentDTO
        {

            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }
        }

        public class UpdateStudentDTO
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}
