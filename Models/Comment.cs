namespace SqlInjectionVulnerable.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }  // This will be used for linking to the logged-in user
        public DateTime CreatedAt { get; set; }
    }

}
