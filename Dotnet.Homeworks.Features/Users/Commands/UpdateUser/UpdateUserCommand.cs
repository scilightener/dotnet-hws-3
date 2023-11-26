using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : ICommand, IClientRequest
{
    public User User { get; }
    
    public Guid Guid { get; }

    public UpdateUserCommand(User user)
    {
        Guid = user.Id;
        User = user;
    }

}