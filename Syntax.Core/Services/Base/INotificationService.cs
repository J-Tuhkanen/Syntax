using Syntax.Core.Dtos;

namespace Syntax.Core.Services.Base
{
    public interface INotificationService
    {
        Task SendCommentNotification(CommentDto comment, Guid topicId);
    }
}