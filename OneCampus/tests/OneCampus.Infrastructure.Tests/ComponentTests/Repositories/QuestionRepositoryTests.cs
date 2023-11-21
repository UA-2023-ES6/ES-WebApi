namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

public class QuestionRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private QuestionRepository _QuestionRepository;

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

        _QuestionRepository = new QuestionRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateQuestion_ReturnsTheQuestion()
    {
        const string Content = "Test Question";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var Question = await _QuestionRepository.CreateAsync(Content, group.Id, user.Id);

        Question.Should().NotBeNull();
        Question!.Id.Should().BePositive();
        Question.Content.Should().Be(Content);
        Question.GroupId.Should().Be(group.Id);
        /*Question.UserId.Should().NotBeNullOrWhiteSpace()
            .And.Be(user.Name);*/
    }

    [Test]
    public async Task CreateAsync_WithUserNotFound_ReturnsNull()
    {
        const string Content = "Test Question";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var Question = await _QuestionRepository.CreateAsync(Content, group.Id, Guid.NewGuid());

        Question.Should().BeNull();
    }

    #endregion

    #region GetQuestionsByGroupAsync

    [Test]
    public async Task GetQuestionsByGroupAsync_GroupWithQuestions_ReturnsTheGroupQuestion()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var Question1 = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);
        var Question2 = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);

        var Questions = await _QuestionRepository.GetQuestionsByGroupAsync(group.Id);

        Questions.Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.Contain(item => item.Content == Question1.Content)
            .And.Contain(item => item.Content == Question2.Content)
            .And.BeInAscendingOrder(item => item.CreateDate);
    }

    #endregion
}


