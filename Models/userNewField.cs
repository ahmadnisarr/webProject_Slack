using Microsoft.AspNetCore.Identity;

namespace WebProject.Models
{
    public class userNewField:IdentityUser
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public DateTime registrationDate { get; set;}
        public bool isNewUser { get; set; }
    }
}
