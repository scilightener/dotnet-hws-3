using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersDto>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        if (!users.Any())
            return new Result<GetAllUsersDto>(null, false, "No users");

        var result = new GetAllUsersDto(users.Select(u => new GetUserDto(u.Id, u.Name, u.Email)));

        return new Result<GetAllUsersDto>(result, true);
    }
}