using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Blob : EntityWithId
    {
        public string Path { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
    }
}