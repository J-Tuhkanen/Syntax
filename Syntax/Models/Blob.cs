using System;

namespace Syntax.Models
{
    public class Blob
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; internal set; }
    }
}