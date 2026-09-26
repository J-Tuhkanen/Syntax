using Syntax.Core.Models.Base;

namespace Syntax.Core.Models
{
    public class Blob : ActivityBase
    {
        public string Path { get; set; } = string.Empty;
    }
}