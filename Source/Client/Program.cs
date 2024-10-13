//using Grpc.Core;

using System.Security.Cryptography.X509Certificates;
using Contracts;
using FastEndpoints;
using Grpc.Core;

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

        // c.ChannelOptions.Credentials = ChannelCredentials.Create(
        //     new SslCredentials(),
        //     CallCredentials.FromInterceptor(
        //         async (ctx, metadata) =>
        //         {
        //             await Task.CompletedTask;
        //             var jwtToken = "token should be retrieved from storage";
        //             metadata.Add("Authorization", $"Bearer {jwtToken}");
        //         }));

        c.Register<SayHelloCommand>();
        c.Register<UserLoginCommand, string>();
        c.Register<RestrictedCommand, string>();
    });

//trigger the command initiations by hitting the following endpoints

app.MapGet(
    "/",
    async () =>
    {
        await new SayHelloCommand { From = "Mars" }.RemoteExecuteAsync();

        return Results.Ok("see console output of the server to see the result...");
    });

app.MapGet(
    "/protected",
    async () =>
    {
        try
        {
            //obtain a JWT access token by providing login credentials.
            //this is just demonstrating retrieval of the token. do not retrieve tokens on every call.
            //store the retrieved token on the client once and reuse until it expires.
            var jwt = await new UserLoginCommand { Username = "user", Password = "pass" }.RemoteExecuteAsync();

            //use the jwt in order to access a protected grpc endpoint/command handler
            var result = await new RestrictedCommand()
                             .RemoteExecuteAsync(
                                 new(
                                     new()
                                     {
                                         { "Authorization", $"Bearer {jwt}" }
                                     }));

            return Results.Ok($"result: {result}");
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
        {
            return Results.BadRequest("The supplied credentials are invalid!");
        }
    });

app.Run();