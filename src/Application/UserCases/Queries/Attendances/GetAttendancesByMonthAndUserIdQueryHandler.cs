﻿using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDtos;
using Domain.Exceptions.Attendances;

namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendancesByMonthAndUserIdQueryHandler
    (IAttendanceRepository _attendanceRepository, IMapper _mapper
    ) : IQueryHandler<GetAttendancesByMonthAndUserIdQuery, List<AttendanceUserReponse>>
{
    public async Task<Result.Success<List<AttendanceUserReponse>>> Handle(GetAttendancesByMonthAndUserIdQuery request, CancellationToken cancellationToken)
    {
        var attendances = await _attendanceRepository.GetAttendanceByMonthAndUserIdAsync(request.Month, request.Year, request.UserId);
        if (attendances is null || attendances.Count <= 0)
        {
            throw new AttendanceNotFoundException();
        }
        var attendancesReponse = attendances
                .GroupBy(a => new { a.Date.Month, a.Date.Year, a.UserId })
                .Select(group => new AttendanceUserReponse(
                    Month: group.Key.Month,
                    Year: group.Key.Year,
                    UserId: group.Key.UserId,
                    Attendances: group
                        .GroupBy(a => a.Date.ToString("dd/MM/yyyy"))
                        .Select(dateGroup => new AttendanceUserReportResponse(
                            Date: dateGroup.Key,
                            AttedanceDateReports: dateGroup.Select(a => new AttedanceDateReport(
                                SlotId: a.SlotId,
                                IsPresent: a.IsAttendance,
                                isSalaryByProduct: a.IsSalaryByProduct,
                                isOverTime: a.IsOverTime
                            )).ToList()
                        )).ToList()
                )).ToList();

        return Result.Success<List<AttendanceUserReponse>>.Get(attendancesReponse);
    }
}