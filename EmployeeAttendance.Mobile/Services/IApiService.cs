using EmployeeAttendance.Mobile.Models;

namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// واجهة خدمة الاتصال بواجهة برمجة التطبيقات
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        Task<User> LoginAsync(string employeeNumber, string password);

        /// <summary>
        /// الحصول على معلومات الموظف
        /// </summary>
        Task<Employee> GetEmployeeAsync(string employeeId);

        /// <summary>
        /// تسجيل الحضور
        /// </summary>
        Task<Attendance> CheckInAsync(string employeeId, string faceEncodingData);

        /// <summary>
        /// تسجيل الانصراف
        /// </summary>
        Task<Attendance> CheckOutAsync(string employeeId, string faceEncodingData);

        /// <summary>
        /// الحصول على سجل الحضور
        /// </summary>
        Task<List<Attendance>> GetAttendanceRecordsAsync(string employeeId, int month, int year);

        /// <summary>
        /// الحصول على إحصائيات الحضور
        /// </summary>
        Task<AttendanceStats> GetAttendanceStatsAsync(string employeeId);

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        Task<bool> ChangePasswordAsync(string employeeId, string currentPassword, string newPassword);

        /// <summary>
        /// تحديث الصورة الشخصية
        /// </summary>
        Task<bool> UpdateProfileImageAsync(string employeeId, byte[] imageData);

        /// <summary>
        /// اختبار الاتصال بالخادم
        /// </summary>
        Task<bool> TestConnectionAsync(string serverAddress);
    }
}
