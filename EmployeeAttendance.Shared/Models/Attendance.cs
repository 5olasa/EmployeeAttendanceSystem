using System;

namespace EmployeeAttendance.Shared.Models
{
    /// <summary>
    /// نموذج بيانات الحضور
    /// </summary>
    public class Attendance
    {
        /// <summary>
        /// معرف سجل الحضور
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// مرجع للموظف
        /// </summary>
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// تاريخ الحضور
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Today;

        /// <summary>
        /// وقت الحضور
        /// </summary>
        public DateTime? CheckInTime { get; set; }

        /// <summary>
        /// وقت الانصراف
        /// </summary>
        public DateTime? CheckOutTime { get; set; }

        /// <summary>
        /// مسار صورة الحضور (إن وجدت)
        /// </summary>
        public string CheckInImagePath { get; set; } = string.Empty;

        /// <summary>
        /// مسار صورة الانصراف (إن وجدت)
        /// </summary>
        public string CheckOutImagePath { get; set; } = string.Empty;

        /// <summary>
        /// هل تم التحقق من الحضور يدويًا
        /// </summary>
        public bool IsManualCheckIn { get; set; }

        /// <summary>
        /// هل تم التحقق من الانصراف يدويًا
        /// </summary>
        public bool IsManualCheckOut { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// حساب عدد ساعات العمل
        /// </summary>
        public double WorkHours
        {
            get
            {
                if (CheckInTime.HasValue && CheckOutTime.HasValue)
                {
                    return (CheckOutTime.Value - CheckInTime.Value).TotalHours;
                }
                return 0;
            }
        }

        /// <summary>
        /// هل الموظف متأخر
        /// </summary>
        public bool IsLate { get; set; }

        /// <summary>
        /// مقدار التأخير بالدقائق
        /// </summary>
        public int LateMinutes { get; set; }

        /// <summary>
        /// الجهاز الذي تم تسجيل الحضور منه
        /// </summary>
        public string Device { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ آخر تحديث للبيانات
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Attendance()
        {
            Date = DateTime.Today;
            LastUpdated = DateTime.Now;
        }
    }
}
