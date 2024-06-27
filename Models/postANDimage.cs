namespace WebProject.Models
{
    public class postANDimage
    {
        public Posting posting { get; set; }
        public List<string> imagePath { get; set; }
        public string profilePath { get; set; }
        public string userName { get; set; }

        public postANDimage()
        {
            // Initialize the imagePath list
            imagePath = new List<string>();
        }

    }
}
