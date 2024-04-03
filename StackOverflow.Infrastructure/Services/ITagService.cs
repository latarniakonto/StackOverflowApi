
namespace StackOverflow.Infrastructure.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetTagsAsync(int page, int pageSize, string nameSortOrder, string weightSortOrder);
    Task<IEnumerable<Tag>> GetOrUpdateAllTagsAsync();
}
