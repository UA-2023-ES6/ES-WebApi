using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests;
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

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService.ThrowIfNull().Value;
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateGroupRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateGroupAsync([FromBody] CreateGroupRequest request)
    {
        var group = await _groupService.CreateGroupAsync(request.Name, request.ParentGroupId);

        return Ok(new BaseResponse<CreateGroupRequest, Group>(request, group));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UpdateGroupRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateGroupAsync([FromRoute] int id, [FromBody] UpdateGroupRequest request)
    {
        var group = await _groupService.UpdateGroupAsync(id,request.Name);

        return Ok(new BaseResponse<UpdateGroupRequest, Group>(request, group));
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseWithoutRequest<IEnumerable<Group>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetGroupsAsync()
    {
        var results = await _groupService.GetGroupsAsync();

        return Ok(new BaseResponseWithoutRequest<IEnumerable<Group>>(results));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GroupByIdRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindGroupAsync([FromRoute] int id)
    {
        var group = await _groupService.FindGroupAsync(id);

        var request = new GroupByIdRequest
        {
            Id = id
        };

        return Ok(new BaseResponse<GroupByIdRequest, Group>(request, group));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GroupByIdRequest, Group?>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteGroupAsync(int id)
    {
        var group = await _groupService.DeleteGroupAsync(id);

        var request = new GroupByIdRequest
        {
            Id = id
        };

        return Ok(new BaseResponse<GroupByIdRequest, Group?>(request, group));
    }

    [HttpPost("{id}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddUserAsync([FromRoute] int id, [FromRoute] Guid userId)
    {
        var group = await _groupService.AddUserAsync(id,userId);

        var request = new UserRequest
        {
            GroupId = id,
            UserId = userId
        };

        return Ok(new BaseResponse<UserRequest, Group>(request, group));
    }

    [HttpDelete("{id}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserRequest, Group>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveUserAsync([FromRoute] int id, [FromRoute] Guid userId)
    {
        var group = await _groupService.RemoveUserAsync(id,userId);

        var request = new UserRequest
        {
            GroupId = id,
            UserId = userId
        };

        return Ok(new BaseResponse<UserRequest, Group>(request, group));
    }
}
