using EmployeeAttendance.Mobile.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Threading.Tasks;

namespace EmployeeAttendance.Mobile
{
    /// <summary>
    /// تطبيق الموبايل لنظام حضور الموظفين
    /// </summary>
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public App(ISettingsService settingsService)
        {
            try
            {
                // تسجيل معالج الاستثناءات غير المعالجة
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                _settingsService = settingsService;
                InitializeComponent();

                // تطبيق الوضع الليلي إذا كان مفعلاً
                if (_settingsService.GetDarkModeEnabled())
                {
                    UserAppTheme = AppTheme.Dark;
                }
                else
                {
                    UserAppTheme = AppTheme.Light;
                }

                // تعيين الصفحة الرئيسية
                MainPage = new AppShell(_settingsService);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in App constructor: {ex.Message}");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {exception?.Message}");
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Unobserved task exception: {e.Exception.Message}");
            e.SetObserved(); // منع تعطل التطبيق
        }

        /// <summary>
        /// يتم استدعاؤها عند بدء تشغيل التطبيق
        /// </summary>
        protected override void OnStart()
        {
            try
            {
                // تأخير التنقل لضمان اكتمال تهيئة Shell
                // استخدام تأخير أطول للتأكد من اكتمال تهيئة التطبيق
                Task.Delay(2000).ContinueWith(t =>
                {
                    Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            // التحقق من وجود جلسة نشطة
                            var (accessToken, userId) = _settingsService.GetSessionData();

                            // تأكد من أن Shell.Current غير فارغ
                            if (Shell.Current != null)
                            {
                                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(userId))
                                {
                                    // إذا كانت هناك جلسة نشطة، انتقل إلى الصفحة الرئيسية
                                    await Shell.Current.GoToAsync("//HomePage");
                                }
                                else
                                {
                                    // إذا لم تكن هناك جلسة نشطة، انتقل إلى صفحة تسجيل الدخول
                                    await Shell.Current.GoToAsync("//LoginPage");
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Shell.Current is null");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error navigating: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                            // محاولة إعادة تعيين الصفحة الرئيسية
                            try
                            {
                                MainPage = new AppShell(_settingsService);
                            }
                            catch (Exception innerEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error resetting MainPage: {innerEx.Message}");
                            }
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnStart: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// يتم استدعاؤها عند إيقاف التطبيق مؤقتًا
        /// </summary>
        protected override void OnSleep()
        {
            // حفظ الإعدادات عند إيقاف التطبيق مؤقتًا
        }

        /// <summary>
        /// يتم استدعاؤها عند استئناف التطبيق
        /// </summary>
        protected override void OnResume()
        {
            // التحقق من صلاحية الجلسة عند استئناف التطبيق
        }
    }
}

