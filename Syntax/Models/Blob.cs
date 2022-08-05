using Syntax.Models.Base;
using System;

namespace Syntax.Models
{
    public class Blob : EntityWithId
    {        
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; internal set; }
    }
}