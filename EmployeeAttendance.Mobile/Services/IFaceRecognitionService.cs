namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// واجهة خدمة التعرف على الوجه
    /// </summary>
    public interface IFaceRecognitionService
    {
        /// <summary>
        /// اكتشاف الوجه في الصورة
        /// </summary>
        Task<bool> DetectFaceAsync(byte[] imageData);

        /// <summary>
        /// استخراج بيانات ترميز الوجه
        /// </summary>
        Task<string> ExtractFaceEncodingAsync(byte[] imageData);

        /// <summary>
        /// مقارنة بيانات ترميز الوجه
        /// </summary>
        Task<double> CompareFaceEncodingsAsync(string encoding1, string encoding2);

        /// <summary>
        /// التحقق من تطابق الوجه
        /// </summary>
        Task<bool> VerifyFaceAsync(string storedEncoding, byte[] imageData);
    }
}
