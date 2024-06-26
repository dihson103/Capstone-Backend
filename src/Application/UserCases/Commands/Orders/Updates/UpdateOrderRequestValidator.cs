﻿using Application.Abstractions.Data;
using Contract.Services.Order.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Updates;

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(IOrderRepository _orderRepository, ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.")
            .MustAsync(async (orderId, cancellationToken) =>
            {
                return await _orderRepository.IsOrderExist(orderId);
            }).WithMessage("Order does not exist.");
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required")
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsExistAsync(companyId);
            }).WithMessage("Company does not exist.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}
