using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class GroupServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IGroupRepository> _mockGroupRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IInstitutionRepository> _mockInstitutionRepository;

    private GroupService _service;

    [SetUp]
    public void Setup()
    {
        _mockGroupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockInstitutionRepository = new Mock<IInstitutionRepository>(MockBehavior.Strict);

        _service = new GroupService(_mockGroupRepository.Object, _mockUserRepository.Object, _mockInstitutionRepository.Object);

        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(true);
    }

    #region CreateGroupAsync

    [Test]
    public async Task CreateGroupAsync_CreateGroup_ReturnsTheNewGroup()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockGroupRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockGroupRepository.Setup(item => item.AddUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.CreateGroupAsync(Guid.NewGuid(), "name", 1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task CreateGroupAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.CreateGroupAsync(Guid.NewGuid(), "name", 1))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region UpdateGroupAsync

    [Test]
    public async Task UpdateGroupAsync_UpdateGroup_ReturnsTheNewGroup()
    {
        _mockGroupRepository.Setup(item => item.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.UpdateGroupAsync(Guid.NewGuid(), 1, "new name");

        result.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateGroupAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.UpdateGroupAsync(Guid.NewGuid(), 1, "new name"))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task UpdateGroupAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.UpdateGroupAsync(Guid.NewGuid(), 1, "new name"))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region GetGroupsAsync

    [Test]
    public async Task GetGroupsAsync_GetGroups_ReturnsGroups()
    {
        _mockInstitutionRepository.Setup(item => item.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.CreateMany<Domain.Entities.Institutions.Institution>(1).ToList());

        _mockGroupRepository.Setup(item => item.FindByInstitutionIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockGroupRepository.Setup(item => item.GetSubGroupsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Group>(0));

        var result = await _service.GetGroupsAsync(Guid.NewGuid());

        result.Should().NotBeNull();
    }

    #endregion

    #region FindGroupAsync

    [Test]
    public async Task FindGroupAsync_FindGroup_ReturnsTheGroup()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.FindGroupAsync(Guid.NewGuid(), 1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task FindGroupAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.FindGroupAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task FindGroupAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.FindGroupAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region DeleteGroupAsync

    [Test]
    public async Task DeleteGroupAsync_WithGroup_ReturnsTheDeletedGroup()
    {
        _mockGroupRepository.Setup(item => item.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.DeleteGroupAsync(Guid.NewGuid(), 1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task DeleteGroupAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.DeleteGroupAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region AddUserAsync

    [Test]
    public async Task AddUserAsync_AddUserToGroup_ReturnsTheGroup()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.AddUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.AddUserAsync(Guid.NewGuid(), 1, Guid.NewGuid());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task AddUserAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.AddUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        await _service.Invoking(s => s.AddUserAsync(Guid.NewGuid(), 1, Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AddUserAsync_WithUserNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockGroupRepository.Setup(item => item.AddUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        await _service.Invoking(s => s.AddUserAsync(Guid.NewGuid(), 1, Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AddUserAsync_WithGroupToUpdateNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.AddUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.AddUserAsync(Guid.NewGuid(), 1, Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AddUserAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.AddUserAsync(Guid.NewGuid(), 1, Guid.NewGuid()))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region RemoveUserAsync

    [Test]
    public async Task RemoveUserAsync_AddUserToGroup_ReturnsTheGroup()
    {
        var userId = Guid.NewGuid();

        var group = Fixture
                .Build<Group>()
                .Create();
        group.Users.Add(new User(userId, "name", "email"));

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(group);

        _mockGroupRepository.Setup(item => item.RemoveUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.RemoveUserAsync(Guid.NewGuid(), 1, userId);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task RemoveUserAsync_WithGroupToUpdateNotFound_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();

        var group = Fixture
                .Build<Group>()
                .Create();
        group.Users.Add(new User(userId, "name", "email"));

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(group);

        _mockGroupRepository.Setup(item => item.RemoveUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.RemoveUserAsync(Guid.NewGuid(), 1, userId))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task RemoveUserAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.IsUserInTheGroupAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.RemoveUserAsync(Guid.NewGuid(), 1, Guid.NewGuid()))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion
}
