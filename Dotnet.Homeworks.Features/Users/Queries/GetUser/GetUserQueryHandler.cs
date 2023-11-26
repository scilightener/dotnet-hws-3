using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : CqrsDecorator<GetUserQuery, Result<GetUserDto>>,
    IQueryHandler<GetUserQuery, GetUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(
        IUserRepository userRepository,
        IEnumerable<IValidator<GetUserQuery>> validators,
        IPermissionCheck checkers
    )
        : base(validators, checkers)
    {
        _userRepository = userRepository;
    }

    public new async Task<Result<GetUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var resultDecorators = await base.Handle(request, cancellationToken);
        if (resultDecorators.IsFailure)
            return resultDecorators;

        var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
        if (user is null)
            return new Result<GetUserDto>(null, false, "No such user exists");

        var result = new GetUserDto(user.Id, user.Name, user.Email);

        return new Result<GetUserDto>(result, true);
    }
}