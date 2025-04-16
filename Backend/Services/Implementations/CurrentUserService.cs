using Backend.Models;

namespace Backend.Services.Implementations
{
    public class CurrentUserService
    {
        public User CurrentUser { private get; set; }

        public int GetUserId()
        {
            Console.WriteLine(CurrentUser.Username);
            if(CurrentUser==null)
            {
                return 0;
            }
            return CurrentUser.Id;
        }
    }
}
