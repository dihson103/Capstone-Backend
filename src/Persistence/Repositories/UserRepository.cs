﻿using Application.Abstractions.Data;
using Contract.Services.User.GetUsers;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
    }

    public async Task<User?> GetUserActiveByIdAsync(string id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id.Equals(id) && user.IsActive == true);
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .SingleOrDefaultAsync(user => user.Id.Equals(id));
    }

    public async Task<User> GetUserByPhoneNumberOrIdAsync(string search)
    {
        return await _context.Users
            .Include(user => user.Role)
            .SingleOrDefaultAsync(user => (user.Phone == search || user.Id == search) && user.IsActive);
    }

    public Task<List<User>> GetUsersByCompanyId(Guid companyId)
    {
        return _context.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .Where(user => user.CompanyId == companyId && user.IsActive == true)
            .ToListAsync();
    }

    public async Task<bool> IsAllUserActiveAsync(List<string> userIds)
    {
        var countUsers = await _context.Users.CountAsync(user => userIds.Contains(user.Id) && user.IsActive == true);
        return countUsers == userIds.Count;
    }

    public async Task<bool> IsAllUserActiveByCompanyId(List<string> userIds, Guid companyId)
    {
        var countUsers = await _context.Users.CountAsync(user => userIds.Contains(user.Id) && user.IsActive == true && user.CompanyId == companyId);
        return countUsers == userIds.Count;
    }

    public async Task<bool> IsPhoneNumberExistAsync(string phoneNumber)
    {
        return await _context.Users
            .AnyAsync(u => u.Phone ==  phoneNumber);    
    }

    public async Task<bool> IsUpdatePhoneNumberExistAsync(string phone, string userId)
    {
        return await _context.Users
            .AnyAsync (u => u.Phone == phone && u.Id != userId);
    }

    public async Task<bool> IsUserActiveAsync(string id)
    {
        return await _context.Users.AnyAsync(u => u.Id.Equals(id) && u.IsActive == true);
    }

    public async Task<bool> IsUserExistAsync(string id)
    {
        return await _context.Users.AnyAsync(user => user.Id.Equals(id));
    }

    public async Task<(List<User>?, int)> SearchUsersAsync(GetUsersQuery request)
    {
        var query = _context.Users
            .Where(user => user.IsActive == request.IsActive && user.RoleId == request.RoleId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(user => user.Id.Contains(request.SearchTerm));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var users = await query
            .OrderBy(user => user.CreatedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (users, totalPages);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
