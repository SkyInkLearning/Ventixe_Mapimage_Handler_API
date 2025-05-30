using Application.Controllers;
using Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatGptTests;

public class ControllerTests
{
    private readonly Mock<IMapService> _mapServiceMock;
    private readonly MapUploadController _controller;

    public ControllerTests()
    {
        _mapServiceMock = new Mock<IMapService>();
        _controller = new MapUploadController(_mapServiceMock.Object);
    }

    [Fact]
    public async Task UploadMapImage_ReturnsBadRequest_WhenFormFileIsNull()
    {
        var result = await _controller.UploadMapImage(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Image is invalid.", badRequest.Value);
    }

    [Fact]
    public async Task UploadMapImage_ReturnsBadRequest_WhenFormFileIsEmpty()
    {
        var emptyStream = new MemoryStream();
        var file = new FormFile(emptyStream, 0, 0, "file", "empty.png");
        var result = await _controller.UploadMapImage(file);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Image is invalid.", badRequest.Value);
    }

    [Fact]
    public async Task UploadMapImage_ReturnsOk_WithUrl_WhenServiceSucceeds()
    {
        var content = new byte[] { 1, 2, 3 };
        var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, content.Length, "file", "map.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        _mapServiceMock
            .Setup(s => s.UploadMapAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("https://example.com/map.png");

        var result = await _controller.UploadMapImage(file);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("https://example.com/map.png", ok.Value);
    }

    [Fact]
    public async Task UploadMapImage_ReturnsBadRequest_WhenServiceReturnsEmpty()
    {
        var content = new byte[] { 1 };
        var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, content.Length, "file", "map.svg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/svg+xml"
        };

        _mapServiceMock
            .Setup(s => s.UploadMapAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync(string.Empty);

        var result = await _controller.UploadMapImage(file);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Something went wrong in the service when trying to upload image.", badRequest.Value);
    }

    [Fact]
    public async Task UploadMapImage_ReturnsBadRequest_WhenServiceThrows()
    {
        var content = new byte[] { 1 };
        var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, content.Length, "file", "map.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        _mapServiceMock
            .Setup(s => s.UploadMapAsync(It.IsAny<IFormFile>()))
            .ThrowsAsync(new InvalidOperationException("Failure!"));

        var result = await _controller.UploadMapImage(file);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failure!", badRequest.Value);
    }
}