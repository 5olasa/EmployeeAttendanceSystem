using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeAttendance.Shared.Services;
using EmployeeAttendance.Shared.Models;

namespace EmployeeAttendance.API.Controllers
{
    /// <summary>
    /// تحكم في عمليات إدارة الموظفين
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly SqliteDatabaseService _databaseService;

        public EmployeesController(SqliteDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// الحصول على جميع الموظفين
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _databaseService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// الحصول على موظف بواسطة المعرف
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(string id)
        {
            try
            {
                if (!int.TryParse(id, out int employeeId))
                {
                    return BadRequest(new { message = "معرف الموظف غير صحيح" });
                }

                var employee = await _databaseService.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                {
                    return NotFound(new { message = "الموظف غير موجود" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// إضافة موظف جديد
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "بيانات الموظف مطلوبة" });
                }

                // التحقق من صحة البيانات
                if (string.IsNullOrEmpty(employee.EmployeeNumber) || string.IsNullOrEmpty(employee.Name))
                {
                    return BadRequest(new { message = "رقم الموظف والاسم مطلوبان" });
                }

                var employeeId = await _databaseService.AddEmployeeAsync(employee);
                employee.Id = employeeId;

                return CreatedAtAction(nameof(GetEmployee), new { id = employeeId }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// تحديث بيانات موظف
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            try
            {
                if (employee == null || id != employee.Id)
                {
                    return BadRequest(new { message = "بيانات الموظف غير صحيحة" });
                }

                var existingEmployee = await _databaseService.GetEmployeeByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound(new { message = "الموظف غير موجود" });
                }

                var success = await _databaseService.UpdateEmployeeAsync(employee);
                if (!success)
                {
                    return StatusCode(500, new { message = "فشل في تحديث بيانات الموظف" });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }

        /// <summary>
        /// تحديث صورة الموظف
        /// </summary>
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UpdateEmployeeImage(int id, IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest(new { message = "الصورة مطلوبة" });
                }

                var employee = await _databaseService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound(new { message = "الموظف غير موجود" });
                }

                // قراءة بيانات الصورة
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // تحويل الصورة إلى Base64
                var base64Image = Convert.ToBase64String(imageBytes);
                employee.FaceImageBase64 = base64Image;

                // تحديث بيانات الموظف
                var success = await _databaseService.UpdateEmployeeAsync(employee);
                if (!success)
                {
                    return StatusCode(500, new { message = "فشل في تحديث صورة الموظف" });
                }

                return Ok(new { message = "تم تحديث صورة الموظف بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"حدث خطأ في الخادم: {ex.Message}" });
            }
        }
    }
}
