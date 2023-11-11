namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

public class MessageRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private MessageRepository _messageRepository;

    private IDbContextFactory<OneCampusDbContext> _dbContextFactory => _mockDbContextFactory.Object;

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

        _messageRepository = new MessageRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateMessage_ReturnsTheMessage()
    {
        const string Content = "Test message";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var message = await _messageRepository.CreateAsync(Content, group.Id, user.Id);

        message.Should().NotBeNull();
        message!.Id.Should().BePositive();
        message.Content.Should().Be(Content);
        message.GroupId.Should().Be(group.Id);
        message.SenderName.Should().NotBeNullOrWhiteSpace()
            .And.Be(user.Name);
    }

    [Test]
    public async Task CreateAsync_WithUserNotFound_ReturnsNull()
    {
        const string Content = "Test message";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var message = await _messageRepository.CreateAsync(Content, group.Id, Guid.NewGuid());

        message.Should().BeNull();
    }

    #endregion

    #region GetMessagesByGroupAsync

    [Test]
    public async Task GetMessagesByGroupAsync_GroupWithMessages_ReturnsTheGroupMessage()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var message1 = await MessageHelper.AddMessageAsync(_dbContextFactory, group.Id, user.Id);
        var message2 = await MessageHelper.AddMessageAsync(_dbContextFactory, group.Id, user.Id);

        var messages = await _messageRepository.GetMessagesByGroupAsync(group.Id);

        messages.Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.Contain(item => item.Content == message1.Content)
            .And.Contain(item => item.Content == message2.Content)
            .And.BeInAscendingOrder(item => item.CreateDate);
    }

    #endregion
}


