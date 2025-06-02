using EmployeeAttendance.Mobile.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// خدمة الاتصال بواجهة برمجة التطبيقات
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingsService _settingsService;
        private readonly string _baseUrl;

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public ApiService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _baseUrl = _settingsService.GetServerAddress();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        public async Task<User> LoginAsync(string employeeNumber, string password)
        {
            try
            {
                var loginData = new { EmployeeNumber = employeeNumber, Password = password };
                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Login failed: {response.StatusCode} - {errorContent}");
                }

                var user = await response.Content.ReadFromJsonAsync<User>();
                if (user == null)
                {
                    throw new InvalidOperationException("Failed to deserialize user data");
                }

                // حفظ التوكن في headers للطلبات المستقبلية
                if (!string.IsNullOrEmpty(user.Token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على معلومات الموظف
        /// </summary>
        public async Task<Employee> GetEmployeeAsync(string employeeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/employees/{employeeId}");
                response.EnsureSuccessStatusCode();

                var employee = await response.Content.ReadFromJsonAsync<Employee>();
                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEmployeeAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تسجيل الحضور
        /// </summary>
        public async Task<Attendance> CheckInAsync(string employeeId, string faceEncodingData)
        {
            try
            {
                var checkInData = new {
                    EmployeeId = employeeId,
                    FaceEncodingData = faceEncodingData,
                    Device = "Mobile App",
                    Notes = ""
                };
                var content = new StringContent(JsonSerializer.Serialize(checkInData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/attendance/checkin", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Check-in failed: {response.StatusCode} - {errorContent}");
                }

                var attendance = await response.Content.ReadFromJsonAsync<Attendance>();
                if (attendance == null)
                {
                    throw new InvalidOperationException("Failed to deserialize attendance data");
                }

                return attendance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckInAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تسجيل الانصراف
        /// </summary>
        public async Task<Attendance> CheckOutAsync(string employeeId, string faceEncodingData)
        {
            try
            {
                var checkOutData = new {
                    EmployeeId = employeeId,
                    FaceEncodingData = faceEncodingData,
                    Device = "Mobile App"
                };
                var content = new StringContent(JsonSerializer.Serialize(checkOutData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/attendance/checkout", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Check-out failed: {response.StatusCode} - {errorContent}");
                }

                var attendance = await response.Content.ReadFromJsonAsync<Attendance>();
                if (attendance == null)
                {
                    throw new InvalidOperationException("Failed to deserialize attendance data");
                }

                return attendance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckOutAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على سجل الحضور
        /// </summary>
        public async Task<List<Attendance>> GetAttendanceRecordsAsync(string employeeId, int month, int year)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/attendance/{employeeId}?month={month}&year={year}");
                response.EnsureSuccessStatusCode();

                var attendanceRecords = await response.Content.ReadFromJsonAsync<List<Attendance>>();
                return attendanceRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAttendanceRecordsAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// الحصول على إحصائيات الحضور
        /// </summary>
        public async Task<AttendanceStats> GetAttendanceStatsAsync(string employeeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/attendance/stats/{employeeId}");
                response.EnsureSuccessStatusCode();

                var attendanceStats = await response.Content.ReadFromJsonAsync<AttendanceStats>();
                return attendanceStats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAttendanceStatsAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string employeeId, string currentPassword, string newPassword)
        {
            try
            {
                var passwordData = new { EmployeeId = employeeId, CurrentPassword = currentPassword, NewPassword = newPassword };
                var content = new StringContent(JsonSerializer.Serialize(passwordData), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/changepassword", content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ChangePasswordAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// تحديث الصورة الشخصية
        /// </summary>
        public async Task<bool> UpdateProfileImageAsync(string employeeId, byte[] imageData)
        {
            try
            {
                var imageContent = new MultipartFormDataContent();
                imageContent.Add(new ByteArrayContent(imageData), "image", "profile.jpg");

                var response = await _httpClient.PostAsync($"/api/employees/{employeeId}/image", imageContent);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProfileImageAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// اختبار الاتصال بالخادم
        /// </summary>
        public async Task<bool> TestConnectionAsync(string serverAddress)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync($"{serverAddress}/api/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestConnectionAsync: {ex.Message}");
                return false;
            }
        }
    }
}
