﻿using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Services;
using System.Net.Mime;

namespace OneCampus.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AnswerController : ControllerBase
{
    private readonly IAnswerService _answerService;


    public AnswerController(IAnswerService answerService)
    {
        _answerService = answerService.ThrowIfNull().Value;

    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<CreateAnswerRequest, Answer>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateAnswerAsync([FromBody] CreateAnswerRequest request)
    {
        var answer = await _answerService.CreateAnswerAsync(request.QuestionId, request.Content, request.UserId);

        return Ok(new BaseResponse<CreateAnswerRequest, Answer>(request, answer!));
    }

    [HttpGet("question/{questionId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnumerableResponse<MessagesByGroupRequest, Answer>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> FindAnswersAsync([FromRoute] int questionId)
    {
        var answers = await _answerService.FindAnswersByQuestionAsync(questionId);

        var request = new AnswersByQuestionRequest
        {
            QuestionId = questionId
        };

        return Ok(new EnumerableResponse<AnswersByQuestionRequest, Answer>(request, answers, answers.Count()));
    }
}

