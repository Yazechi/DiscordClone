using DiscordClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Application.DTOs
{
    public record CreateGuildRequestDto(string Name, string? IconUrl);

    public record GuildResponseDto(
        Guid Id,
        string Name,
        string? IconUrl,
        Guid OwnerId,
        List<ChannelResponseDto> Channels
        );

    public record CreateChannelRequestDto(string Name, ChannelType Type); // O = Text, 1 = Voice

    public record ChannelResponseDto(
        Guid Id,
        Guid? GuildId,
        string Name,
        int Type,
        int Position
        );
}
