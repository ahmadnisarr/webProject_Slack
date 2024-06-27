namespace WebProject.Models
{
    public class userProfile
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string UserName { get; set; }
        public string Fname { get; set; }    
        public string Lname { get; set; }
        public string OrganizationName { get; set; }
            
        public string country { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string lives { get; set; }

        public string department { get; set; }
        public string about {  get; set; }
        public string profilePath { get; set; }

    }
}
