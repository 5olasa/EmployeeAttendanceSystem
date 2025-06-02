using EmployeeAttendance.Mobile.Services;
using EmployeeAttendance.Mobile.ViewModels;
using EmployeeAttendance.Mobile.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;

namespace EmployeeAttendance.Mobile
{
    /// <summary>
    /// نقطة دخول تطبيق MAUI
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// إنشاء تطبيق MAUI
        /// </summary>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemiBold");
                    fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
                    fonts.AddFont("OpenSans-Medium.ttf", "OpenSansMedium");
                    fonts.AddFont("Cairo-Regular.ttf", "CairoRegular");
                    fonts.AddFont("Cairo-Bold.ttf", "CairoBold");
                    fonts.AddFont("Cairo-SemiBold.ttf", "CairoSemiBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // تسجيل الخدمات
            RegisterServices(builder.Services);

            // تسجيل نماذج العرض
            RegisterViewModels(builder.Services);

            // تسجيل الصفحات
            RegisterViews(builder.Services);

            return builder.Build();
        }

        /// <summary>
        /// تسجيل الخدمات في حاوية الاعتماد
        /// </summary>
        private static void RegisterServices(IServiceCollection services)
        {
            // تسجيل خدمات التطبيق
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IFaceRecognitionService, FaceRecognitionService>();
        }

        /// <summary>
        /// تسجيل نماذج العرض في حاوية الاعتماد
        /// </summary>
        private static void RegisterViewModels(IServiceCollection services)
        {
            // تسجيل نماذج العرض
            services.AddTransient<LoginViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<AttendanceViewModel>();
            services.AddTransient<AttendanceHistoryViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<SettingsViewModel>();
        }

        /// <summary>
        /// تسجيل الصفحات في حاوية الاعتماد
        /// </summary>
        private static void RegisterViews(IServiceCollection services)
        {
            // تسجيل الصفحات
            services.AddTransient<LoginPage>();
            services.AddTransient<HomePage>();
            services.AddTransient<AttendancePage>();
            services.AddTransient<AttendanceHistoryPage>();
            services.AddTransient<ProfilePage>();
            services.AddTransient<SettingsPage>();
        }
    }
}
