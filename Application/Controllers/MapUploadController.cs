using Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapUploadController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapUploadController(IMapService mapService)
    {
        _mapService = mapService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadMapImage(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0) { return BadRequest("Image is invalid."); }

        try
        {
            var imageUrl = await _mapService.UploadMapAsync(formFile);
            if (string.IsNullOrEmpty(imageUrl)) { return BadRequest("Something went wrong in the service when trying to upload image."); }

            return Ok(imageUrl);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
