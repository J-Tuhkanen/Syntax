using System.ComponentModel.DataAnnotations.Schema;

namespace Syntax.Core.Models.Base
{
    public abstract class ActivityBase : EntityBase
    {
        public bool IsDeleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Timestamp { get; set; }
    }
}
