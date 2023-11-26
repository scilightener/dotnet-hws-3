using Dotnet.Homeworks.Mailing.API.Dto;
using Dotnet.Homeworks.Mailing.API.Services;
using Dotnet.Homeworks.Shared.MessagingContracts.Email;
using MassTransit;

namespace Dotnet.Homeworks.Mailing.API.Consumers;

public class EmailConsumer : IEmailConsumer
{

    private readonly IMailingService _mailingService;

    public EmailConsumer(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    public async Task Consume(ConsumeContext<SendEmail> context)
    {
        await _mailingService.SendEmailAsync(new EmailMessage(context.Message.ReceiverEmail, context.Message.Subject,
            context.Message.Content));
    }
}