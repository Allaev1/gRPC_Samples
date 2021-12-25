using Google.Protobuf;
using Grpc.Core;

namespace PhotoStreaming.Services;

public class PhotoStreamService : PhotoStream.PhotoStreamBase
{
    private const int dataLength = 1024;

    public async override Task GetImage(GetImageRequest request, IServerStreamWriter<GetImageResponse> responseStream, ServerCallContext context)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "Mountain.jpg");
        byte[] buffer = new byte[dataLength];
        int bytesCount = 1;

        using var fileStream = File.OpenRead(path);
        fileStream.Position = request.ByteOffset;

        while (bytesCount != 0)
        {
            bytesCount = await fileStream.ReadAsync(buffer, 0, dataLength);
            await responseStream.WriteAsync(new() { Data = ByteString.CopyFrom(buffer, 0, bytesCount) });
        }
    }
}
