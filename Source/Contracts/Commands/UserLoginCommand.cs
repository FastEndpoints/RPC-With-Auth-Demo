using FastEndpoints;

namespace Contracts;

public class UserLoginCommand : ICommand<string>
{
    public string Username { get; set; }
    public string Password { get; set; }
}