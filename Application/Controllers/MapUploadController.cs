using Application.Extensions.Attributes;
using Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Application.Controllers;

[UseApiKey]
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
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [SwaggerOperation(Summary = "Uploads a map image to the blob storage and returns a url.")]
    [SwaggerResponse(200, "Returns the imageurl.")]
    [SwaggerResponse(400, "The IFormFile was either containing invalid data or missing properties.")]
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
