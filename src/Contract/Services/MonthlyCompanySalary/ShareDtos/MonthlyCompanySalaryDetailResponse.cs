﻿namespace Contract.Services.MonthlyCompanySalary.ShareDtos;

public record MonthlyCompanySalaryDetailResponse
(
    Guid CompanyId,
    int Month,
    int Year,
    decimal Salary,
    StatusSalary Status,
    string? Note,
    decimal TotalSalaryProduct,
    decimal TotalSalaryMaterial,
    decimal TotalSalaryBroken,
    decimal TotalSalaryTotal,
    double TotalProduct,
    double TotalMaterial,
    double TotalBroken,
    double RateProduct,
    double RateBroken,
    List<MaterialExportReponse> MaterialResponses,
    List<ProductExportResponse> ProductExportResponses,
    List<ProductExportResponse> ProductBrokenResponses
    );
