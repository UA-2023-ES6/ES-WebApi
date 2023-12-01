using OneCampus.Domain.Entities.Groups;

namespace OneCampus.Domain.Services;

public interface IGroupService
{
    Task<Group> CreateGroupAsync(Guid userId, string name, int parentGroupId);

    Task<Group> UpdateGroupAsync(Guid userId, int id, string name);

    Task<IEnumerable<Group>> GetGroupsAsync(Guid userId);

    Task<Group> FindGroupAsync(Guid userId, int id);

    Task<Group?> DeleteGroupAsync(Guid userId, int id);

    Task<Group> AddUserAsync(Guid userId, int groupId, Guid userIdToAdd);

    Task<Group> RemoveUserAsync(Guid userId, int groupId, Guid userIdToRemove);

    Task<(IEnumerable<User> Results, int TotalResults)> GetUsersAsync(Guid userId, int id, int take, int skip);
}
