using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AlMohasebAlAraby.Shared.Data;
using AlMohasebAlAraby.Desktop.Services;
using AlMohasebAlAraby.Desktop.Views;
using Serilog;

namespace AlMohasebAlAraby.Desktop
{
    /// <summary>
    /// تطبيق المحاسب العربي - الإصدار المصري
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private IConfiguration? _configuration;

        public App()
        {
            // تعيين الثقافة العربية
            SetArabicCulture();

            // معالجة الاستثناءات غير المعالجة
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // تهيئة التطبيق
            InitializeApplication();
        }

        /// <summary>
        /// تعيين الثقافة العربية للتطبيق
        /// </summary>
        private void SetArabicCulture()
        {
            var arabicCulture = new CultureInfo("ar-EG");
            
            // تعيين الثقافة للخيط الحالي
            Thread.CurrentThread.CurrentCulture = arabicCulture;
            Thread.CurrentThread.CurrentUICulture = arabicCulture;
            
            // تعيين الثقافة الافتراضية للتطبيق
            CultureInfo.DefaultThreadCurrentCulture = arabicCulture;
            CultureInfo.DefaultThreadCurrentUICulture = arabicCulture;

            // تعيين اتجاه النص من اليمين لليسار
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage("ar-EG")));
        }

        /// <summary>
        /// تهيئة التطبيق والخدمات
        /// </summary>
        private void InitializeApplication()
        {
            try
            {
                // إنشاء المجلدات اللازمة
                CreateRequiredDirectories();

                // تحميل الإعدادات
                LoadConfiguration();

                // تهيئة نظام السجلات
                InitializeLogging();

                // تهيئة خدمات التطبيق
                ConfigureServices();

                // تهيئة قاعدة البيانات
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة التطبيق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// إنشاء المجلدات المطلوبة للتطبيق
        /// </summary>
        private void CreateRequiredDirectories()
        {
            var directories = new[]
            {
                "Data",
                "Data/Database",
                "Data/Reports",
                "Data/Exports",
                "Data/Backups",
                "Data/Logs",
                "Data/Templates",
                "Data/Images",
                "Data/Images/Products",
                "Data/Images/Company",
                "Data/Attachments"
            };

            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        /// <summary>
        /// تحميل إعدادات التطبيق
        /// </summary>
        private void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        /// <summary>
        /// تهيئة نظام السجلات
        /// </summary>
        private void InitializeLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Data/Logs/AlMohasebAlAraby-.log", 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        /// <summary>
        /// تكوين خدمات التطبيق
        /// </summary>
        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // إضافة الإعدادات
            services.AddSingleton(_configuration!);

            // إضافة نظام السجلات
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            // إضافة قاعدة البيانات
            var connectionString = _configuration!.GetConnectionString("DefaultConnection") 
                ?? "Data Source=Data/Database/AlMohasebAlAraby.db";
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            // إضافة خدمات التطبيق
            services.AddScoped<IAccountingService, AccountingService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IInstallmentService, InstallmentService>();
            services.AddScoped<IPharmacyService, PharmacyService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IBackupService, BackupService>();
            services.AddScoped<ISettingsService, SettingsService>();

            // إضافة النوافذ
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// تهيئة قاعدة البيانات
        /// </summary>
        private void InitializeDatabase()
        {
            using var scope = _serviceProvider!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // إنشاء قاعدة البيانات إذا لم تكن موجودة
            context.Database.EnsureCreated();
            
            // تطبيق أي تحديثات معلقة
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // عرض نافذة تسجيل الدخول
                var loginWindow = _serviceProvider!.GetRequiredService<LoginWindow>();
                var loginResult = loginWindow.ShowDialog();

                if (loginResult == true)
                {
                    // عرض النافذة الرئيسية
                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                else
                {
                    // إغلاق التطبيق إذا لم يتم تسجيل الدخول
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطأ في بدء تشغيل التطبيق");
                MessageBox.Show($"خطأ في بدء تشغيل التطبيق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // تنظيف الموارد
            _serviceProvider?.Dispose();
            Log.CloseAndFlush();
            
            base.OnExit(e);
        }

        /// <summary>
        /// معالج الاستثناءات غير المعالجة في AppDomain
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Log.Fatal(exception, "استثناء غير معالج في AppDomain");
            
            MessageBox.Show($"حدث خطأ غير متوقع: {exception?.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// معالج الاستثناءات غير المعالجة في Dispatcher
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "استثناء غير معالج في Dispatcher");
            
            MessageBox.Show($"حدث خطأ غير متوقع: {e.Exception.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            
            // منع إغلاق التطبيق
            e.Handled = true;
        }

        /// <summary>
        /// الحصول على خدمة من حاوية الاعتماد
        /// </summary>
        public static T GetService<T>() where T : class
        {
            return ((App)Current)._serviceProvider!.GetRequiredService<T>();
        }
    }
}
