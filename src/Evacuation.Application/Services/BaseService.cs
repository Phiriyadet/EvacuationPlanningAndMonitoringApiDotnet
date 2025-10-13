using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Repositories.Factory.Interfaces;

namespace Evacuation.Application.Services.Interfaces;

public abstract class BaseService
{
    protected readonly IRepositoryFactory _repoFactory;
    protected readonly DatabaseType _dbType;

    protected BaseService(IRepositoryFactory repoFactory, DatabaseType dbType)
    {
        _repoFactory = repoFactory;
        _dbType = dbType;
    }
}

