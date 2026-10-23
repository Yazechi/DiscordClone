using DiscordClone.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChannelsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChannelsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{channelId}/messages")]
    public async Task<IActionResult> GetChannelMessages(Guid channelId, [FromQuery] int limit = 50)
    {
        var messages = await _context.Messages
            .Where(m => m.ChannelId == channelId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .Select(m => new
            {
                m.Id,
                Author = m.Author != null ? m.Author.Username : "Unknown",
                m.Content,
                m.CreatedAt
            })
            .ToListAsync();

        messages.Reverse();
        return Ok(messages);
    }
}