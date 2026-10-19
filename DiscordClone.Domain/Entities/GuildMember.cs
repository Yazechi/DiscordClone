using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Entities
{
    public class GuildMember
    {
        public Guid GuildId { get; set; }
        public Guild Guild { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string? Nickname { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Roles assigned to this member
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
