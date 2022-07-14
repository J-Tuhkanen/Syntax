namespace Syntax.Models
{
    public class Post
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsDeleted { get; set; }
    }
}
