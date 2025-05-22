using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Application.Service;

public class MapService(string connectionString, string container) : IMapService
{
    private readonly BlobContainerClient _containerClient = new(connectionString, container);

    public async Task<string> UploadMapAsync(IFormFile mapFile)
    {
        if (mapFile == null || mapFile.Length == 0) { return null!; }

        var fileExtension = Path.GetExtension(mapFile.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";

        string contentType = !string.IsNullOrEmpty(mapFile.ContentType)
            ? mapFile.ContentType : "application/octet-stream";

        if ((contentType == "application/octet-stream" || string.IsNullOrEmpty(contentType)) && fileExtension.Equals(".svg", StringComparison.OrdinalIgnoreCase)
            {
            contentType = "image/svg+xml";
            }

        BlobClient blobCLient = _containerClient.GetBlobClient(fileName);
        var uploadOptions = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } };

        using var stream = mapFile.OpenReadStream();
        await blobCLient.UploadAsync(stream, uploadOptions);

        return blobCLient.Uri.ToString();
    }
}
