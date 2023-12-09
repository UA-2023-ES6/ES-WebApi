using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Services;
using System.Net.Mime;

namespace OneCampus.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    private readonly UserInfo _userInfo;

    public QuestionController(IQuestionService questionService, UserInfo userInfo)
    {
        _questionService = questionService.ThrowIfNull().Value;
        _userInfo = userInfo.ThrowIfNull().Value;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateQuestionRequest, Question>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateQuestionAsync([FromBody] CreateQuestionRequest request)
    {
        var question = await _questionService.CreateQuestionAsync(_userInfo.Id, request.GroupId, request.Content);

        return Ok(new BaseResponse<CreateQuestionRequest, Question>(request, question!));
    }

    [HttpGet("group/{groupId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnumerableResponse<MessagesByGroupRequest, Question>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindGroupAsync([FromRoute] int groupId)
    {
        var questions = await _questionService.FindQuestionsByGroupAsync(_userInfo.Id, groupId);

        var request = new QuestionsByGroupRequest
        {
            GroupId = groupId
        };

        return Ok(new EnumerableResponse<QuestionsByGroupRequest, Question>(request, questions, questions.Count()));
    }
}
