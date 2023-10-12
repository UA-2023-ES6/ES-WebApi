using ES6WebApi.Controllers;
using ES6WebApi.Models.Responses;
using ES6WebApi.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ES6WebApi.Tests.ComponentTests.Controllers;

[TestFixture]
public class TimeControllerTests
{
    private Mock<IDateTimeProvider> _mockDateTimeProvider;

    private TimeController _controller;

    [SetUp]
    public void Setup()
    {
        _mockDateTimeProvider = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        _controller = new TimeController(_mockDateTimeProvider.Object);
    }

    [Test]
    public async Task GetServerTimeAsync_GetServerTime_ReturnsUtcNow()
    {
        var date = DateTime.UtcNow;

        _mockDateTimeProvider.SetupGet(item => item.UtcNow)
            .Returns(date);

        var result = await _controller.GetServerTimeAsync();
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<ServerTimeResponse>();

        var serverTimeResponse = response.Value as ServerTimeResponse;

        serverTimeResponse!.ServerTime.Should().Be(date);
    }
}
