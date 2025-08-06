using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class TaskHub : Hub
    {
        public async Task JoinTaskGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "TaskUpdates");
        }

        public async Task LeaveTaskGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "TaskUpdates");
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "TaskUpdates");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "TaskUpdates");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
