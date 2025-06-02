namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// واجهة خدمة إعدادات التطبيق
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// الحصول على عنوان الخادم
        /// </summary>
        string GetServerAddress();

        /// <summary>
        /// تعيين عنوان الخادم
        /// </summary>
        void SetServerAddress(string serverAddress);

        /// <summary>
        /// الحصول على حالة تفعيل الإشعارات
        /// </summary>
        bool GetNotificationsEnabled();

        /// <summary>
        /// تعيين حالة تفعيل الإشعارات
        /// </summary>
        void SetNotificationsEnabled(bool enabled);

        /// <summary>
        /// الحصول على حالة تذكر بيانات تسجيل الدخول
        /// </summary>
        bool GetRememberLoginEnabled();

        /// <summary>
        /// تعيين حالة تذكر بيانات تسجيل الدخول
        /// </summary>
        void SetRememberLoginEnabled(bool enabled);

        /// <summary>
        /// الحصول على حالة المزامنة التلقائية
        /// </summary>
        bool GetAutoSyncEnabled();

        /// <summary>
        /// تعيين حالة المزامنة التلقائية
        /// </summary>
        void SetAutoSyncEnabled(bool enabled);

        /// <summary>
        /// الحصول على حالة الوضع الليلي
        /// </summary>
        bool GetDarkModeEnabled();

        /// <summary>
        /// تعيين حالة الوضع الليلي
        /// </summary>
        void SetDarkModeEnabled(bool enabled);

        /// <summary>
        /// الحصول على دقة الكاميرا
        /// </summary>
        string GetCameraResolution();

        /// <summary>
        /// تعيين دقة الكاميرا
        /// </summary>
        void SetCameraResolution(string resolution);

        /// <summary>
        /// الحصول على حساسية التعرف على الوجه
        /// </summary>
        double GetFaceDetectionSensitivity();

        /// <summary>
        /// تعيين حساسية التعرف على الوجه
        /// </summary>
        void SetFaceDetectionSensitivity(double sensitivity);

        /// <summary>
        /// الحصول على فترة المزامنة
        /// </summary>
        string GetSyncInterval();

        /// <summary>
        /// تعيين فترة المزامنة
        /// </summary>
        void SetSyncInterval(string interval);

        /// <summary>
        /// حفظ بيانات تسجيل الدخول
        /// </summary>
        void SaveLoginCredentials(string employeeNumber, string password);

        /// <summary>
        /// الحصول على بيانات تسجيل الدخول المحفوظة
        /// </summary>
        (string EmployeeNumber, string Password) GetSavedLoginCredentials();

        /// <summary>
        /// مسح بيانات تسجيل الدخول المحفوظة
        /// </summary>
        void ClearSavedLoginCredentials();

        /// <summary>
        /// حفظ بيانات الجلسة
        /// </summary>
        void SaveSessionData(string accessToken, string userId);

        /// <summary>
        /// الحصول على بيانات الجلسة
        /// </summary>
        (string AccessToken, string UserId) GetSessionData();

        /// <summary>
        /// مسح بيانات الجلسة
        /// </summary>
        void ClearSessionData();

        /// <summary>
        /// استعادة الإعدادات الافتراضية
        /// </summary>
        void RestoreDefaultSettings();

        /// <summary>
        /// مسح جميع الإعدادات
        /// </summary>
        void ClearAllSettings();
    }
}
