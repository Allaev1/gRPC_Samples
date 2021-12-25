using Grpc.Core;

namespace gRPC_Streaming.Services;

public class PinPongService : Chat.ChatBase
{
    public override async Task Start(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        Console.WriteLine("Enter y/Y for closing the chat");
        
        while (await requestStream.MoveNext())
        {
            if(requestStream.Current.Message.ToUpper() == "Y")
            {
                Console.WriteLine("Chat closed by client");
                return;
            }    

            Console.WriteLine();
            Console.WriteLine($"Client: {requestStream.Current.Message}");

            Console.Write("Message: ");
            var message = Console.ReadLine()!;

            await responseStream.WriteAsync(new Response { Message = message });
        }
    }
}
