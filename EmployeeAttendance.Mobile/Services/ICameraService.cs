namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// واجهة خدمة الكاميرا
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// حدث التقاط إطار جديد
        /// </summary>
        event EventHandler<byte[]> FrameCaptured;

        /// <summary>
        /// حدث اكتشاف وجه
        /// </summary>
        event EventHandler<bool> FaceDetected;

        /// <summary>
        /// حدث حدوث خطأ
        /// </summary>
        event EventHandler<Exception> ErrorOccurred;

        /// <summary>
        /// تهيئة الكاميرا
        /// </summary>
        Task<bool> InitializeAsync();

        /// <summary>
        /// بدء التقاط الصور
        /// </summary>
        Task<bool> StartCaptureAsync();

        /// <summary>
        /// إيقاف التقاط الصور
        /// </summary>
        void StopCapture();

        /// <summary>
        /// التقاط صورة واحدة
        /// </summary>
        Task<byte[]> CaptureImageAsync();

        /// <summary>
        /// التحقق من حالة تشغيل الكاميرا
        /// </summary>
        bool IsRunning();
    }
}
