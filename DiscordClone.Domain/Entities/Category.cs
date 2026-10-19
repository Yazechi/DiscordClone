using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Entities;

using DiscordClone.Domain.Enums;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GuildId { get; set; }
    public Guild Guild { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }

    public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}