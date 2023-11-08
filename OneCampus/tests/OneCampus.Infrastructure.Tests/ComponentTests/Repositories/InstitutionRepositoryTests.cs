using AutoFixture.Dsl;

namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class InstitutionRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private InstitutionRepository _institutionRepository;

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

        _institutionRepository = new InstitutionRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region FindAsync

    [Test]
    public async Task FindAsync_WithInstitution_ReturnsTheInstitution()
    {
        var dbInstitution = GetMockedInstitution()
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Institutions.AddAsync(dbInstitution);

            await dbContext.SaveChangesAsync();

            dbInstitution = result.Entity;
        }

        var institution = await _institutionRepository.FindAsync(dbInstitution.Id);

        institution.Should().NotBeNull();
        institution!.Id.Should().Be(dbInstitution.Id);
        institution.Name.Should().Be(dbInstitution.Name);
    }

    [Test]
    public async Task FindAsync_WithDeletedInstitution_ReturnsNull()
    {
        var dbInstitution = GetMockedInstitution()
            .With(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            var result = await dbContext.Institutions.AddAsync(dbInstitution);

            await dbContext.SaveChangesAsync();

            dbInstitution = result.Entity;
        }

        var institution = await _institutionRepository.FindAsync(dbInstitution.Id);

        institution.Should().BeNull();
    }

    #endregion

    #region GetAsync

    [Test]
    public async Task GetAsync_WithDeletedInstitution_ReturnsNull()
    {
        var dbInstitution = GetMockedInstitution()
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _mockDbContextFactory.Object.CreateDbContextAsync())
        {
            await dbContext.Institutions.AddAsync(GetMockedInstitution()
                .Without(item => item.DeleteDate)
                .Create());

            await dbContext.Institutions.AddAsync(GetMockedInstitution()
                .Without(item => item.DeleteDate)
                .Create());

            // Deleted Institution
            await dbContext.Institutions.AddAsync(GetMockedInstitution()
                .With(item => item.DeleteDate)
                .Create());

            await dbContext.SaveChangesAsync();
        }

        var institutions = await _institutionRepository.GetAsync();

        institutions.Should().NotBeNullOrEmpty()
            .And.HaveCount(2);
    }

    #endregion

    #region Private Methods

    private IPostprocessComposer<Database.Institution> GetMockedInstitution()
    {
        var dbInstitutionGroup = _fixture.Build<Database.Group>()
            .Without(item => item.Id)
            .Without(item => item.ParentId)
            .Create();

        return _fixture.Build<Database.Institution>()
            .Without(item => item.Id)
            .With(item => item.Group, dbInstitutionGroup)
            .Without(item => item.GroupId);
    }

    #endregion
}
