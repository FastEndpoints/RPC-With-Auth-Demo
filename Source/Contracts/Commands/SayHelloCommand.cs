using FastEndpoints;

namespace Contracts;

public class SayHelloCommand : ICommand
{
    public string From { get; set; }
}