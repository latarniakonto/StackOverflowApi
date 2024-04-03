
namespace StackOverflow.Infrastructure.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsAsync();
    Task<IEnumerable<Tag>> GetOrUpdateAllTagsAsync();
}
