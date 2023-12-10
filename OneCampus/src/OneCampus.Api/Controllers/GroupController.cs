using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models;
using OneCampus.Api.Models.Requests.Groups;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Services;
using System.Net.Mime;

namespace OneCampus.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    private readonly UserInfo _userInfo;

    public GroupController(IGroupService groupService, UserInfo userInfo)
    {
        _groupService = groupService.ThrowIfNull().Value;
        _userInfo = userInfo.ThrowIfNull().Value;
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateGroupRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CreateGroupAsync([FromBody] CreateGroupRequest request)
    {
        var group = await _groupService.CreateGroupAsync(_userInfo.Id, request.Name, request.ParentGroupId);

        return Ok(new BaseResponse<CreateGroupRequest, Group>(request, group));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UpdateGroupRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateGroupAsync([FromRoute] int id, [FromBody] UpdateGroupRequest request)
    {
        var group = await _groupService.UpdateGroupAsync(_userInfo.Id, id, request.Name);

        return Ok(new BaseResponse<UpdateGroupRequest, Group>(request, group));
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseWithoutRequest<IEnumerable<Group>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetGroupsAsync()
    {
        var results = await _groupService.GetGroupsAsync(_userInfo.Id);

        return Ok(new BaseResponseWithoutRequest<IEnumerable<Group>>(results));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GroupByIdRequest, GroupDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindGroupAsync([FromRoute] int id)
    {
        var group = await _groupService.FindGroupAsync(_userInfo.Id, id);

        var request = new GroupByIdRequest
        {
            Id = id
        };

        return Ok(new BaseResponse<GroupByIdRequest, GroupDetails>(request, group));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GroupByIdRequest, Group?>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteGroupAsync(int id)
    {
        var group = await _groupService.DeleteGroupAsync(_userInfo.Id, id);

        var request = new GroupByIdRequest
        {
            Id = id
        };

        return Ok(new BaseResponse<GroupByIdRequest, Group?>(request, group));
    }

    [HttpGet("{id}/user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnumerableResponse<UsersRequest, User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUsersAsync([FromRoute] int id, [FromQuery] int take, [FromQuery] int skip)
    {
        var (users, totalResults) = await _groupService.GetUsersAsync(_userInfo.Id, id, take, skip);

        var request = new UsersRequest
        {
            Id = id,
            Take = take,
            Skip = skip
        };

        return Ok(new EnumerableResponse<UsersRequest, User>(request, users, totalResults));
    }

    [HttpPost("{id}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserRequest, GroupDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddUserAsync([FromRoute] int id, [FromRoute] Guid userId)
    {
        var group = await _groupService.AddUserAsync(_userInfo.Id, id, userId);

        var request = new UserRequest
        {
            GroupId = id,
            UserId = userId
        };

        return Ok(new BaseResponse<UserRequest, GroupDetails>(request, group));
    }

    [HttpDelete("{id}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserRequest, GroupDetails>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveUserAsync([FromRoute] int id, [FromRoute] Guid userId)
    {
        var group = await _groupService.RemoveUserAsync(_userInfo.Id, id, userId);

        var request = new UserRequest
        {
            GroupId = id,
            UserId = userId
        };

        return Ok(new BaseResponse<UserRequest, GroupDetails>(request, group));
    }
}
