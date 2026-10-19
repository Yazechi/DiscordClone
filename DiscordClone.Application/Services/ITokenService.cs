using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordClone.Domain.Entities;

namespace DiscordClone.Application.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}