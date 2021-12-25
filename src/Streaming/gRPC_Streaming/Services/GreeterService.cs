using Grpc.Core;
using gRPC_Streaming;

namespace gRPC_Streaming.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public async override Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(1000);

            await responseStream.WriteAsync(new HelloReply() { Message = "Hello from server" });
        }
    }
}
