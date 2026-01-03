using System.ComponentModel.DataAnnotations.Schema;

namespace Syntax.Core.Models.Base
{
    public abstract class EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}
