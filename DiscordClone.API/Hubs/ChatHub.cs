using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DiscordClone.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinChannel(string channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task LeaveChannel(string channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task SendMessageToChannel(string channelId, string user, string message)
    {
        // Broadcast user and message to all clients listening in that channel group
        await Clients.Group(channelId).SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendTypingIndicator(string channelId, string user)
    {
        await Clients.OthersInGroup(channelId).SendAsync("UserTyping", user);
    }
}