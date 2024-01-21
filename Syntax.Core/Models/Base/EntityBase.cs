using System;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models.Base
{
    public abstract class EntityBase

    {
        [Key]
        [Required]
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }

        public EntityBase()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
