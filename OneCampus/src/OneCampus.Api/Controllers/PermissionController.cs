using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests.Permissions;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Services;
using System.Net.Mime;

namespace OneCampus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService.ThrowIfNull().Value;
        }

        [HttpPost("group/{groupId}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<PermissionResponseRequest, UserPermissions>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AllowPermissionsAsync(
            [FromRoute] int groupId,
            [FromRoute] Guid userId,
            [FromBody] PermissionRequest request)
        {
            var permissions = await _permissionService.AllowPermissionsAsync(userId, groupId, request.Permissions);

            var responseRequest = new PermissionResponseRequest
            {
                UserId = userId,
                GroupId = groupId,
                Permissions = request.Permissions
            };

            return Ok(new BaseResponse<PermissionResponseRequest, UserPermissions>(responseRequest, permissions));
        }

        [HttpDelete("group/{groupId}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<PermissionResponseRequest, UserPermissions>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DenyPermissionsAsync(
            [FromRoute] int groupId,
            [FromRoute] Guid userId,
            [FromBody] PermissionRequest request)
        {
            var permissions = await _permissionService.DenyPermissionsAsync(userId, groupId, request.Permissions);

            var responseRequest = new PermissionResponseRequest
            {
                UserId = userId,
                GroupId = groupId,
                Permissions = request.Permissions
            };

            return Ok(new BaseResponse<PermissionResponseRequest, UserPermissions>(responseRequest, permissions));
        }

        [HttpGet("group/{groupId}/me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<MePermissionsRequest, UserPermissions>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetPermissionsAsync([FromRoute] int groupId)
        {
            var permissions = await _permissionService.GetMyPermissionsAsync(groupId);

            var responseRequest = new MePermissionsRequest
            {
                GroupId = groupId
            };

            return Ok(new BaseResponse<MePermissionsRequest, UserPermissions>(responseRequest, permissions));
        }

        [HttpGet("group/{groupId}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<UserPermissionsRequest, UserPermissions>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetUserPermissionsAsync([FromRoute] int groupId, [FromRoute] Guid userId)
        {
            var permissions = await _permissionService.GetPermissionsAsync(userId, groupId);

            var responseRequest = new UserPermissionsRequest
            {
                UserId = userId,
                GroupId = groupId
            };

            return Ok(new BaseResponse<UserPermissionsRequest, UserPermissions>(responseRequest, permissions));
        }
    }
}
