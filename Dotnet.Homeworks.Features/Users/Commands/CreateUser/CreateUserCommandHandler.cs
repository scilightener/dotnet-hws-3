using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>, ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IEnumerable<IValidator<CreateUserCommand>> validators,
        IPermissionCheck checker,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : base(validators, checker)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public new async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var resultDecorators = await base.Handle(request, cancellationToken);

        if (resultDecorators.IsFailure)
            return resultDecorators;

        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };
        var guid = await _userRepository.InsertUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new CreateUserDto(guid);

        return new Result<CreateUserDto>(result, true);
    }
}