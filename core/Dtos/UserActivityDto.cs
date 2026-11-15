using Syntax.Core.Dtos;

namespace Syntax.API.Controllers
{
    public record UserActivityDto(
        IEnumerable<CommentDto> Comments, 
        IEnumerable<TopicDto> Topics);
}
