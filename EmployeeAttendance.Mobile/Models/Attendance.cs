using System.Text.Json.Serialization;

namespace EmployeeAttendance.Mobile.Models
{
    /// <summary>
    /// نموذج بيانات الحضور
    /// </summary>
    public class Attendance
    {
        /// <summary>
        /// معرف الحضور
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// تاريخ الحضور
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// وقت الحضور
        /// </summary>
        [JsonPropertyName("checkInTime")]
        public DateTime? CheckInTime { get; set; }

        /// <summary>
        /// وقت الانصراف
        /// </summary>
        [JsonPropertyName("checkOutTime")]
        public DateTime? CheckOutTime { get; set; }

        /// <summary>
        /// حالة الحضور
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// طريقة الحضور
        /// </summary>
        [JsonPropertyName("checkInMethod")]
        public string CheckInMethod { get; set; }

        /// <summary>
        /// طريقة الانصراف
        /// </summary>
        [JsonPropertyName("checkOutMethod")]
        public string CheckOutMethod { get; set; }

        /// <summary>
        /// مدة العمل (بالدقائق)
        /// </summary>
        [JsonPropertyName("workDurationMinutes")]
        public int? WorkDurationMinutes { get; set; }

        /// <summary>
        /// مدة التأخير (بالدقائق)
        /// </summary>
        [JsonPropertyName("lateMinutes")]
        public int? LateMinutes { get; set; }

        /// <summary>
        /// مدة الخروج المبكر (بالدقائق)
        /// </summary>
        [JsonPropertyName("earlyDepartureMinutes")]
        public int? EarlyDepartureMinutes { get; set; }

        /// <summary>
        /// مدة العمل الإضافي (بالدقائق)
        /// </summary>
        [JsonPropertyName("overtimeMinutes")]
        public int? OvertimeMinutes { get; set; }
    }
}
