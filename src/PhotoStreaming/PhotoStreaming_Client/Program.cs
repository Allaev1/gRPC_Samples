using Grpc.Core;
using Grpc.Net.Client;
using PhotoStreaming;

var channel = GrpcChannel.ForAddress("https://localhost:7096");
var client = new PhotoStream.PhotoStreamClient(channel);

string path = @"C:\Users\Dev4\OneDrive\Рабочий стол\images\Mountain.jpg";
using var fileStream = File.Open(path, FileMode.Append);
using var stream = client.GetImage(new GetImageRequest { ByteOffset = fileStream.Position });

try
{
    await foreach (var response in stream.ResponseStream.ReadAllAsync())
        fileStream.Write(response.Data.ToByteArray(), 0, response.Data.Length);
}
catch (RpcException)
{

}
finally
{

}