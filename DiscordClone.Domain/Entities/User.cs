using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordClone.Domain.Enums;


namespace DiscordClone.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Offline;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GuildMember> GuildMemberships { get; set; } = new List<GuildMember>();
}