using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models.Requests.Groups;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Services;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class GroupControllerTests
{
    private readonly Fixture _fixture = new();

    private Mock<IGroupService> _mockIGroupService;

    private GroupController _controller;

    private UserInfo userInfo;

    private readonly CreateGroupRequest request = new CreateGroupRequest
    {
        Name = "TestGroup",
        ParentGroupId = 1
    };

    private readonly UpdateGroupRequest update_request = new UpdateGroupRequest
    {
        Name = "TestGroup",
    };

    private readonly Group expectedGroup = new Group(0, "TestGroup");

    [SetUp]
    public void Setup()
    {
        _mockIGroupService = new Mock<IGroupService>(MockBehavior.Strict);

        userInfo = _fixture.Create<UserInfo>();

        _controller = new GroupController(_mockIGroupService.Object, userInfo);
    }

    [Test]
    public async Task CreateGroupAsyncTest()
    {
        _mockIGroupService.Setup(s => s.CreateGroupAsync(userInfo.Id, request.Name, request.ParentGroupId))
            .ReturnsAsync(expectedGroup);

        var result = await _controller.CreateGroupAsync(request);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<CreateGroupRequest, Group>>();
    }

    [Test]
    public async Task UpdateGroupAsyncTest()
    {
        _mockIGroupService.Setup(s => s.UpdateGroupAsync(userInfo.Id, 0, "TestGroup"))
            .ReturnsAsync(expectedGroup);

        var result = await _controller.UpdateGroupAsync(0, update_request);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<UpdateGroupRequest, Group>>();
    }

    [Test]
    public async Task GetGroupsAsyncTest()
    {
        var expectedData = new List<Group>
        {
            new Group(1, "TestGroup1"),
            new Group(2, "TestGroup2"),
            new Group(3, "TestGroup3"),
        };

        _mockIGroupService.Setup(s => s.GetGroupsAsync(userInfo.Id))
            .ReturnsAsync(expectedData);

        var result = await _controller.GetGroupsAsync();

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;

        okResult!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponseWithoutRequest<IEnumerable<Group>>>();

        var response = okResult!.Value as BaseResponseWithoutRequest<IEnumerable<Group>>;

        response.Should().NotBeNull();
        response!.Data.Should().BeEquivalentTo(expectedData);
    }

    [Test]
    public async Task FindGroupAsyncTest()
    {
        // Arrange
        int validId = 1;  // Assuming 1 is a valid ID for this test
        var expected =  _fixture.Create<GroupDetails>();

        _mockIGroupService.Setup(s => s.FindGroupAsync(userInfo.Id, validId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.FindGroupAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as BaseResponse<GroupByIdRequest, GroupDetails>;

        response.Should().NotBeNull();
        response!.Request.Id.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task DeleteGroupAsyncTest()
    {
        // Arrange
        int validId = 1;  // Assuming 1 is a valid ID for this test
        var expectedDeletedGroup = new Group(1, "TestGroup");

        _mockIGroupService.Setup(s => s.DeleteGroupAsync(userInfo.Id, validId))
            .ReturnsAsync(expectedDeletedGroup);

        // Act
        var result = await _controller.DeleteGroupAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as BaseResponse<GroupByIdRequest, Group?>;

        response.Should().NotBeNull();
        response!.Request.Id.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expectedDeletedGroup);
    }

    [Test]
    public async Task GetUsersAsyncTest()
    {
        // Arrange
        int id = 10;
        int take = 10;
        int skip = 1;
        var expectedResults = _fixture.CreateMany<User>(3);
        var expectedTotalResults = 10;

        _mockIGroupService.Setup(s => s.GetUsersAsync(userInfo.Id, id, take, skip))
            .ReturnsAsync((expectedResults, expectedTotalResults));

        // Act
        var result = await _controller.GetUsersAsync(id, take, skip);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as EnumerableResponse<UsersRequest, User>;

        response.Should().NotBeNull();
        response!.Request.Id.Should().Be(id);
        response.Request.Take.Should().Be(take);
        response.Request.Skip.Should().Be(skip);
        response.Data.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(expectedResults);
        response.TotalResults.Should().Be(expectedTotalResults);
    }

    [Test]
    public async Task AddUserAsyncTest()
    {
        // Arrange
        int validGroupId = 1;  // Sample Group ID
        Guid validUserId = Guid.NewGuid();  // Sample User ID
        var expected = _fixture.Create<GroupDetails>();

        _mockIGroupService.Setup(s => s.AddUserAsync(userInfo.Id, validGroupId, validUserId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.AddUserAsync(validGroupId, validUserId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as BaseResponse<UserRequest, GroupDetails>;

        response.Should().NotBeNull();
        response!.Request.GroupId.Should().Be(validGroupId);
        response.Request.UserId.Should().Be(validUserId);
        response.Data.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task DeleteUserAsyncTest()
    {
        int validGroupId = 1;  // Sample Group ID
        Guid validUserId = Guid.NewGuid();  // Sample User ID
        var expectedGroupAfterRemoval = _fixture.Create<GroupDetails>();

        _mockIGroupService.Setup(s => s.RemoveUserAsync(userInfo.Id, validGroupId, validUserId))
            .ReturnsAsync(expectedGroupAfterRemoval);

        // Act
        var result = await _controller.RemoveUserAsync(validGroupId, validUserId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as BaseResponse<UserRequest, GroupDetails>;

        response.Should().NotBeNull();
        response!.Request.GroupId.Should().Be(validGroupId);
        response.Request.UserId.Should().Be(validUserId);
        response.Data.Should().BeEquivalentTo(expectedGroupAfterRemoval);
    }
}
