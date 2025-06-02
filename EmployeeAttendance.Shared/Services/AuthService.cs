using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using EmployeeAttendance.Shared.Models;

namespace EmployeeAttendance.Shared.Services
{
    /// <summary>
    /// خدمة المصادقة وإدارة المستخدمين
    /// </summary>
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly DatabaseService _databaseService;

        // المستخدم الحالي
        private User? _currentUser;

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User? CurrentUser => _currentUser;

        /// <summary>
        /// إنشاء خدمة المصادقة
        /// </summary>
        public AuthService(string connectionString, DatabaseService databaseService)
        {
            _connectionString = connectionString;
            _databaseService = databaseService;
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // البحث عن المستخدم
                var user = await GetUserByUsernameAsync(username);

                if (user == null || !user.IsActive)
                {
                    return false;
                }

                // التحقق من كلمة المرور
                if (user.VerifyPassword(password))
                {
                    // تحديث تاريخ آخر تسجيل دخول
                    user.LastLogin = DateTime.Now;
                    await UpdateUserLastLoginAsync(user.Id);

                    // تعيين المستخدم الحالي
                    _currentUser = user;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل الدخول: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// تسجيل الخروج
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
        }

        /// <summary>
        /// إنشاء مستخدم جديد
        /// </summary>
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                // التحقق من عدم وجود مستخدم بنفس اسم المستخدم
                var existingUser = await GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                // تعيين كلمة المرور
                user.SetPassword(password);

                // إضافة المستخدم إلى قاعدة البيانات
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("INSERT INTO Users (Username, PasswordHash, PasswordSalt, FullName, Email, Role, IsActive, CreatedAt) VALUES (@Username, @PasswordHash, @PasswordSalt, @FullName, @Email, @Role, @IsActive, @CreatedAt); SELECT SCOPE_IDENTITY();", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", user.Username));
                        command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
                        command.Parameters.Add(new SqlParameter("@PasswordSalt", user.PasswordSalt));
                        command.Parameters.Add(new SqlParameter("@FullName", user.FullName));
                        command.Parameters.Add(new SqlParameter("@Email", user.Email));
                        command.Parameters.Add(new SqlParameter("@Role", (int)user.Role));
                        command.Parameters.Add(new SqlParameter("@IsActive", user.IsActive));
                        command.Parameters.Add(new SqlParameter("@CreatedAt", user.CreatedAt));

                        // الحصول على معرف المستخدم الجديد
                        var result = await command.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            user.Id = Convert.ToInt32(result);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إنشاء المستخدم: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// الحصول على مستخدم بواسطة اسم المستخدم
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@Username", username));

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return ReadUserFromDataReader(reader);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في الحصول على المستخدم: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// تحديث تاريخ آخر تسجيل دخول
        /// </summary>
        private async Task<bool> UpdateUserLastLoginAsync(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("UPDATE Users SET LastLogin = @LastLogin WHERE Id = @Id", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@LastLogin", DateTime.Now));
                        command.Parameters.Add(new SqlParameter("@Id", userId));

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحديث تاريخ آخر تسجيل دخول: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// قراءة بيانات المستخدم من قارئ البيانات
        /// </summary>
        private User ReadUserFromDataReader(SqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString() ?? string.Empty,
                PasswordHash = reader["PasswordHash"].ToString() ?? string.Empty,
                PasswordSalt = reader["PasswordSalt"].ToString() ?? string.Empty,
                FullName = reader["FullName"].ToString() ?? string.Empty,
                Email = reader["Email"].ToString() ?? string.Empty,
                Role = (UserRole)Convert.ToInt32(reader["Role"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                LastLogin = reader["LastLogin"] != DBNull.Value ? Convert.ToDateTime(reader["LastLogin"]) : null
            };
        }

        /// <summary>
        /// إنشاء جدول المستخدمين إذا لم يكن موجودًا
        /// </summary>
        public async Task<bool> EnsureUsersTableExistsAsync()
        {
            try
            {
                // التحقق من وجود جدول المستخدمين
                bool tableExists = await _databaseService.CheckTableExistsAsync("Users");

                Console.WriteLine($"جدول المستخدمين موجود: {tableExists}");

                if (!tableExists)
                {
                    // إنشاء جدول المستخدمين
                    string createTableSql = @"
                    CREATE TABLE Users (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Username NVARCHAR(50) NOT NULL UNIQUE,
                        PasswordHash NVARCHAR(MAX) NOT NULL,
                        PasswordSalt NVARCHAR(MAX) NOT NULL,
                        FullName NVARCHAR(100) NOT NULL,
                        Email NVARCHAR(100),
                        Role INT NOT NULL DEFAULT 0,
                        IsActive BIT NOT NULL DEFAULT 1,
                        CreatedAt DATETIME NOT NULL,
                        LastLogin DATETIME NULL
                    )";

                    int result = await _databaseService.ExecuteNonQueryAsync(createTableSql);
                    Console.WriteLine($"تم إنشاء جدول المستخدمين: {result}");

                    // إنشاء مستخدم مدير افتراضي
                    User adminUser = new User
                    {
                        Username = "admin",
                        FullName = "مدير النظام",
                        Email = "admin@example.com",
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    bool userCreated = await CreateUserAsync(adminUser, "admin123");
                    Console.WriteLine($"تم إنشاء المستخدم الافتراضي: {userCreated}");

                    return true;
                }
                else
                {
                    // التحقق من وجود المستخدم الافتراضي
                    var adminUser = await GetUserByUsernameAsync("admin");
                    if (adminUser == null)
                    {
                        // إنشاء مستخدم مدير افتراضي
                        User newAdminUser = new User
                        {
                            Username = "admin",
                            FullName = "مدير النظام",
                            Email = "admin@example.com",
                            Role = UserRole.Admin,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        };

                        bool userCreated = await CreateUserAsync(newAdminUser, "admin123");
                        Console.WriteLine($"تم إنشاء المستخدم الافتراضي: {userCreated}");
                    }
                    else
                    {
                        Console.WriteLine($"المستخدم الافتراضي موجود: {adminUser.Username}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إنشاء جدول المستخدمين: {ex.Message}");
                return false;
            }
        }
    }
}
