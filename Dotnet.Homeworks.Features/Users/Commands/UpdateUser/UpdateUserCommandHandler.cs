using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : CqrsDecorator<UpdateUserCommand, Result>, ICommandHandler<UpdateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IPermissionCheck permissionCheck,
        IEnumerable<IValidator<UpdateUserCommand>> validators
    ) : base(validators, permissionCheck)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public new async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var resultDecorators = await base.Handle(request, cancellationToken);

        if (resultDecorators.IsFailure)
            return resultDecorators;

        try
        {
            if (await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken) is null)
            {
                return new Result(false, "No such user");
            }

            await _userRepository.UpdateUserAsync(request.User, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }

        return new Result(true);
    }
}