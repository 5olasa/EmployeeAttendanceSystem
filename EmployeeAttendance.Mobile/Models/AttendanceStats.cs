using System.Text.Json.Serialization;

namespace EmployeeAttendance.Mobile.Models
{
    /// <summary>
    /// نموذج بيانات إحصائيات الحضور
    /// </summary>
    public class AttendanceStats
    {
        /// <summary>
        /// معرف الموظف
        /// </summary>
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// الفترة الزمنية
        /// </summary>
        [JsonPropertyName("period")]
        public string Period { get; set; }

        /// <summary>
        /// عدد أيام العمل
        /// </summary>
        [JsonPropertyName("workDays")]
        public int WorkDays { get; set; }

        /// <summary>
        /// عدد أيام الحضور
        /// </summary>
        [JsonPropertyName("attendanceDays")]
        public int AttendanceDays { get; set; }

        /// <summary>
        /// عدد أيام الغياب
        /// </summary>
        [JsonPropertyName("absenceDays")]
        public int AbsenceDays { get; set; }

        /// <summary>
        /// عدد أيام التأخير
        /// </summary>
        [JsonPropertyName("lateDays")]
        public int LateDays { get; set; }

        /// <summary>
        /// عدد أيام الإجازة
        /// </summary>
        [JsonPropertyName("vacationDays")]
        public int VacationDays { get; set; }

        /// <summary>
        /// عدد أيام الإجازة المتبقية
        /// </summary>
        [JsonPropertyName("remainingVacationDays")]
        public int RemainingVacationDays { get; set; }

        /// <summary>
        /// نسبة الحضور
        /// </summary>
        [JsonPropertyName("attendanceRate")]
        public double AttendanceRate { get; set; }

        /// <summary>
        /// إجمالي ساعات العمل
        /// </summary>
        [JsonPropertyName("totalWorkHours")]
        public double TotalWorkHours { get; set; }

        /// <summary>
        /// إجمالي ساعات التأخير
        /// </summary>
        [JsonPropertyName("totalLateHours")]
        public double TotalLateHours { get; set; }

        /// <summary>
        /// إجمالي ساعات العمل الإضافي
        /// </summary>
        [JsonPropertyName("totalOvertimeHours")]
        public double TotalOvertimeHours { get; set; }
    }
}
