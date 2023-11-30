using OneCampus.Domain.Entities.Groups;

namespace OneCampus.Domain.Repositories;

public interface IGroupRepository
{
    Task<Group> CreateAsync(string name, int parentGroupId);

    Task<Group?> UpdateAsync(int id, string name);

    Task<Group?> FindAsync(int id);

    Task<Group?> FindByInstitutionIdAsync(int institutionId);

    Task<IEnumerable<Group>> GetSubGroupsAsync(int id);

    Task<Group?> DeleteAsync(int id);

    Task<Group?> AddUserAsync(int groupId, Guid userId);

    Task<Group?> RemoveUserAsync(int groupId, Guid userId);

    Task<(IEnumerable<User> Results, int TotalResults)> GetUsersAsync(int id, int take, int skip);
}
