using Syntax.Core.Models;
using Syntax.Core.Wrappers.Base;

namespace Syntax.Core.Wrappers
{
    public class PostWrapper : WrapperBase<Post>
    {
        private int _maxBodyLengthAsShortened = 80;

        public string DisplayBody { get; }

        public PostWrapper(Post post) : base(post)
        {
            DisplayBody = post.Body.Length > _maxBodyLengthAsShortened + 3
                ? post.Body.Substring(0, _maxBodyLengthAsShortened) + "..."
                : post.Body;
        }
    }
}
