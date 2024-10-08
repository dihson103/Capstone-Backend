﻿namespace Contract.Services.ProductPhaseSalary.ShareDtos;

public record ProductPhaseSalaryResponse
(
    Guid PhaseId,
    string PhaseName,
    string PhaseDescription,
    decimal SalaryPerProduct
    );