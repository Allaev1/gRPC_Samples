using Grpc.Core;
using Grpc.Net.Client;
using gRPC_Streaming;

var channel = GrpcChannel.ForAddress("https://localhost:7266");
var client = new Greeter.GreeterClient(channel);

using var stream = client.SayHello(new HelloRequest { Name = "Hello" });

try
{
    await foreach (var response in stream.ResponseStream.ReadAllAsync())
        Console.WriteLine(response.Message);
}
catch (RpcException ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Goodbye");
}

//Important: When `Credentials = ChannelCredentials.Insecure` you should use HTTP not HTTPS because HTTPS require secured connection
var chatChannel = GrpcChannel.ForAddress("http://localhost:5266", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
var chatClient = new Chat.ChatClient(chatChannel);

using var chatStream = chatClient.Start();

Console.WriteLine("Start");
Console.WriteLine("Enter y/Y for closing the chat");
await chatStream.RequestStream.WriteAsync(new Request { Message = "First message from client" });

while (await chatStream.ResponseStream.MoveNext())
{
    if (chatStream.ResponseStream.Current.Message.ToUpper() == "Y")
    {
        Console.WriteLine("Chat closed by server");
        await chatStream.RequestStream.CompleteAsync();

        Console.ReadLine();
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"Server: {chatStream.ResponseStream.Current.Message}");

    Console.Write("Message:");
    var message = Console.ReadLine()!;
    if (message.ToUpper() == "Y")
    {
        Console.WriteLine("Chat closed by client");
        await chatStream.RequestStream.WriteAsync(new Request { Message = "Y" });
        await chatStream.RequestStream.CompleteAsync();

        Console.ReadLine();
        return;
    }

    await chatStream.RequestStream.WriteAsync(new Request { Message = message });
}