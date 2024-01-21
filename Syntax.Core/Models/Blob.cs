using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Blob : EntityBase
    {
        public string Path { get; set; }
        public string UserId { get; set; }
    }
}