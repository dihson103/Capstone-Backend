﻿using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipmentRepository
{
    void Add(Shipment shipment);
    Task<Shipment> GetByIdAsync(Guid shipmentId);
    void Update(Shipment shipment);
    Task<bool> IsShipmentIdExistAsync(Guid shipmentId);
}
