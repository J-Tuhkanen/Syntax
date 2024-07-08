using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Syntax.Core.Dtos;

namespace Syntax.Core.Hubs
{
    public class NotificationHub : Hub
    {
        public Task NotifyAll(CommentDto comment) 
            => Clients.All.SendAsync("messageReceived", comment);

        public override async Task OnConnectedAsync()
        {
            object? topicIdRouteValue = Context.GetHttpContext()?.GetRouteValue("topicId");

            if(topicIdRouteValue is string topicId && string.IsNullOrWhiteSpace(topicId) == false)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, topicId);
                await base.OnConnectedAsync();
            }
        }
    }
}
