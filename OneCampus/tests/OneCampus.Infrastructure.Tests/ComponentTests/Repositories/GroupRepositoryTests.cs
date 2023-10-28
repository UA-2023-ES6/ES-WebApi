using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class GroupRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private GroupRepository _groupRepository;

    [SetUp]
    public void SetUp()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());

        _mockDbContextFactory = new Mock<IDbContextFactory<OneCampusDbContext>>();

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<OneCampusDbContext>()
            .UseSqlite(_connection)
            .Options;
        using (var dbContext = new OneCampusDbContext(options))
        {
            dbContext.Database.EnsureCreated();
        }

        _mockDbContextFactory.Setup(item => item.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new OneCampusDbContext(options));

        _groupRepository = new GroupRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();

        const string Name = "group name";

        var group = await _groupRepository.CreateAsync(Name, dbGroupWithInstitution.Id);

        group.Should().NotBeNull();
        group!.Id.Should().BePositive();
        group.Name.Should().Be(Name);
    }

    #endregion

    #region UpdateAsync

    [Test]
    public async Task UpdateAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();
        var dbGroup = await AddGroupAsync(dbGroupWithInstitution.Id);

        const string Name = "new group name";

        var group = await _groupRepository.UpdateAsync(dbGroup.Id, Name);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(Name);
    }

    #endregion

    #region FindAsync

    [Test]
    public async Task FindAsync_WithGroup_ReturnsTheGroup()
    {
        var dbGroup = _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .Without(item => item.Institution)
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroup);

            await dbContext.SaveChangesAsync();

            dbGroup = result.Entity;
        }

        var group = await _groupRepository.FindAsync(dbGroup.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(dbGroup.Name);
    }

    [Test]
    public async Task FindAsync_WithDeletedGroup_ReturnsNull()
    {
        var dbGroup = GetMockedGroupWithInstitution()
            .With(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroup);

            await dbContext.SaveChangesAsync();

            dbGroup = result.Entity;
        }

        var group = await _groupRepository.FindAsync(dbGroup.Id);

        group.Should().BeNull();
    }

    #endregion

    #region FindByInstitutionIdAsync

    [Test]
    public async Task FindByInstitutionIdAsync_WithInstitution_ReturnsTheInstitution()
    {
        var dbGroup = await AddGroupWithInstitutionAsync();

        var group = await _groupRepository.FindByInstitutionIdAsync(dbGroup.Institution!.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(dbGroup.Name);
    }

    [Test]
    public async Task FindByInstitutionIdAsync_WithDeletedInstitution_ReturnsNull()
    {
        var dbGroup = GetMockedGroupWithInstitution()
            .With(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroup);

            await dbContext.SaveChangesAsync();

            dbGroup = result.Entity;
        }

        var group = await _groupRepository.FindByInstitutionIdAsync(dbGroup.Institution!.Id);

        group.Should().BeNull();
    }

    #endregion

    #region GetSubGroupsAsync

    [Test]
    public async Task GetSubGroupsAsync_GetSubGroups_ReturnsSubGroups()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();
        var dbGroup = await AddGroupAsync(dbGroupWithInstitution.Id);
        var dbGroup2 = await AddGroupAsync(dbGroupWithInstitution.Id, deleted: true);
        var dbGroup3 = await AddGroupAsync(dbGroupWithInstitution.Id);

        var groups = await _groupRepository.GetSubGroupsAsync(dbGroupWithInstitution.Id);

        groups.Should().NotBeNullOrEmpty()
            .And.Contain(item => item.Id == dbGroup.Id)
            .And.NotContain(item => item.Id == dbGroup2.Id)
            .And.Contain(item => item.Id == dbGroup3.Id);
    }

    #endregion

    #region DeleteAsync

    [Test]
    public async Task DeleteAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();
        var dbGroup = await AddGroupAsync(dbGroupWithInstitution.Id);

        var group = await _groupRepository.DeleteAsync(dbGroup.Id);

        group.Should().NotBeNull();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.FindAsync(dbGroup.Id);
            result.Should().NotBeNull();
            result!.DeleteDate.Should().NotBeNull();
        }
    }

    #endregion

    #region AddUserAsync

    [Test]
    public async Task AddUserAsync_AddUser_ReturnsTheGroupWithTheNewUser()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();
        var dbGroup = await AddGroupAsync(dbGroupWithInstitution.Id);
        var bdUser = await AddUserAsync();

        var group = await _groupRepository.AddUserAsync(dbGroup.Id, bdUser.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Users.Should().NotBeNullOrEmpty()
            .And.Contain(user => user.Id == bdUser.Id);
    }

    #endregion

    #region RemoveUserAsync

    [Test]
    public async Task RemoveUserAsync_RemovesUser_ReturnsTheGroupWithoutTheUser()
    {
        var dbGroupWithInstitution = await AddGroupWithInstitutionAsync();
        var dbGroup = await AddGroupAsync(dbGroupWithInstitution.Id);
        var bdUser = await AddUserAsync();
        var bdUser2 = await AddUserAsync();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.FindAsync(dbGroup.Id);
            result!.Users.Add(bdUser);
            result!.Users.Add(bdUser2);

            await dbContext.SaveChangesAsync();
        }

        var group = await _groupRepository.RemoveUserAsync(dbGroup.Id, bdUser.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Users.Should().NotBeNullOrEmpty()
            .And.Contain(user => user.Id == bdUser2.Id)
            .And.NotContain(user => user.Id == bdUser.Id);
    }

    #endregion

    #region Private Methods

    private IPostprocessComposer<Database.Group> GetMockedGroupWithInstitution()
    {
        var institution = _fixture.Build<Database.Institution>()
            .Without(item => item.Group)
            .Without(item => item.DeleteDate)
            .Without(item => item.GroupId)
            .Create();

        return _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .With(item => item.Institution, institution);
    }

    private async Task<Database.Group> AddGroupWithInstitutionAsync()
    {
        var dbGroupWithInstitution = GetMockedGroupWithInstitution()
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroupWithInstitution);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }

    private async Task<Database.Group> AddGroupAsync(int parentId, bool deleted = false)
    {
        var dbGroupWithInstitutionBuilder = GetMockedGroupWithInstitution()
            .With(item => item.ParentId, parentId);

        dbGroupWithInstitutionBuilder = deleted
            ? dbGroupWithInstitutionBuilder.With(item => item.DeleteDate)
            : dbGroupWithInstitutionBuilder.Without(item => item.DeleteDate);

        var dbGroupWithInstitution = dbGroupWithInstitutionBuilder.Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroupWithInstitution);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }

    private async Task<Database.User> AddUserAsync()
    {
        var dbUser = _fixture.Build<Database.User>()
            .Without(item => item.Id)
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Users.AddAsync(dbUser);

            await dbContext.SaveChangesAsync();

            return result.Entity;
        }
    }

    #endregion
}
