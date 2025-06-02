using System.Text.Json.Serialization;

namespace EmployeeAttendance.Mobile.Models
{
    /// <summary>
    /// نموذج بيانات المستخدم
    /// </summary>
    public class User
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <summary>
        /// رمز الوصول
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// نوع المستخدم
        /// </summary>
        [JsonPropertyName("userType")]
        public string UserType { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }
    }
}
