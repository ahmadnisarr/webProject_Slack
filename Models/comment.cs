namespace WebProject.Models
{
    public class comment
    {
        public int Id { get; set; }
        public int postId { get; set; }
        public string userId { get; set; }
        public string commentContent { get; set; }
        public DateTime commentDate { get; set; }

    }
}
