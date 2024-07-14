﻿using Application.Abstractions.Data;
using Contract.Services.MonthEmployeeSalary.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MonthlyEmployeeSalaryRepository : IMonthlyEmployeeSalaryRepository
{
    private readonly AppDbContext _context;
    public MonthlyEmployeeSalaryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddRange(List<MonthlyEmployeeSalary> monthlyEmployeeSalaries)
    {
        _context.MonthlyEmployeeSalaries.AddRange(monthlyEmployeeSalaries);
    }

    public async Task<MonthlyEmployeeSalary> GetMonthlyEmployeeSalaryByUserIdAsync(string userId, int month, int year)
    {
        var query = await _context.MonthlyEmployeeSalaries
            .AsNoTracking()
            .Include(mes => mes.User)
                .ThenInclude(u => u.Attendances)
            .Include(mes => mes.User)
                .ThenInclude(u => u.EmployeeProducts)
                .ThenInclude(ep => ep.Product)
                .ThenInclude(p => p.ProductPhaseSalaries)
                .ThenInclude(pp => pp.Phase)
            .Where(u => u.User.Id == userId && u.User.IsActive == true && u.User.Attendances.Any(a => a.Date.Month == month && a.Date.Year == year) ||
                                   u.User.EmployeeProducts.Any(ep => ep.Date.Month == month && ep.Date.Year == year)).ToListAsync();
        return query.FirstOrDefault();
    }

    public async Task<(List<MonthlyEmployeeSalary>?, int)> SearchMonthlySalary(GetMonthlySalaryQuery request)
    {
        var query = _context.MonthlyEmployeeSalaries
            .Include(mes => mes.User)
            .Where(mes => mes.User.IsActive);

        if (!string.IsNullOrEmpty(request.userId))
        {
            query = query.Where(mes => mes.User.Id == request.userId);
        }
        if (!string.IsNullOrEmpty(request.fullName))
        {
            query = query.Where(mes => mes.User.FirstName.ToLower().Trim().Contains(request.fullName.ToLower().Trim()) ||
                            mes.User.LastName.ToLower().Trim().Contains(request.fullName.ToLower().Trim()) ||
                            (mes.User.FirstName.ToLower().Trim() +
                            " " +
                            mes.User.LastName.ToLower().Trim()).Contains(request.fullName.ToLower().Trim()));
        }
        if (request.month.HasValue)
        {
            query = query.Where(mes => mes.Month == request.month);
        }
        if (request.year.HasValue)
        {
            query = query.Where(mes => mes.Year == request.year);
        }

        var totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var result = await query
            .OrderByDescending(mes => mes.Year)
            .ThenByDescending(mes => mes.Month)
            .ThenByDescending(mes => mes.Salary)
            .AsNoTracking()
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();


        return (result, totalPages);


    }
}
