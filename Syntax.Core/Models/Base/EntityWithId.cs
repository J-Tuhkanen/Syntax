using System;

namespace Syntax.Core.Models.Base
{
    public abstract class EntityWithId
    {
        public string Id { get; set; }

        public EntityWithId()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
