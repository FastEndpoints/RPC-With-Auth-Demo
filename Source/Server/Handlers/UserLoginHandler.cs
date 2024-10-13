using Contracts;
using FastEndpoints;
using Grpc.Core;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Server;

public sealed class UserLoginHandler : ICommandHandler<UserLoginCommand, string>
{
    public async Task<string> ExecuteAsync(UserLoginCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        if (command is { Username: "user", Password: "pass" }) //simplified condition for creating a jwt
        {
            return new JsonWebTokenHandler()
                .CreateToken(
                    new SecurityTokenDescriptor
                    {
                        IssuedAt = DateTime.UtcNow,
                        Subject = new( //specify claims to be embedded in token
                        [
                            new("username", command.Username),
                            new("role", "app_user")
                        ]),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new(
                            new SymmetricSecurityKey("a very very long jwt signing secret"u8.ToArray()),
                            SecurityAlgorithms.HmacSha256Signature)
                    });
        }

        throw new RpcException(new(StatusCode.InvalidArgument, "Invalid username or password!"));
    }
}