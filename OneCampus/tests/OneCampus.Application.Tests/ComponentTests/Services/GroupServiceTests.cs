﻿using OneCampus.Application.Services;
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
    }

    #region CreateGroupAsync

    [Test]
    public async Task CreateGroupAsync_CreateGroup_ReturnsTheNewGroup()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockGroupRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.CreateGroupAsync("name", 1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task CreateGroupAsync_WithParentNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        _mockGroupRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        await _service.Invoking(s => s.CreateGroupAsync("name", 1))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region UpdateGroupAsync

    [Test]
    public async Task UpdateGroupAsync_UpdateGroup_ReturnsTheNewGroup()
    {
        _mockGroupRepository.Setup(item => item.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.UpdateGroupAsync(1, "new name");

        result.Should().NotBeNull();
    }

    [Test]
    public async Task UpdateGroupAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.UpdateGroupAsync(1, "new name"))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetGroupsAsync

    [Test]
    public async Task GetGroupsAsync_GetGroups_ReturnsGroups()
    {
        _mockInstitutionRepository.Setup(item => item.GetAsync())
            .ReturnsAsync(Fixture.CreateMany<Domain.Entities.Institutions.Institution>(1).ToList());

        _mockGroupRepository.Setup(item => item.FindByInstitutionIdAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        _mockGroupRepository.Setup(item => item.GetSubGroupsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Group>(0));

        var result = await _service.GetGroupsAsync();

        result.Should().NotBeNull();
    }

    #endregion

    #region FindGroupAsync

    [Test]
    public async Task FindGroupAsync_FindGroup_ReturnsTheGroup()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.FindGroupAsync(1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task FindGroupAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        await _service.Invoking(s => s.FindGroupAsync(1))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region DeleteGroupAsync

    [Test]
    public async Task DeleteGroupAsync_WithGroup_ReturnsTheDeletedGroup()
    {
        _mockGroupRepository.Setup(item => item.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Group>());

        var result = await _service.DeleteGroupAsync(1);

        result.Should().NotBeNull();
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

        var result = await _service.AddUserAsync(1, Guid.NewGuid());

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

        await _service.Invoking(s => s.AddUserAsync(1, Guid.NewGuid()))
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

        await _service.Invoking(s => s.AddUserAsync(1, Guid.NewGuid()))
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

        await _service.Invoking(s => s.AddUserAsync(1, Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
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

        var result = await _service.RemoveUserAsync(1, userId);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task RemoveUserAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Group?)null);

        _mockGroupRepository.Setup(item => item.RemoveUserAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Group>());

        await _service.Invoking(s => s.RemoveUserAsync(1, Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
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

        await _service.Invoking(s => s.RemoveUserAsync(1, userId))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion
}