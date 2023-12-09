using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Services;
using System.Net.Mime;

namespace OneCampus.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    private readonly UserInfo _userInfo;

    public MessageController(IMessageService messageService, UserInfo userInfo)
    {
        _messageService = messageService.ThrowIfNull().Value;
        _userInfo = userInfo.ThrowIfNull().Value;
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateMessageRequest, Message>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMessageAsync([FromBody] CreateMessageRequest request)
    {
        var message = await _messageService.CreateMessageAsync(request.GroupId, request.Content, _userInfo.Id);

        return Ok(new BaseResponse<CreateMessageRequest, Message>(request, message!));
    }

    [HttpGet("group/{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnumerableResponse<MessagesByGroupRequest, Message>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindGroupAsync([FromRoute] int groupId)
    {
        var messages = await _messageService.FindMessagesByGroupAsync(groupId);

        var request = new MessagesByGroupRequest
        {
            GroupId = groupId
        };

        return Ok(new EnumerableResponse<MessagesByGroupRequest, Message>(request, messages, messages.Count()));
    }
}
