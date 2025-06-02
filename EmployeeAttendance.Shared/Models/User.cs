using System;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeAttendance.Shared.Models
{
    /// <summary>
    /// نموذج المستخدم للنظام
    /// </summary>
    public class User
    {
        /// <summary>
        /// معرف المستخدم (كنص للتوافق مع تطبيق الموبايل)
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم الرقمي
        /// </summary>
        public int NumericId { get; set; }

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور المشفرة
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// ملح التشفير
        /// </summary>
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل للمستخدم
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// نوع المستخدم (مدير، مشرف، مستخدم عادي)
        /// </summary>
        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>
        /// حالة المستخدم (نشط، معطل)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// تاريخ إنشاء الحساب
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// تاريخ آخر تسجيل دخول
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// رقم الموظف (للتوافق مع تطبيق الموبايل)
        /// </summary>
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// اسم الموظف (للتوافق مع تطبيق الموبايل)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// رمز المصادقة JWT
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// تعيين كلمة المرور وتشفيرها
        /// </summary>
        public void SetPassword(string password)
        {
            // إنشاء ملح عشوائي
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            
            PasswordSalt = Convert.ToBase64String(saltBytes);
            
            // تشفير كلمة المرور مع الملح
            PasswordHash = HashPassword(password, PasswordSalt);
        }

        /// <summary>
        /// التحقق من صحة كلمة المرور
        /// </summary>
        public bool VerifyPassword(string password)
        {
            // تشفير كلمة المرور المدخلة ومقارنتها بالمخزنة
            string hashedPassword = HashPassword(password, PasswordSalt);
            return hashedPassword == PasswordHash;
        }

        /// <summary>
        /// تشفير كلمة المرور باستخدام الملح
        /// </summary>
        private string HashPassword(string password, string salt)
        {
            // دمج كلمة المرور مع الملح
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);
            
            // تشفير باستخدام SHA256
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

    /// <summary>
    /// أنواع المستخدمين
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// مستخدم عادي
        /// </summary>
        User = 0,
        
        /// <summary>
        /// مشرف
        /// </summary>
        Supervisor = 1,
        
        /// <summary>
        /// مدير
        /// </summary>
        Admin = 2
    }
}
