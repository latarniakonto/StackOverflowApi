
namespace StackOverflow.Infrastructure.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsAsync(int page = 1, int pageSize = 10, string nameSortOrder = "asc", string weightSortOrder = "asc");
    Task<IEnumerable<Tag>> GetOrUpdateAllTagsAsync();
}
