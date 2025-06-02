using EmployeeAttendance.Mobile.Services;
using EmployeeAttendance.Mobile.Views;
using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;

namespace EmployeeAttendance.Mobile
{
    /// <summary>
    /// الصدفة الرئيسية للتطبيق
    /// </summary>
    public partial class AppShell : Shell
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// أمر تسجيل الخروج
        /// </summary>
        public ICommand LogoutCommand { get; }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public AppShell(ISettingsService settingsService)
        {
            try
            {
                _settingsService = settingsService;
                InitializeComponent();

                // تسجيل الصفحات للتنقل
                Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
                Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
                Routing.RegisterRoute(nameof(AttendancePage), typeof(AttendancePage));
                Routing.RegisterRoute(nameof(AttendanceHistoryPage), typeof(AttendanceHistoryPage));
                Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
                Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

                // تعريف أمر تسجيل الخروج
                LogoutCommand = new Command(OnLogoutClicked);
                BindingContext = this;

                // تسجيل معالج الأخطاء غير المتوقعة
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var exception = args.ExceptionObject as Exception;
                    System.Diagnostics.Debug.WriteLine($"Unhandled exception in AppShell: {exception?.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {exception?.StackTrace}");
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AppShell constructor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// معالج حدث النقر على زر تسجيل الخروج
        /// </summary>
        private async void OnLogoutClicked()
        {
            // عرض رسالة تأكيد
            bool result = await DisplayAlert("تسجيل الخروج", "هل أنت متأكد من تسجيل الخروج؟", "نعم", "لا");

            if (result)
            {
                try
                {
                    // تنفيذ عملية تسجيل الخروج
                    // حذف بيانات الجلسة
                    _settingsService.ClearSessionData();

                    // حذف بيانات تسجيل الدخول المحفوظة إذا لم يكن خيار "تذكرني" مفعلاً
                    if (!_settingsService.GetRememberLoginEnabled())
                    {
                        _settingsService.ClearSavedLoginCredentials();
                    }

                    // العودة إلى صفحة تسجيل الدخول
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("خطأ", $"حدث خطأ أثناء تسجيل الخروج: {ex.Message}", "موافق");
                }
            }
        }
    }
}
