using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evacuation.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Config.Interfaces;

public interface IDbContextFactory
{
    DbContext GetDbContext(DatabaseType dbType);
}
