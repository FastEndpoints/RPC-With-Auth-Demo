using System.Security.Cryptography.X509Certificates;
using Contracts;
using FastEndpoints;

var bld = WebApplication.CreateBuilder();
var app = bld.Build();

app.MapRemote(
    "https://localhost:6000",
    c =>
    {
        //validate the thumbprint of the self-signed cert of the server.
        //do not hardcode the thumbprint in clear text like this in production.
        ((SocketsHttpHandler)c.ChannelOptions.HttpHandler!).SslOptions = new()
        {
            RemoteCertificateValidationCallback =
                (_, cert, _, _) => cert is not null && ((X509Certificate2)cert).Thumbprint == "D0E968048B2EBAD4E3BD4B29D9311821A5AEAA22"
        };

        c.Register<SayHelloCommand>();
    });

//trigger the command by hitting this endpoint
app.MapGet(
    "/",
    async () =>
    {
        await new SayHelloCommand
            {
                From = "Mars"
            }
            .RemoteExecuteAsync();

        return Results.Ok();
    });

app.Run();