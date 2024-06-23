using System;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models.Base
{
    public abstract class EntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
