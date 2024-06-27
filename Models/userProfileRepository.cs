using Microsoft.Data.SqlClient;
using System.IO;

namespace WebProject.Models
{
    public class userProfileRepository(string connectionString) : GenericRepository<messages>(connectionString)
    {
        private string connString = connectionString;
        public void add(userProfile UserProfile)
        {
            if (UserProfile != null) {

                IRepository<userProfile> repo = new GenericRepository<userProfile>(connectionString);
                repo.Update(UserProfile);
            }
        }
        public userProfile UserDeatils(int Id)
        {
            IRepository<userProfile> repo = new GenericRepository<userProfile>(connectionString);
            userProfile user=repo.GetById(Id);
            return user;
        }

        public List<userProfile> GetAll()
        {
            IRepository<userProfile> repo = new GenericRepository<userProfile>(connectionString);
            List<userProfile> user = repo.GetAll().ToList();
            return user;
        }
        
    }
}
