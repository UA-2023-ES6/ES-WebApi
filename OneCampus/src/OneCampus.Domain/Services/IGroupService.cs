using OneCampus.Domain.Entities.Groups;

namespace OneCampus.Domain.Services;

public interface IGroupService
{
    Task<Group> CreateGroupAsync(string name, int parentGroupId);

    Task<Group> UpdateGroupAsync(int id, string name);

    Task<IEnumerable<Group>> GetGroupsAsync();

    Task<Group> FindGroupAsync(int id);

    Task<Group?> DeleteGroupAsync(int id);

    Task<Group> AddUserAsync(int groupId, Guid userId);

    Task<Group> RemoveUserAsync(int groupId, Guid userId);
}
