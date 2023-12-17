using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models.Requests.Permissions;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Services;

namespace OneCampus.Api.Tests.ComponentTests.Controllers;

[TestFixture]
public class PermissionControllerTests
{
    private readonly Fixture _fixture = new();
    private Mock<IPermissionService> _mockPermissionService;
    private PermissionController _controller;

    [SetUp]
    public void Setup()
    {
        _mockPermissionService = new Mock<IPermissionService>(MockBehavior.Strict);

        _controller = new PermissionController(_mockPermissionService.Object);
    }

    [Test]
    public async Task AllowPermissionsAsync_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        const int GroupId = 1;
        var permissions = new List<PermissionType>
        {
            PermissionType.CreateSubGroup,
            PermissionType.CreateMessage
        };

        _mockPermissionService.Setup(s => s.AllowPermissionsAsync(userId, GroupId, permissions))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _controller.AllowPermissionsAsync(GroupId,userId,new Models.Requests.Permissions.PermissionRequest
        {
            Permissions = permissions
        });

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<PermissionResponseRequest, UserPermissions>>();
    }

    [Test]
    public async Task DenyPermissionsAsync_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        const int GroupId = 1;
        var permissions = new List<PermissionType>
        {
            PermissionType.CreateSubGroup,
            PermissionType.CreateMessage
        };

        _mockPermissionService.Setup(s => s.DenyPermissionsAsync(userId, GroupId, permissions))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _controller.DenyPermissionsAsync(GroupId, userId, new PermissionRequest
        {
            Permissions = permissions
        });

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<PermissionResponseRequest, UserPermissions>>();
    }

    [Test]
    public async Task GetPermissionsAsync_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        const int GroupId = 1;

        _mockPermissionService.Setup(s => s.GetMyPermissionsAsync(GroupId))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _controller.GetPermissionsAsync(GroupId);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<MePermissionsRequest, UserPermissions>>();
    }

    [Test]
    public async Task GetUserPermissionsAsync_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        const int GroupId = 1;

        _mockPermissionService.Setup(s => s.GetPermissionsAsync(userId, GroupId))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _controller.GetUserPermissionsAsync(GroupId, userId);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<UserPermissionsRequest, UserPermissions>>();
    }
}