﻿using OneCampus.Domain;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _AnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IPermissionService _permissionService;

    public AnswerService(
        IAnswerRepository AnswerRepository,
        IUserRepository userRepository,
        IQuestionRepository questionRepository,
        IPermissionService permissionService)
    {
        _AnswerRepository = AnswerRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _questionRepository = questionRepository.ThrowIfNull().Value;
        _permissionService = permissionService.ThrowIfNull().Value;
    }

    public async Task<Answer?> CreateAnswerAsync(Guid userId, int questionId, string content)
    {        
        await ValidateQuestionAccessAsync(userId, questionId);

        content.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        questionId.Throw()
            .IfNegativeOrZero();

        var user = await _userRepository.FindAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("user not found");
        }

        var question = await _questionRepository.FindAsync(questionId);
        if (question is null)
        {
            throw new NotFoundException("question not found");
        }

        await _permissionService.ValidatePermissionAsync(userId, question.GroupId, PermissionType.CreateAnswer);

        return await _AnswerRepository.CreateAsync(content, questionId, userId);
    }

    public async Task<IEnumerable<Answer>> FindAnswersByQuestionAsync(Guid userId, int questionId)
    {
        await ValidateQuestionAccessAsync(userId, questionId);

        questionId.Throw()
            .IfNegativeOrZero();

        var question = await _questionRepository.FindAsync(questionId);
        if (question is null)
        {
            throw new NotFoundException("question not found");
        }

        return await _AnswerRepository.GetAnswersByQuestionAsync(questionId);
    }

    private async Task ValidateQuestionAccessAsync(Guid userId, int questionId)
    {
        userId.Throw()
            .IfDefault();

        questionId.Throw()
            .IfNegativeOrZero();

        var userHasAccess = await _questionRepository.HasAccessAsync(userId, questionId);
        if (!userHasAccess)
        {
            throw new ForbiddenException("the user does not have access to the question");
        }
    }
}
