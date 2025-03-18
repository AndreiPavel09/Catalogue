using Backend.Models;

namespace Backend.Models
{
    public class Admin:User
    {
        public Admin()
        {
            Role = UserRole.Admin;
        }

        // Admin-specific properties can be added here if needed
    }
}
