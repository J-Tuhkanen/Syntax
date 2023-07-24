using System;

namespace Syntax.Core.Models.Base
{
    public interface IUserActivity
    {
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
