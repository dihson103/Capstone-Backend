﻿using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.MaterialHistories.Create;

public sealed class CreateMaterialHistoryCommandHandler(
    IMaterialHistoryRepository _materialHistoryRepository,
    IMaterialRepository _materialRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateMaterialHistoryRequest> _validator
    ) : ICommandHandler<CreateMaterialHistoryCommand>
{
    public async Task<Result.Success> Handle(CreateMaterialHistoryCommand request, CancellationToken cancellationToken)
    {
        var createMaterialHistoryCommand = request.createMaterialHistoryRequest;
        var validationResult = await _validator.ValidateAsync(createMaterialHistoryCommand);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
        var materialHistory = MaterialHistory.Create(createMaterialHistoryCommand);
        _materialHistoryRepository.AddMaterialHistory(materialHistory);

        var material = await _materialRepository.GetMaterialByIdAsync(createMaterialHistoryCommand.MaterialId);
        material.UpdateQuantityInStock1(createMaterialHistoryCommand.Quantity);
        _materialRepository.UpdateMaterial(material);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
