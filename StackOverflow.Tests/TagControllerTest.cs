using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using StackOverflow.Api.Controllers;
using StackOverflow.Api.Models;
using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Test;

public class TagControllerTest
{
    [Fact]
    public async Task GetTags_ReturnsTagsView()
    {
        // Arrange
        var mockTagService = new Mock<ITagService>();
        mockTagService.Setup(service => service.GetTagsAsync())
            .ReturnsAsync(new List<Tag>
            {
                new Tag { Id = ObjectId.GenerateNewId(), Name = "Tag1", Count = 12, Weight = 0.6f },
                new Tag { Id = ObjectId.GenerateNewId(), Name = "Tag2", Count = 8, Weight = 0.4f }
            });
        var controller = new TagController(mockTagService.Object);

        // Act
        var result = await controller.GetTags();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Tag>>(viewResult.ViewData.Model);
        Assert.Equal(12, model.First(t => t.Name == "Tag1").Count);
        Assert.Equal(8, model.First(t => t.Name == "Tag2").Count);
    }

    [Fact]
    public async Task GetTags_ReturnsErrorView()
    {
        // Arrange
        var mockTagService = new Mock<ITagService>();
        mockTagService.Setup(service => service.GetTagsAsync())
            .ThrowsAsync(new Exception("Some error occurred."));
        var controller = new TagController(mockTagService.Object);

        // Act
        var result = await controller.GetTags();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.ViewData.Model);
        Assert.Equal("Some error occurred.", model.Message);
    }

    [Fact]
    public async Task UpdateTags_ReturnsTagsView()
    {
        // Arrange
        var mockTagService = new Mock<ITagService>();
        mockTagService.Setup(service => service.GetOrUpdateAllTagsAsync())
            .ReturnsAsync(new List<Tag>
            {
                new Tag { Id = ObjectId.GenerateNewId(), Name = "Tag1", Count = 2, Weight = 0.25f },
                new Tag { Id = ObjectId.GenerateNewId(), Name = "Tag2", Count = 8, Weight = 0.75f }
            });
        var controller = new TagController(mockTagService.Object);

        // Act
        var result = await controller.UpdateTags();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Tag>>(viewResult.ViewData.Model);
        Assert.Equal(0.25f, model.First(t => t.Name == "Tag1").Weight);
        Assert.Equal(0.75f, model.First(t => t.Name == "Tag2").Weight);
    }

    [Fact]
    public async Task UpdateTags_ReturnsErrorView()
    {
        // Arrange
        var mockTagService = new Mock<ITagService>();
        mockTagService.Setup(service => service.GetOrUpdateAllTagsAsync())
            .ThrowsAsync(new Exception("Some error occurred."));
        var controller = new TagController(mockTagService.Object);

        // Act
        var result = await controller.UpdateTags();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.ViewData.Model);
        Assert.Equal("Some error occurred.", model.Message);
    }
}
