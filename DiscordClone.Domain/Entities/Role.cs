using DiscordClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GuildId { get; set; }
        public Guild Guild { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#99AAB5"; // Default Discord Gray
        public int Position { get; set; }
        public GuildPermissions Permissions { get; set; }

        public ICollection<GuildMember> Members { get; set; } = new List<GuildMember>();
    }
}
