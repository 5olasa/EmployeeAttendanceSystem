using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeAttendance.Shared.Services;
using EmployeeAttendance.Shared.Models;

namespace EmployeeAttendance.API.Controllers
{
    /// <summary>
    /// تحكم في عمليات الحضور والانصراف
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly SqliteDatabaseService _databaseService;

        public AttendanceController(SqliteDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// تسجيل الحضور
        /// </summary>
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmployeeId))
                {
                    return BadRequest(new { message = "معرف الموظف مطلوب" });
                }

                if (!int.TryParse(request.EmployeeId, out int employeeId))
                {
                    return BadRequest(new { message = "معرف الموظف غير صحيح" });
                }

                // التحقق من وجود الموظف
                var employee = await _databaseService.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                {
                    return NotFound(new { message = "الموظف غير موجود" });
                }

                // إنشاء سجل حضور جديد
                var attendance = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = DateTime.Today,
                    CheckInTime = DateTime.Now,
                    Device = request.Device ?? "Mobile App",
                    IsManualCheckIn = false,
                    Notes = request.Notes ?? string.Empty
                };

                // تسجيل الحضور في قاعدة البيانات
                var attendanceId = await _databaseService.RecordCheckInAsync(attendance);
                attendance.Id = attendanceId;
                attendance.Employee = employee;

                return Ok(attendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// تسجيل الانصراف
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmployeeId))
                {
                    return BadRequest(new { message = "معرف الموظف مطلوب" });
                }

                if (!int.TryParse(request.EmployeeId, out int employeeId))
                {
                    return BadRequest(new { message = "معرف الموظف غير صحيح" });
                }

                // البحث عن سجل الحضور لليوم الحالي
                var attendanceRecords = await _databaseService.GetAttendanceRecordsAsync(employeeId, DateTime.Today, DateTime.Today);
                var todayAttendance = attendanceRecords.FirstOrDefault();

                if (todayAttendance == null || !todayAttendance.CheckInTime.HasValue)
                {
                    return BadRequest(new { message = "لم يتم تسجيل الحضور لهذا اليوم" });
                }

                if (todayAttendance.CheckOutTime.HasValue)
                {
                    return BadRequest(new { message = "تم تسجيل الانصراف مسبقاً لهذا اليوم" });
                }

                // تسجيل الانصراف
                var success = await _databaseService.RecordCheckOutAsync(
                    todayAttendance.Id,
                    DateTime.Now,
                    string.Empty,
                    false,
                    request.Device ?? "Mobile App"
                );

                if (!success)
                {
                    return StatusCode(500, new { message = "فشل في تسجيل الانصراف" });
                }

                // تحديث سجل الحضور
                todayAttendance.CheckOutTime = DateTime.Now;

                return Ok(todayAttendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// الحصول على سجلات الحضور لموظف
        /// </summary>
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAttendanceRecords(string employeeId, [FromQuery] int month = 0, [FromQuery] int year = 0)
        {
            try
            {
                if (!int.TryParse(employeeId, out int empId))
                {
                    return BadRequest(new { message = "معرف الموظف غير صحيح" });
                }

                // تحديد الشهر والسنة إذا لم يتم تمريرهما
                if (month == 0) month = DateTime.Now.Month;
                if (year == 0) year = DateTime.Now.Year;

                // حساب تاريخ البداية والنهاية للشهر
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var records = await _databaseService.GetAttendanceRecordsAsync(empId, startDate, endDate);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// الحصول على إحصائيات الحضور لموظف
        /// </summary>
        [HttpGet("stats/{employeeId}")]
        public async Task<IActionResult> GetAttendanceStats(string employeeId)
        {
            try
            {
                if (!int.TryParse(employeeId, out int empId))
                {
                    return BadRequest(new { message = "معرف الموظف غير صحيح" });
                }

                // الحصول على سجلات الحضور للشهر الحالي
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var records = await _databaseService.GetAttendanceRecordsAsync(empId, startDate, endDate);

                // حساب الإحصائيات
                var stats = new AttendanceStats
                {
                    TotalWorkingDays = records.Count,
                    PresentDays = records.Count(r => r.CheckInTime.HasValue),
                    AbsentDays = GetWorkingDaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - records.Count(r => r.CheckInTime.HasValue),
                    LateDays = records.Count(r => r.IsLate),
                    TotalWorkingHours = records.Where(r => r.CheckInTime.HasValue && r.CheckOutTime.HasValue)
                                             .Sum(r => r.WorkHours),
                    AverageWorkingHours = records.Where(r => r.CheckInTime.HasValue && r.CheckOutTime.HasValue)
                                                .Average(r => r.WorkHours)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// حساب عدد أيام العمل في الشهر (باستثناء الجمعة والسبت)
        /// </summary>
        private int GetWorkingDaysInMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var workingDays = 0;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }
    }

    /// <summary>
    /// نموذج طلب تسجيل الحضور
    /// </summary>
    public class CheckInRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FaceEncodingData { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج طلب تسجيل الانصراف
    /// </summary>
    public class CheckOutRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string FaceEncodingData { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
    }
}
