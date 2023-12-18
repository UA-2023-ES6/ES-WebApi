using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Services;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class MessageControllerTests
{
    private readonly Fixture _fixture = new();

    private Mock<IMessageService> _mockIMessageService;

    private MessageController _controller;

    private readonly CreateMessageRequest request = new CreateMessageRequest
    {
        Content = "Test Message",
        GroupId = 1
    };

    private readonly Message expectedMessage = new Message(1, 1, "Test Message", "User 1", DateTime.UtcNow);

    [SetUp]
    public void Setup()
    {
        _mockIMessageService = new Mock<IMessageService>(MockBehavior.Strict);

        var user = _fixture.Create<UserInfo>();

        _controller = new MessageController(_mockIMessageService.Object, user);
    }

    [Test]
    public async Task CreateMessageAsyncTest()
    {
        _mockIMessageService.Setup(s => s.CreateMessageAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .ReturnsAsync(expectedMessage);

        var result = await _controller.CreateMessageAsync(request);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<CreateMessageRequest, Message>>();
    }

    [Test]
    public async Task FindGroupMessagesAsyncTest()
    {
        // Arrange
        int validId = 1;  // Assuming 1 is a valid ID for this test
        var expected = new List<Message>
        {
            new Message(1, 1, "Test Message 1", "User 1", DateTime.UtcNow.AddMinutes(-2)),
            new Message(2, 1, "Test Message 2", "User 2", DateTime.UtcNow.AddMinutes(-1)),
            new Message(3, 1, "Test Message 3", "User 3", DateTime.UtcNow)
        };

        _mockIMessageService.Setup(s => s.FindMessagesByGroupAsync(validId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.FindGroupAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;

        okResult!.Value.Should().NotBeNull()
            .And.BeOfType<EnumerableResponse<MessagesByGroupRequest, Message>>();

        var response = okResult!.Value as EnumerableResponse<MessagesByGroupRequest, Message>;

        response.Should().NotBeNull();
        response!.Request.GroupId.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expected);
    }
}
