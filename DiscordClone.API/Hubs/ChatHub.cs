using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DiscordClone.Infrastructure.Data;
using DiscordClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace DiscordClone.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IServiceScopeFactory _scopeFactory;
    public ChatHub(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public async Task JoinChannel(string channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task LeaveChannel(string channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task SendMessageToChannel(string channelIdStr, string user, string message)
    {
        // 1. Broadcast real-time message to active channel group
        await Clients.Group(channelIdStr).SendAsync("ReceiveMessage", user, message);

        // 2. Parse exact channel GUID sent by client
        if (!Guid.TryParse(channelIdStr, out var channelGuid) || channelGuid == Guid.Empty) return;

        var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdStr, out var authorGuid))
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var chatMessage = new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = channelGuid,
                AuthorId = authorGuid,
                Content = message,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Messages.Add(chatMessage);
            await dbContext.SaveChangesAsync();
        }
    }
    public async Task SendTypingIndicator(string channelId, string user)
    {
        await Clients.OthersInGroup(channelId).SendAsync("UserTyping", user);
    }
}