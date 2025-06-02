using System;
using System.Collections.Generic;

namespace EmployeeAttendance.Shared.Models
{
    /// <summary>
    /// نموذج بيانات الموظف
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// معرف الموظف
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// الرقم الوظيفي للموظف
        /// </summary>
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// اسم الموظف
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ التوظيف
        /// </summary>
        public DateTime HireDate { get; set; } = DateTime.Now;

        /// <summary>
        /// نوع الشفت
        /// </summary>
        public int ShiftId { get; set; }

        /// <summary>
        /// الراتب الشهري
        /// </summary>
        public decimal MonthlySalary { get; set; }

        /// <summary>
        /// عدد أيام الإجازة المتاحة
        /// </summary>
        public int AvailableVacationDays { get; set; }

        /// <summary>
        /// مسار صورة الوجه المخزنة
        /// </summary>
        public string FaceImagePath { get; set; } = string.Empty;

        /// <summary>
        /// بيانات صورة الوجه المشفرة بـ Base64
        /// </summary>
        public string FaceImageBase64 { get; set; } = string.Empty;

        /// <summary>
        /// بيانات التعرف على الوجه (للاستخدام في خوارزميات التعرف)
        /// </summary>
        public byte[]? FaceEncodingData { get; set; }

        /// <summary>
        /// تاريخ آخر تحديث للبيانات
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// قائمة سجلات الحضور للموظف
        /// </summary>
        public virtual ICollection<Attendance> AttendanceRecords { get; set; }

        public Employee()
        {
            AttendanceRecords = new List<Attendance>();
            LastUpdated = DateTime.Now;
        }
    }
}