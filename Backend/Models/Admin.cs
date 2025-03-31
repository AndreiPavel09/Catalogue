namespace Backend.Models;

public class Admin : User
{
    public Admin()
    {
        UserRole = UserRole.Admin;
    }

    // Admin-specific properties can be added here if needed
}
