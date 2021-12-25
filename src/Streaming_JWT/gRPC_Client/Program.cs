using Grpc.Core;
using Grpc.Net.Client;
using gRPC_Streaming;

using var httpClient = new HttpClient();
var request = new HttpRequestMessage
{
    RequestUri = new Uri($"https://localhost:7266/generateJwtToken"),
    Method = HttpMethod.Get,
    Version = new Version(2, 0)
};
var tokenResponse = await httpClient.SendAsync(request);
tokenResponse.EnsureSuccessStatusCode();

var token = await tokenResponse.Content.ReadAsStringAsync();

var headers = new Metadata();
headers.Add("Authorization", $"Bearer {token}");

var channel = GrpcChannel.ForAddress("https://localhost:7266");
var client = new Greeter.GreeterClient(channel);

using var stream = client.SayHello(new HelloRequest { Name = "Hello" }, headers: headers);

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

Console.ReadLine();
