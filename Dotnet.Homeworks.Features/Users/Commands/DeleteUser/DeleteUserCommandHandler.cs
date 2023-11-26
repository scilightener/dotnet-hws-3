using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : CqrsDecorator<DeleteUserCommand, Result>,
    ICommandHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IPermissionCheck permissionCheck,
        IEnumerable<IValidator<DeleteUserCommand>> validators
    ) : base(validators, permissionCheck)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public new async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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

            await _userRepository.DeleteUserByGuidAsync(request.Guid, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }

        return new Result(true);
    }
}