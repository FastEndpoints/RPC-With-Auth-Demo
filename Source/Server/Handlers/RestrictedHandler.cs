using System.Security.Claims;
using Contracts;
using FastEndpoints;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Server;

public sealed class RestrictedHandler :
    ICommandHandler<RestrictedCommand, string>,
    IHasServerCallContext //implement this interface to access the ServerCallContext
{
    public ServerCallContext ServerCallContext { get; set; } = default!;

    [Authorize(Roles = "app_user")] //makes this endpoint/handler require authorization
    public async Task<string> ExecuteAsync(RestrictedCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;
        var username = ServerCallContext.GetHttpContext().User.FindFirstValue("username");

        return $"Hello {username}! You are authorized to execute this handler!";
    }
}