using System.Security.Claims;
using DiscordClone.Application.DTOs;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;
using DiscordClone.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class GuildsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GuildsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserGuilds()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var guilds = await _context.GuildMembers
            .Where(gm => gm.UserId == userId)
            .Select(gm => gm.Guild)
            .Select(g => new GuildResponseDto(
                g.Id,
                g.Name,
                g.IconUrl,
                g.OwnerId,
                g.Channels.Select(c => new ChannelResponseDto(c.Id, c.GuildId, c.Name, (int)c.Type, c.Position)).ToList()
                ))
            .ToListAsync();

        return Ok(guilds);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGuild([FromBody] CreateGuildRequestDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            IconUrl = dto.IconUrl,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var defaultChannel = new Channel
        {
            Id = Guid.NewGuid(),
            GuildId = guild.Id,
            Name = "general",
            Type = 0,
            Position = 0
        };

        // Add Creator as owner member
        var member = new GuildMember
        {
            GuildId = guild.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
        };

        _context.Guilds.Add(guild);
        _context.Channels.Add(defaultChannel);
        _context.GuildMembers.Add(member);
        await _context.SaveChangesAsync();

        var response = new GuildResponseDto(
            guild.Id,
            guild.Name,
            guild.IconUrl,
            guild.OwnerId,
            new List<ChannelResponseDto>
            {
                new(defaultChannel.Id, defaultChannel.GuildId, defaultChannel.Name, (int)defaultChannel.Type, defaultChannel.Position)
            }
            );
        return Ok(response);
    }

    [HttpPost("{guildId}/channels")]
    public async Task<IActionResult> CreateChannel(Guid guildId, [FromBody] CreateChannelRequestDto dto)
    {
        var guild = await _context.Guilds.FindAsync(guildId);
        if (guild == null) return NotFound("Guild not found.");

        var channel = new Channel
        {
            Id = Guid.NewGuid(),
            GuildId = guildId,
            Name = dto.Name.ToLower().Replace(" ", "-"),
            Type = (ChannelType)dto.Type,
            Position = await _context.Channels.CountAsync(c => c.GuildId == guildId)
        };

        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        return Ok(new ChannelResponseDto(channel.Id, channel.GuildId, channel.Name, (int)channel.Type, channel.Position));
    }
}
