using System;
using OneCampus.Application.Services;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Entities.Messages;


namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class MessageServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IMessageRepository> _mockMessageRepository;

    private MessageService _service;

    [SetUp]
    public void Setup()
    {
        _mockMessageRepository = new Mock<IMessageRepository>(MockBehavior.Strict);

        _service = new MessageService(_mockMessageRepository.Object);
    }

    [Test]
    public async Task CreateMessageAsync_CreateMessage_ReturnTheNewMessage()
    {
        _mockMessageRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Message>());

        var result = await _service.CreateMessageAsync(1, "Test Message", new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87"));

        result.Should().NotBeNull();
    }

    /*[Test]
    public async Task FindMessagesByGroupAsync_FindMessages_ReturnMessages()
    {
        _mockMessageRepository.Setup(item => item.FindMessagesAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Message>());

        var result = await _service.FindMessagesByGroupAsync(1);

        result.Should().NotBeNull();
    }*/




}


