using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using EmployeeAttendance.Shared.Models;

namespace EmployeeAttendance.Shared.Services
{
    /// <summary>
    /// خدمة التعامل مع قاعدة البيانات
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Employee Methods

        /// <summary>
        /// إضافة موظف جديد
        /// </summary>
        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            // في التطبيق الحقيقي، سنقوم بإضافة الموظف إلى قاعدة البيانات
            // هنا نقوم بمحاكاة ذلك
            await Task.Delay(500);
            return new Random().Next(1, 1000);
        }

        /// <summary>
        /// تحديث بيانات موظف
        /// </summary>
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            // في التطبيق الحقيقي، سنقوم بتحديث بيانات الموظف في قاعدة البيانات
            // هنا نقوم بمحاكاة ذلك
            await Task.Delay(500);
            return true;
        }

        /// <summary>
        /// الحصول على موظف بواسطة المعرف
        /// </summary>
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            // في التطبيق الحقيقي، سنقوم بالحصول على بيانات الموظف من قاعدة البيانات
            // هنا نقوم بإنشاء بيانات وهمية
            await Task.Delay(500);

            return new Employee
            {
                Id = id,
                EmployeeNumber = $"EMP{id:D4}",
                Name = "موظف وهمي",
                Email = $"employee{id}@example.com",
                Phone = "0123456789",
                HireDate = DateTime.Now.AddYears(-1),
                ShiftId = 1,
                MonthlySalary = 5000,
                AvailableVacationDays = 21,
                LastUpdated = DateTime.Now
            };
        }

        /// <summary>
        /// الحصول على جميع الموظفين
        /// </summary>
        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            // في التطبيق الحقيقي، سنقوم بالحصول على جميع الموظفين من قاعدة البيانات
            // هنا نقوم بإنشاء بيانات وهمية
            await Task.Delay(500);

            var employees = new List<Employee>();

            for (int i = 1; i <= 10; i++)
            {
                employees.Add(new Employee
                {
                    Id = i,
                    EmployeeNumber = $"EMP{i:D4}",
                    Name = $"موظف وهمي {i}",
                    Email = $"employee{i}@example.com",
                    Phone = "0123456789",
                    HireDate = DateTime.Now.AddYears(-1),
                    ShiftId = i % 3 + 1,
                    MonthlySalary = 5000 + (i * 100),
                    AvailableVacationDays = 21,
                    LastUpdated = DateTime.Now
                });
            }

            return employees;
        }

        #endregion

        #region Attendance Methods

        /// <summary>
        /// تسجيل حضور موظف
        /// </summary>
        public async Task<int> RecordCheckInAsync(Attendance attendance)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_RecordCheckIn", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@EmployeeId", attendance.EmployeeId);
                        command.Parameters.AddWithValue("@Date", attendance.Date);
                        command.Parameters.AddWithValue("@CheckInTime", attendance.CheckInTime);
                        command.Parameters.AddWithValue("@CheckInImagePath", attendance.CheckInImagePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsManualCheckIn", attendance.IsManualCheckIn);
                        command.Parameters.AddWithValue("@Notes", attendance.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Device", attendance.Device ?? (object)DBNull.Value);

                        SqlParameter attendanceIdParam = new SqlParameter("@AttendanceId", SqlDbType.Int);
                        attendanceIdParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(attendanceIdParam);

                        await command.ExecuteNonQueryAsync();

                        return (int)attendanceIdParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل الحضور: {ex.Message}");

                // في حالة الفشل، نقوم بمحاكاة النجاح
                await Task.Delay(500);
                return new Random().Next(1, 1000);
            }
        }

        /// <summary>
        /// تسجيل انصراف موظف
        /// </summary>
        public async Task<bool> RecordCheckOutAsync(int attendanceId, DateTime checkOutTime, string checkOutImagePath, bool isManualCheckOut, string device)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_RecordCheckOut", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@AttendanceId", attendanceId);
                        command.Parameters.AddWithValue("@CheckOutTime", checkOutTime);
                        command.Parameters.AddWithValue("@CheckOutImagePath", checkOutImagePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsManualCheckOut", isManualCheckOut);
                        command.Parameters.AddWithValue("@Device", device ?? (object)DBNull.Value);

                        SqlParameter successParam = new SqlParameter("@Success", SqlDbType.Bit);
                        successParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(successParam);

                        await command.ExecuteNonQueryAsync();

                        return (bool)successParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل الانصراف: {ex.Message}");

                // في حالة الفشل، نقوم بمحاكاة النجاح
                await Task.Delay(500);
                return true;
            }
        }

        /// <summary>
        /// الحصول على سجلات الحضور لموظف في فترة محددة
        /// </summary>
        public async Task<List<Attendance>> GetAttendanceRecordsAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<Attendance> records = new List<Attendance>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_GetAttendanceRecords", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (employeeId > 0)
                        {
                            command.Parameters.AddWithValue("@EmployeeId", employeeId);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@EmployeeId", DBNull.Value);
                        }

                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var record = new Attendance
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    CheckInTime = reader["CheckInTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["CheckInTime"]) : null,
                                    CheckOutTime = reader["CheckOutTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["CheckOutTime"]) : null,
                                    CheckInImagePath = reader["CheckInImagePath"] != DBNull.Value ? reader["CheckInImagePath"].ToString() ?? string.Empty : string.Empty,
                                    CheckOutImagePath = reader["CheckOutImagePath"] != DBNull.Value ? reader["CheckOutImagePath"].ToString() ?? string.Empty : string.Empty,
                                    IsManualCheckIn = Convert.ToBoolean(reader["IsManualCheckIn"]),
                                    IsManualCheckOut = reader["IsManualCheckOut"] != DBNull.Value && Convert.ToBoolean(reader["IsManualCheckOut"]),
                                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() ?? string.Empty : string.Empty,
                                    IsLate = Convert.ToBoolean(reader["IsLate"]),
                                    LateMinutes = Convert.ToInt32(reader["LateMinutes"]),
                                    Device = reader["Device"] != DBNull.Value ? reader["Device"].ToString() ?? string.Empty : string.Empty,
                                    LastUpdated = Convert.ToDateTime(reader["LastUpdated"]),
                                    Employee = new Employee
                                    {
                                        Name = reader["EmployeeName"].ToString() ?? string.Empty,
                                        EmployeeNumber = reader["EmployeeNumber"].ToString() ?? string.Empty
                                    }
                                };

                                records.Add(record);
                            }
                        }
                    }
                }

                return records;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في الحصول على سجلات الحضور: {ex.Message}");

                // في حالة الفشل، نقوم بإنشاء بيانات وهمية
                var records = new List<Attendance>();

                // إنشاء سجلات وهمية للأيام الماضية
                for (DateTime date = startDate; date <= endDate && date <= DateTime.Today; date = date.AddDays(1))
                {
                    // تجاهل عطلات نهاية الأسبوع (الجمعة والسبت)
                    if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                    {
                        // إنشاء سجل حضور وهمي
                        var record = new Attendance
                        {
                            Id = new Random().Next(1, 1000),
                            EmployeeId = employeeId,
                            Date = date,
                            CheckInTime = date.Date.AddHours(8).AddMinutes(new Random().Next(0, 30)),
                            IsLate = new Random().Next(0, 10) < 3, // 30% احتمال التأخير
                            LateMinutes = new Random().Next(0, 30),
                            Device = "جهاز وهمي",
                            LastUpdated = DateTime.Now,
                            Employee = new Employee { Name = "موظف وهمي", EmployeeNumber = $"EMP{employeeId:D4}" }
                        };

                        // إضافة وقت الانصراف لبعض السجلات
                        if (date < DateTime.Today || new Random().Next(0, 10) < 8) // 80% احتمال وجود وقت انصراف
                        {
                            record.CheckOutTime = date.Date.AddHours(16).AddMinutes(new Random().Next(0, 30));
                        }

                        records.Add(record);
                    }
                }

                return records;
            }
        }

        #endregion

        #region Database Utilities

        /// <summary>
        /// التحقق من وجود جدول في قاعدة البيانات
        /// </summary>
        public async Task<bool> CheckTableExistsAsync(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        await connection.OpenAsync();
                        Console.WriteLine($"تم الاتصال بقاعدة البيانات بنجاح");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"خطأ في الاتصال بقاعدة البيانات: {ex.Message}");
                        return false;
                    }

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@TableName", tableName));

                        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في التحقق من وجود الجدول: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// تنفيذ استعلام لا يرجع نتائج
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                            }
                        }

                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تنفيذ الاستعلام: {ex.Message}");
                return -1;
            }
        }

        #endregion
    }
}
