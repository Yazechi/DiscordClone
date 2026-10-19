using DiscordClone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Entities
{
    public class Channel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? GuildId { get; set; }
        public Guild? Guild { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public string Name { get; set; } = string.Empty;
        public ChannelType Type { get; set; } = ChannelType.GuildText;
        public int Position { get; set; }
        public string? Topic { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}