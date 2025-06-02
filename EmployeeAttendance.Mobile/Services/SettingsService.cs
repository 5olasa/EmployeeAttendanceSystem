using Microsoft.Maui.Storage;

namespace EmployeeAttendance.Mobile.Services
{
    /// <summary>
    /// خدمة إعدادات التطبيق
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private const string ServerAddressKey = "ServerAddress";
        private const string NotificationsEnabledKey = "NotificationsEnabled";
        private const string RememberLoginEnabledKey = "RememberLoginEnabled";
        private const string AutoSyncEnabledKey = "AutoSyncEnabled";
        private const string DarkModeEnabledKey = "DarkModeEnabled";
        private const string CameraResolutionKey = "CameraResolution";
        private const string FaceDetectionSensitivityKey = "FaceDetectionSensitivity";
        private const string SyncIntervalKey = "SyncInterval";
        private const string SavedEmployeeNumberKey = "SavedEmployeeNumber";
        private const string SavedPasswordKey = "SavedPassword";
        private const string AccessTokenKey = "AccessToken";
        private const string UserIdKey = "UserId";

        private readonly string _defaultServerAddress = "http://localhost:5000";
        private readonly bool _defaultNotificationsEnabled = true;
        private readonly bool _defaultRememberLoginEnabled = true;
        private readonly bool _defaultAutoSyncEnabled = true;
        private readonly bool _defaultDarkModeEnabled = false;
        private readonly string _defaultCameraResolution = "متوسطة (1280x720)";
        private readonly double _defaultFaceDetectionSensitivity = 70.0;
        private readonly string _defaultSyncInterval = "كل ساعة";

        /// <summary>
        /// الحصول على عنوان الخادم
        /// </summary>
        public string GetServerAddress()
        {
            return Preferences.Get(ServerAddressKey, _defaultServerAddress);
        }

        /// <summary>
        /// تعيين عنوان الخادم
        /// </summary>
        public void SetServerAddress(string serverAddress)
        {
            Preferences.Set(ServerAddressKey, serverAddress);
        }

        /// <summary>
        /// الحصول على حالة تفعيل الإشعارات
        /// </summary>
        public bool GetNotificationsEnabled()
        {
            return Preferences.Get(NotificationsEnabledKey, _defaultNotificationsEnabled);
        }

        /// <summary>
        /// تعيين حالة تفعيل الإشعارات
        /// </summary>
        public void SetNotificationsEnabled(bool enabled)
        {
            Preferences.Set(NotificationsEnabledKey, enabled);
        }

        /// <summary>
        /// الحصول على حالة تذكر بيانات تسجيل الدخول
        /// </summary>
        public bool GetRememberLoginEnabled()
        {
            return Preferences.Get(RememberLoginEnabledKey, _defaultRememberLoginEnabled);
        }

        /// <summary>
        /// تعيين حالة تذكر بيانات تسجيل الدخول
        /// </summary>
        public void SetRememberLoginEnabled(bool enabled)
        {
            Preferences.Set(RememberLoginEnabledKey, enabled);
        }

        /// <summary>
        /// الحصول على حالة المزامنة التلقائية
        /// </summary>
        public bool GetAutoSyncEnabled()
        {
            return Preferences.Get(AutoSyncEnabledKey, _defaultAutoSyncEnabled);
        }

        /// <summary>
        /// تعيين حالة المزامنة التلقائية
        /// </summary>
        public void SetAutoSyncEnabled(bool enabled)
        {
            Preferences.Set(AutoSyncEnabledKey, enabled);
        }

        /// <summary>
        /// الحصول على حالة الوضع الليلي
        /// </summary>
        public bool GetDarkModeEnabled()
        {
            return Preferences.Get(DarkModeEnabledKey, _defaultDarkModeEnabled);
        }

        /// <summary>
        /// تعيين حالة الوضع الليلي
        /// </summary>
        public void SetDarkModeEnabled(bool enabled)
        {
            Preferences.Set(DarkModeEnabledKey, enabled);
        }

