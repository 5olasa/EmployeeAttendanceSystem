using System.Text.Json.Serialization;

namespace EmployeeAttendance.Mobile.Models
{
    /// <summary>
    /// نموذج بيانات الموظف
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// معرف الموظف
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// رقم الموظف
        /// </summary>
        [JsonPropertyName("employeeNumber")]
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// الاسم الأول
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// الاسم الأخير
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// الاسم الكامل
        /// </summary>
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// المسمى الوظيفي
        /// </summary>
        [JsonPropertyName("position")]
        public string Position { get; set; }

        /// <summary>
        /// القسم
        /// </summary>
        [JsonPropertyName("department")]
        public string Department { get; set; }

        /// <summary>
        /// تاريخ التوظيف
        /// </summary>
        [JsonPropertyName("hireDate")]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// المدير المباشر
        /// </summary>
        [JsonPropertyName("manager")]
        public string Manager { get; set; }

        /// <summary>
        /// بيانات ترميز الوجه
        /// </summary>
        [JsonPropertyName("faceEncodingData")]
        public string FaceEncodingData { get; set; }

        /// <summary>
        /// مسار الصورة الشخصية
        /// </summary>
        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; }
    }
}
