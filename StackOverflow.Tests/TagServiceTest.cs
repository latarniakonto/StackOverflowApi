using StackOverflow.Infrastructure;
using StackOverflow.Infrastructure.Services;

namespace StackOverflow.Tests;

public class TagServiceTest
{
    [Fact]
    public async Task GetTagsAsync_ThrowsException()
    {
        // Arrange
        MongoDbContext dbContext = null;
        var service = new TagService(dbContext, null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => service.GetTagsAsync());
    }
}
