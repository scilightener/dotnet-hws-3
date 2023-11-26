using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IQueryable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IQueryable<User>>(_dbContext.Users);
    }

    public Task<User?> GetUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dbContext.Users.FirstOrDefault(user => user.Id == guid));
    }

    public async Task DeleteUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        await _dbContext.Users.Where(user => user.Id == guid).ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users
            .Where(x => x.Id == user.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(x => x.Name, user.Name)
                .SetProperty(x => x.Email, user.Email), cancellationToken: cancellationToken);
    }

    public async Task<Guid> InsertUserAsync(User user, CancellationToken cancellationToken)
    {
        var trackedEntity = await _dbContext.Users.AddAsync(user, cancellationToken);
        return trackedEntity.Entity.Id;
    }
}