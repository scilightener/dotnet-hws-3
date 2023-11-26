using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminCommandHandler : ICommandHandler<DeleteUserByAdminCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public DeleteUserByAdminCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository
    )
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(DeleteUserByAdminCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken) is null)
        {
            return new Result(false, "No such user");
        }

        await _userRepository.DeleteUserByGuidAsync(request.Guid, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Result(true);
    }
}