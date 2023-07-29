using System;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models.Base
{
    public abstract class EntityWithId
    {
        [Key]
        [Required]
        public string Id { get; set; }

        public EntityWithId()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
