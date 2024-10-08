﻿using Contract.Services.Company.ShareDtos;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipmentDetail.Share;
using Contract.Services.User.SharedDto;

namespace Contract.Services.Shipment.GetShipmentDetail;

public record ShipmentDetailResponse(
    CompanyResponse From,
    CompanyResponse To,
    UserResponseWithoutSalary Shipper,
    DateTime ShipDate,
    string StatusDescription,
    Status Status,
    List<DetailResponse>? Details);