        /// <summary>
        /// الحصول على دقة الكاميرا
        /// </summary>
        public string GetCameraResolution()
        {
            return Preferences.Get(CameraResolutionKey, _defaultCameraResolution);
        }

        /// <summary>
        /// تعيين دقة الكاميرا
        /// </summary>
        public void SetCameraResolution(string resolution)
        {
            Preferences.Set(CameraResolutionKey, resolution);
        }

        /// <summary>
        /// الحصول على حساسية التعرف على الوجه
        /// </summary>
        public double GetFaceDetectionSensitivity()
        {
            return Preferences.Get(FaceDetectionSensitivityKey, _defaultFaceDetectionSensitivity);
        }

        /// <summary>
        /// تعيين حساسية التعرف على الوجه
        /// </summary>
        public void SetFaceDetectionSensitivity(double sensitivity)
        {
            Preferences.Set(FaceDetectionSensitivityKey, sensitivity);
        }

        /// <summary>
        /// الحصول على فترة المزامنة
        /// </summary>
        public string GetSyncInterval()
        {
            return Preferences.Get(SyncIntervalKey, _defaultSyncInterval);
        }

        /// <summary>
        /// تعيين فترة المزامنة
        /// </summary>
        public void SetSyncInterval(string interval)
        {
            Preferences.Set(SyncIntervalKey, interval);
        }

        /// <summary>
        /// حفظ بيانات تسجيل الدخول
        /// </summary>
        public void SaveLoginCredentials(string employeeNumber, string password)
        {
            Preferences.Set(SavedEmployeeNumberKey, employeeNumber);
            Preferences.Set(SavedPasswordKey, password);
        }

        /// <summary>
        /// الحصول على بيانات تسجيل الدخول المحفوظة
        /// </summary>
        public (string EmployeeNumber, string Password) GetSavedLoginCredentials()
        {
            var employeeNumber = Preferences.Get(SavedEmployeeNumberKey, string.Empty);
            var password = Preferences.Get(SavedPasswordKey, string.Empty);
            return (employeeNumber, password);
        }

        /// <summary>
        /// مسح بيانات تسجيل الدخول المحفوظة
        /// </summary>
        public void ClearSavedLoginCredentials()
        {
            Preferences.Remove(SavedEmployeeNumberKey);
            Preferences.Remove(SavedPasswordKey);
        }

        /// <summary>
        /// حفظ بيانات الجلسة
        /// </summary>
        public void SaveSessionData(string accessToken, string userId)
        {
            Preferences.Set(AccessTokenKey, accessToken);
            Preferences.Set(UserIdKey, userId);
        }

        /// <summary>
        /// الحصول على بيانات الجلسة
        /// </summary>
        public (string AccessToken, string UserId) GetSessionData()
        {
            var accessToken = Preferences.Get(AccessTokenKey, string.Empty);
            var userId = Preferences.Get(UserIdKey, string.Empty);
            return (accessToken, userId);
        }

        /// <summary>
        /// مسح بيانات الجلسة
        /// </summary>
        public void ClearSessionData()
        {
            Preferences.Remove(AccessTokenKey);
            Preferences.Remove(UserIdKey);
        }

        /// <summary>
        /// استعادة الإعدادات الافتراضية
        /// </summary>
        public void RestoreDefaultSettings()
        {
            SetServerAddress(_defaultServerAddress);
            SetNotificationsEnabled(_defaultNotificationsEnabled);
            SetRememberLoginEnabled(_defaultRememberLoginEnabled);
            SetAutoSyncEnabled(_defaultAutoSyncEnabled);
            SetDarkModeEnabled(_defaultDarkModeEnabled);
            SetCameraResolution(_defaultCameraResolution);
            SetFaceDetectionSensitivity(_defaultFaceDetectionSensitivity);
            SetSyncInterval(_defaultSyncInterval);
        }

        /// <summary>
        /// مسح جميع الإعدادات
        /// </summary>
        public void ClearAllSettings()
        {
            Preferences.Clear();
        }
    }
}
