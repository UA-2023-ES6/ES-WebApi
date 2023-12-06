using OneCampus.Domain.Entities.Groups;

namespace OneCampus.Domain.Repositories;

public interface IGroupRepository
{
    Task<Group> CreateAsync(string name, int parentGroupId);

    Task<Group?> UpdateAsync(int id, string name);

    Task<GroupDetails?> FindAsync(int id);

    Task<Group?> FindByInstitutionIdAsync(int institutionId);

    Task<IEnumerable<Group>> GetSubGroupsAsync(Guid userId, int id);

    Task<Group?> DeleteAsync(int id);

    Task<GroupDetails?> AddUserAsync(int groupId, Guid userId);

    Task<GroupDetails?> RemoveUserAsync(int groupId, Guid userId);

    Task<(IEnumerable<User> Results, int TotalResults)> GetUsersAsync(int id, int take, int skip);

    Task<bool> HasAccessAsync(Guid userId, int groupId);
}
