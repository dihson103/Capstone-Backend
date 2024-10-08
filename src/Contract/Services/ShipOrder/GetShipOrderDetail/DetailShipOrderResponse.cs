﻿using Contract.Services.Company.ShareDtos;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
using Contract.Services.ShipOrder.Share;

namespace Contract.Services.ShipOrder.GetShipOrderDetail;

public record DetailShipOrderResponse(
    Guid ShipOrderId,
    string ShipperId,
    string ShipperName,
    DateTime ShipDate,
    bool IsAccepted,
    Status Status,
    string StatusDescription,
    DeliveryMethod DeliveryMethod,
    string DeliveryMethodDescription,
    List<ShipOrderDetailWithImageLinkResponse> ShipOrderDetailResponses,
    CompanyResponse CompanyResponse);
