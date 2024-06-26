﻿using Contract.Abstractions.Shared.Utils;
using Contract.Services.Attendance.Create;
using Contract.Services.Attendance.Update;
using Domain.Abstractions.Entities;
using Domain.Exceptions.Common;


namespace Domain.Entities
{
    public class Attendance : EntityAuditBaseWithoutId
    {
        public int SlotId { get; set; }
        public string UserId { get; set; }
        public DateOnly Date { get; set; }
        public double HourOverTime { get; set; } = 0;
        public Slot? Slot { get; set; }
        public User? User { get; set; }
        public bool IsAttendance { get; set; } = false;
        public bool IsOverTime { get; set; } = false;
        public bool IsSalaryByProduct { get; set; } = false;
        public bool IsManufacture { get; set; } = false;

        public static Attendance Create(CreateAttendanceWithoutSlotIdRequest createAttendanceRequest, string Date, int SlotId, string CreatedBy)
        {
            return new Attendance()
            {
                SlotId = SlotId,
                UserId = createAttendanceRequest.UserId,
                Date = ConvertStringToDateTimeOnly(Date),
                HourOverTime = createAttendanceRequest.HourOverTime,
                IsAttendance = createAttendanceRequest.IsAttendance,
                IsOverTime = createAttendanceRequest.HourOverTime > 0 ? true : false,
                IsSalaryByProduct = createAttendanceRequest.IsSalaryByProduct,
                IsManufacture = createAttendanceRequest.IsManufacture,
                CreatedBy = CreatedBy,
                CreatedDate = DateUtils.GetNow(),
            };
        }

        public void Update(UpdateAttendanceWithoutSlotIdRequest updateAttendanceRequest, string updatedBy)
        {
            HourOverTime = updateAttendanceRequest.HourOverTime;
            IsAttendance = updateAttendanceRequest.IsAttendance;
            IsOverTime = updateAttendanceRequest.HourOverTime > 0 ? true : false;
            IsManufacture = updateAttendanceRequest.IsManufacture;
            IsSalaryByProduct = updateAttendanceRequest.IsSalaryByProduct;
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;
        }

        private static DateOnly ConvertStringToDateTimeOnly(string dateString)
        {
            string format = "dd/MM/yyyy";

            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }
            else
            {
                throw new WrongFormatDateException();
            }
        }
    }
}
