﻿using Application.Abstractions.Data;
using Contract.Services.Shipment.GetShipments;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ShipmentRepository : IShipmentRepository
{
    private readonly AppDbContext _context;

    public ShipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Shipment shipment)
    {
        _context.Add(shipment);
    }

    public async Task<Shipment> GetByIdAsync(Guid shipmentId)
    {
        return await _context.Shipments
        .AsSplitQuery()
        .AsNoTracking()
        .Include(s => s.Shipper)
        .Include(s => s.ShipmentDetails)
            .ThenInclude(sd => sd.Product)
                .ThenInclude(p => p.Images)
        .Include(s => s.ShipmentDetails)
            .ThenInclude(sd => sd.Phase)
        .Include(s => s.ShipmentDetails)
            .ThenInclude(sd => sd.Set)
            .ThenInclude(s => s.SetProducts)
                .ThenInclude(sp => sp.Product)
                .ThenInclude(p => p.Images)
        .Include(s => s.ShipmentDetails)
            .ThenInclude(sd => sd.MaterialHistory)
             .ThenInclude(mh => mh.Material)
        .SingleOrDefaultAsync(s => s.Id == shipmentId);
    }

    public async Task<bool> IsShipmentIdExistAsync(Guid shipmentId)
    {
        return await _context.Shipments.AnyAsync(s => s.Id == shipmentId);  
    }

    public async Task<(List<Shipment>, int)> SearchShipmentAsync(GetShipmentsQuery request)
    {
        var query = _context.Shipments.Where(s => s.Status == request.Status);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => s.Id.ToString().Contains(request.SearchTerm));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var shipments = await query
            .Include(s => s.FromCompany)
            .Include(s => s.ToCompany)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (shipments, totalPages);
    }

    public void Update(Shipment shipment)
    {
        _context.Shipments.Update(shipment);
    }
}
