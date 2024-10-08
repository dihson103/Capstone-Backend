﻿using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Attendances;
using Domain.Exceptions.Users;
using FluentValidation;
using System.Data;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

internal sealed class CreateAttendanceDefaultCommandHandler(
    IAttendanceRepository _attendanceRepository,
    IUserRepository _userRepository,
    IValidator<CreateAttendanceDefaultRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateAttendanceDefaultCommand>
{
    public async Task<Result.Success> Handle(CreateAttendanceDefaultCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateAttendanceDefaultRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var formattedDate = DateUtil.ConvertStringToDateTimeOnly(request.CreateAttendanceDefaultRequest.Date);
        var dateNow = DateOnly.FromDateTime(DateTime.Now);


        var userIds = request.CreateAttendanceDefaultRequest.CreateAttendances.Select(x => x.UserId).ToList();
        var roleName = request.RoleName;
        var companyId = request.CompanyId;

        await CheckPermissionAsync(request, formattedDate, dateNow);
        await CheckSalaryCalculatedAsync(formattedDate);

        var emps = request.CreateAttendanceDefaultRequest.CreateAttendances;
        var attendances = emps
                        .Select(emp => Attendance
                        .Create(
                                    emp,
                                    request.CreateAttendanceDefaultRequest.Date,
                                    request.CreateAttendanceDefaultRequest.SlotId,
                                    request.CreatedBy))
                        .ToList();

        await _attendanceRepository.AddRangeAsync(attendances);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
    private bool IsOverTwoDays(DateOnly DateRequest, DateOnly DateNow)
    {
        var daysDifference = DateNow.ToDateTime(TimeOnly.MinValue) - DateRequest.ToDateTime(TimeOnly.MinValue);

        return daysDifference.TotalDays > 2;
    }
    private async Task CheckPermissionAsync(CreateAttendanceDefaultCommand request, DateOnly formattedDate, DateOnly dateNow)
    {
        var userIds = request.CreateAttendanceDefaultRequest.CreateAttendances.Select(x => x.UserId).ToList();
        var roleName = request.RoleName;
        var companyId = request.CompanyId;
        if (roleName != "MAIN_ADMIN")
        {
            var check = await _userRepository.IsAllUserActiveByCompanyId(userIds, companyId);
            if (!check)
            {
                throw new UserNotPermissionException("Bạn không có quyền tạo điểm danh cho user của công ty này.");
            }

            if (IsOverTwoDays(formattedDate, dateNow))
            {
                throw new UserNotPermissionException("Bạn không thể tạo hoặc sửa điểm danh do đã quá 2 ngày.");
            }
        }
    }
    private async Task CheckSalaryCalculatedAsync(DateOnly formattedDate)
    {
        var isSalaryCalculated = await _attendanceRepository.IsSalaryCalculatedForMonth(formattedDate.Month, formattedDate.Year);
        if (isSalaryCalculated)
        {
            throw new AttendanceCannotCreateOrUpdateException();
        }
    }

}