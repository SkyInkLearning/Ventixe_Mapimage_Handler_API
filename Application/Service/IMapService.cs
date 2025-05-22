
namespace Application.Service
{
    public interface IMapService
    {
        Task<string> UploadMapAsync(IFormFile mapFile);
    }
}