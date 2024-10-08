﻿using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.Query;
using Contract.Services.Attendance.ShareDto;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IAttendanceRepository
{
    void AddAttendance(Attendance attendance);
    void UpdateAttendance(Attendance attendance);
    Task<List<Attendance>?> GetAttendanceByUserIdAndDateAsync(string userId, DateOnly date);
    Task<List<Attendance>> GetAttendanceByDate(DateOnly date);
    Task<List<Attendance>> GetAttendanceByDateAndSlotId(DateOnly date, int slotId);
    Task<(List<Attendance>?, int)> SearchAttendancesAsync(GetAttendanceRequest request);
    Task<bool> IsCanUpdateAttendance(string userId, int slotId, DateOnly date);
    Task AddRangeAsync(List<Attendance> attendances);
    Task<bool> IsAllAttendancesExist(int slotID, DateOnly date, List<string> userIds);
    Task<(List<AttendanceOverallResponse>?, int)> GetAttendanceOverallAsync(DateOnly? startDate, DateOnly? endDate, int pageIndex, int pageSize);
    Task<List<Attendance>> GetAttendancesByKeys(int slotId, DateOnly date, List<string> userIds);
    Task<bool> IsAllCanUpdateAttendance(List<string> userIds, int slotId, DateOnly date);
    void UpdateRange(List<Attendance> attendances);
    Task<bool> IsAttendanceAlreadyExisted(List<string> userIds, int slotId, DateOnly date);
    Task<List<Attendance>> GetAttendanceByMonthAndUserIdAsync(int month, int year, string userId);
    Task<bool> IsSalaryCalculatedForMonth(int month, int year);
}
