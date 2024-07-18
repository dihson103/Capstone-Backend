﻿using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.UpdateStatus;
using Domain.Entities;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Commands.Shipments.UpdateStatus;

internal sealed class UpdateShipmentStatusCommandHandler(
    IShipmentRepository _shipmentRepository,
    IUnitOfWork _unitOfWork) : ICommandHandler<UpdateShipmentStatusCommand>
{
    public async Task<Result.Success> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateStatusRequest;

        var shipment = await GetAndValidateInput(updateRequest, request.Id);

        shipment.UpdateStatus(request.UpdatedBy, updateRequest.Status);

        _shipmentRepository.Update(shipment);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }

    private async Task<Shipment> GetAndValidateInput(UpdateStatusRequest updateRequest, Guid shipmentId)
    {
        if (shipmentId != updateRequest.ShipmentId)
        {
            throw new ShipmentIdConflictException();
        }

        if (!Enum.IsDefined(typeof(Status), updateRequest.Status))
        {
            throw new ShipmentStatusNotFoundException();
        }

        var shipment = await _shipmentRepository.GetByIdAsync(shipmentId);
        if (shipment == null)
        {
            throw new ShipmentNotFoundException();
        }

        if (shipment.IsAccepted)
        {
            throw new ShipmentAlreadyDoneException();
        }

        return shipment;
    }
}
