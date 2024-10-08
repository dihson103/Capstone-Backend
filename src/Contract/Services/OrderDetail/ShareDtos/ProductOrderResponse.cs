﻿namespace Contract.Services.OrderDetail.ShareDtos;

public record ProductOrderResponse
(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string ProductDescription,
    string ImageProductUrl,
    int Quantity,
    int ShippedQuantiy,
    decimal UnitPrice,
    string? Note
    );
