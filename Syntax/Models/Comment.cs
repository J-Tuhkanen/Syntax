namespace Syntax.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
    }
}
