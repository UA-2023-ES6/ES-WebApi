using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Users;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class UserServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IUserRepository> _mockUserRepository;

    private UsersService _service;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);

        _service = new UsersService(_mockUserRepository.Object);
    }

    #region CreateGroupAsync

    [Test]
    public async Task CreateGroupAsync_CreateGroup_ReturnsTheNewGroup()
    {
        _mockUserRepository.Setup(item => item.CreateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Fixture.Create<User>());

        var result = await _service.CreateAsync(Guid.NewGuid(),"name", "email");

        result.Should().NotBeNull();
    }

    #endregion

    #region FindAsync

    [Test]
    public async Task FindAsync_FindUser_ReturnsTheUser()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<User>());

        var result = await _service.FindAsync(Guid.NewGuid());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task FindAsync_WithUserNotFound_ReturnNull()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var result = await _service.FindAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    #endregion

    #region FindByEmailAsync

    [Test]
    public async Task FindByEmailAsync_FindUser_ReturnsTheUser()
    {
        _mockUserRepository.Setup(item => item.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(Fixture.Create<User>());

        var result = await _service.FindByEmailAsync("username");

        result.Should().NotBeNull();
    }

    [Test]
    public async Task FindByEmailAsync_WithUserNotFound_ReturnNull()
    {
        _mockUserRepository.Setup(item => item.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var result = await _service.FindByEmailAsync("username");

        result.Should().BeNull();
    }

    #endregion
}