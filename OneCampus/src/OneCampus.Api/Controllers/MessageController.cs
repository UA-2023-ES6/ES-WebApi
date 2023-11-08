using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Messages;
using OneCampus.Domain.Services;

namespace OneCampus.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;


    public MessageController(IMessageService messageService)
	{
        _messageService = messageService.ThrowIfNull().Value;

    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateMessageRequest,Message>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMessageAsync([FromBody] CreateMessageRequest request)
    {
        var message = await _messageService.CreateMessageAsync(request.GroupId, request.Content);

        return Ok(new BaseResponse<CreateMessageRequest, Message>(request, message));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<MessagesByGroupRequest, List<Message>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindGroupAsync([FromRoute] int id)
    {
        var messages = await _messageService.FindMessagesByGroupAsync(id);

        var request = new MessagesByGroupRequest
        {
            GroupId = id
        };

        return Ok(new BaseResponse<MessagesByGroupRequest, List<Message>>(request, messages));
    }
}

