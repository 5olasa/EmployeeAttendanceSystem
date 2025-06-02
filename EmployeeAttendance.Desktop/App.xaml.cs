using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace EmployeeAttendance.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            // معالجة الاستثناءات غير المعالجة
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // تعيين مسار قاعدة البيانات
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            // إنشاء مجلد قاعدة البيانات إذا لم يكن موجودًا
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"حدث خطأ غير متوقع: {e.Exception.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"حدث خطأ غير متوقع: {(e.ExceptionObject as Exception)?.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

