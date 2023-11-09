using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Services;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class MessageControllerTests
{
    private Mock<IMessageService> _mockIMessageService;

    private MessageController _controller;

    private readonly CreateMessageRequest request = new CreateMessageRequest
    {
        Content = "Test Message",
        GroupId = 1,
        UserId = new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87")
    };

    private readonly Message expectedMessage = new Message(1, 1, "Test Message");


    [SetUp]
    public void Setup()
    {
        _mockIMessageService = new Mock<IMessageService>(MockBehavior.Strict);

        _controller = new MessageController(_mockIMessageService.Object);
    }

    [Test]
    public async Task CreateMessageAsyncTest()
    {

        _mockIMessageService.Setup(s => s.CreateMessageAsync(request.GroupId, request.Content, request.UserId))
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
            new Message(1,1,"Test Message 1"),
            new Message(2,1,"Test Message 2"),
            new Message(3,1,"Test Message 3")

        };

        _mockIMessageService.Setup(s => s.FindMessagesByGroupAsync(validId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.FindGroupMessagesAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as BaseResponse<MessagesByGroupRequest, List<Message>>;

        response.Should().NotBeNull();
        response!.Request.GroupId.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expected);
    }

}

