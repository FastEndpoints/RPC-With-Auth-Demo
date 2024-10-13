using Contracts;
using FastEndpoints;

namespace Server;

public sealed class SayHelloHandler(ILogger<SayHelloHandler> logger) : ICommandHandler<SayHelloCommand>
{
    public Task ExecuteAsync(SayHelloCommand command, CancellationToken ct)
    {
        logger.LogInformation(">>>>>>>>>>>>>>>>> Hello from {from} <<<<<<<<<<<<<<<<<<", command.From);

        return Task.CompletedTask;
    }
}