namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class UserRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private UserRepository _userRepository;

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

        _userRepository = new UserRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region FindAsync

    [Test]
    public async Task FindAsync_WithUser_ReturnsTheUser()
    {
        var dbUser = _fixture.Build<Database.User>()
            .Without(item => item.Id)
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Users.AddAsync(dbUser);

            await dbContext.SaveChangesAsync();

            dbUser = result.Entity;
        }

        var user = await _userRepository.FindAsync(dbUser.Id);

        user.Should().NotBeNull();
        user!.Id.Should().Be(dbUser.Id);
        user.Name.Should().Be(dbUser.Name);
    }

    [Test]
    public async Task FindAsync_WithDeletedUser_ReturnsNull()
    {
        var dbUser = _fixture.Build<Database.User>()
            .Without(item => item.Id)
            .With(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Users.AddAsync(dbUser);

            await dbContext.SaveChangesAsync();

            dbUser = result.Entity;
        }

        var user = await _userRepository.FindAsync(dbUser.Id);

        user.Should().BeNull();
    }

    #endregion
}
