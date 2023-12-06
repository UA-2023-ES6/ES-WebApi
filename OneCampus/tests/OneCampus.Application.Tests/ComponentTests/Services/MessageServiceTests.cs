using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class MessageServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IMessageRepository> _mockMessageRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IGroupRepository> _mockGroupRepository;

    private MessageService _service;

    [SetUp]
    public void Setup()
    {
        _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockGroupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);

        _service = new MessageService(_mockMessageRepository.Object, _mockUserRepository.Object, _mockGroupRepository.Object);
    }

    #region CreateMessageAsync

    [Test]
    public async Task CreateMessageAsync_CreateMessage_ReturnTheNewMessage()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.GroupDetails>());

        _mockMessageRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Message>());

        var result = await _service.CreateMessageAsync(1, "Test Message", Guid.NewGuid());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task CreateMessageAsync_WithNullUser_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.GroupDetails>());

        _mockMessageRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Message>());

        await _service.Invoking(s => s.CreateMessageAsync(1, "Test Message", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("*user*");
    }

    [Test]
    public async Task CreateMessageAsync_WithNullGroup_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Groups.GroupDetails?)null);

        _mockMessageRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Message>());

        await _service.Invoking(s => s.CreateMessageAsync(1, "Test Message", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("*group*");
    }

    #endregion

    #region FindMessagesByGroupAsync

    [Test]
    public async Task FindMessagesByGroupAsync_FindMessages_ReturnMessages()
    {
        var messages = Fixture
            .CreateMany<Message>(3)
            .ToList();

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.GroupDetails>());

        _mockMessageRepository.Setup(item => item.GetMessagesByGroupAsync(It.IsAny<int>()))
            .ReturnsAsync(messages);

        var result = await _service.FindMessagesByGroupAsync(1);

        result.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(messages);
    }

    [Test]
    public async Task FindMessagesByGroupAsync_WithNullGroup_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Groups.GroupDetails?)null);

        await _service.Invoking(s => s.FindMessagesByGroupAsync(1))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("*group*");
    }

    #endregion


}


