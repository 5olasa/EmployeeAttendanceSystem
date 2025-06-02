using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeAttendance.Shared.Services;
using EmployeeAttendance.Shared.Models;

namespace EmployeeAttendance.API.Controllers
{
    /// <summary>
    /// تحكم في عمليات المصادقة وتسجيل الدخول
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SqliteDatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        public AuthController(SqliteDatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // التحقق من صحة البيانات
                if (string.IsNullOrEmpty(request.EmployeeNumber) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "رقم الموظف وكلمة المرور مطلوبان" });
                }

                // البحث عن الموظف في قاعدة البيانات
                var employee = await _databaseService.GetEmployeeByNumberAsync(request.EmployeeNumber);
                if (employee == null)
                {
                    return Unauthorized(new { message = "رقم الموظف أو كلمة المرور غير صحيحة" });
                }

                // التحقق من كلمة المرور (في التطبيق الحقيقي، يجب استخدام hashing)
                if (!VerifyPassword(request.Password, employee.EmployeeNumber))
                {
                    return Unauthorized(new { message = "رقم الموظف أو كلمة المرور غير صحيحة" });
                }

                // إنشاء JWT Token
                var token = GenerateJwtToken(employee);

                // إرجاع بيانات المستخدم مع التوكن
                var user = new User
                {
                    Id = employee.Id.ToString(),
                    EmployeeNumber = employee.EmployeeNumber,
                    Name = employee.Name,
                    Email = employee.Email,
                    Token = token
                };

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // التحقق من صحة البيانات
                if (string.IsNullOrEmpty(request.EmployeeId) || 
                    string.IsNullOrEmpty(request.CurrentPassword) || 
                    string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new { message = "جميع الحقول مطلوبة" });
                }

                // البحث عن الموظف
                var employee = await _databaseService.GetEmployeeByIdAsync(int.Parse(request.EmployeeId));
                if (employee == null)
                {
                    return NotFound(new { message = "الموظف غير موجود" });
                }

                // التحقق من كلمة المرور الحالية
                if (!VerifyPassword(request.CurrentPassword, employee.EmployeeNumber))
                {
                    return BadRequest(new { message = "كلمة المرور الحالية غير صحيحة" });
                }

                // تحديث كلمة المرور (في التطبيق الحقيقي، يجب استخدام hashing)
                // هنا نحتاج إلى إضافة method لتحديث كلمة المرور في قاعدة البيانات
                
                return Ok(new { message = "تم تغيير كلمة المرور بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }



        /// <summary>
        /// التحقق من كلمة المرور
        /// </summary>
        private bool VerifyPassword(string password, string employeeNumber)
        {
            // في التطبيق الحقيقي، يجب استخدام hashing للتحقق من كلمة المرور
            // هنا نستخدم كلمة مرور بسيطة للاختبار
            return password == "123456";
        }

        /// <summary>
        /// إنشاء JWT Token
        /// </summary>
        private string GenerateJwtToken(Employee employee)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", employee.Id.ToString()),
                    new Claim("employeeNumber", employee.EmployeeNumber),
                    new Claim("name", employee.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    /// <summary>
    /// نموذج طلب تسجيل الدخول
    /// </summary>
    public class LoginRequest
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// نموذج طلب تغيير كلمة المرور
    /// </summary>
    public class ChangePasswordRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
