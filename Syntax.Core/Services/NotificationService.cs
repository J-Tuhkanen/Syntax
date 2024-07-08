using Microsoft.AspNetCore.SignalR;
using Syntax.Core.Dtos;
using Syntax.Core.Hubs;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public NotificationService(IHubContext<NotificationHub> notificationHubContext)
        {
            _notificationHubContext = notificationHubContext;
        }

        public async Task SendCommentNotification(CommentDto comment, Guid topicId)
        {
            var clientsOfGroup = _notificationHubContext.Clients.Group(topicId.ToString());

            await clientsOfGroup.SendAsync("messageReceived", comment);
        }
    }
}
