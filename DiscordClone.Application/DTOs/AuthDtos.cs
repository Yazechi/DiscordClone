using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Application.DTOs;

public record RegisterRequestDto(
    string Username,
    string Email,
    string Password,
    string DisplayName
);

public record LoginRequestDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    Guid UserId,
    string Username,
    string Email,
    string Token
);