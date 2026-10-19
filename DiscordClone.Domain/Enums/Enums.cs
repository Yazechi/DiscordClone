using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Domain.Enums;

public enum UserStatus
{
    Offline = 0,
    Online = 1,
    Idle = 2,
    DoNotDisturb = 3,
    Invisible = 4
}

public enum ChannelType
{
    GuildText = 0,
    GuildVoice = 1,
    GuildCategory = 2,
    DirectMessage = 3,
    GroupDM = 4
}

[Flags]
public enum GuildPermissions : long
{
    None = 0,
    CreateInstantInvite = 1 << 0,
    KickMembers = 1 << 1,
    BanMembers = 1 << 2,
    Administrator = 1 << 3,
    ManageChannels = 1 << 4,
    ManageGuild = 1 << 5,
    SendMessages = 1 << 11,
    ReadMessageHistory = 1 << 16,
    Connect = 1 << 20,
    Speak = 1 << 21
}