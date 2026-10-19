using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Entities;

using DiscordClone.Domain.Enums;
using System.Data;

public class Guild
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<GuildMember> Members { get; set; } = new List<GuildMember>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}