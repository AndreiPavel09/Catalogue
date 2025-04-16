namespace Frontend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public UserRole Role { get; set; }
    }
}
