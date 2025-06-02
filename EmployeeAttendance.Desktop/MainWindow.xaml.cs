using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using EmployeeAttendance.Desktop.Services;
using EmployeeAttendance.Desktop.Views;
using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;

namespace EmployeeAttendance.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly EnhancedFaceRecognitionService _faceRecognitionService;
        private readonly SyncService _syncService;
        private readonly AuthService _authService;

        // المستخدم الحالي
        private User? _currentUser;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // إنشاء المجلدات اللازمة
                Directory.CreateDirectory("Data");
                Directory.CreateDirectory("Data/Models");
                Directory.CreateDirectory("Data/Reports");
                Directory.CreateDirectory("Data/Config");
                Directory.CreateDirectory("Data/Images");
                Directory.CreateDirectory("Data/Images/Employees");
                Directory.CreateDirectory("Data/Images/Attendance");

                // إنشاء الخدمات
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeAttendance.mdf;Integrated Security=True";
                _databaseService = new DatabaseService(connectionString);
                _faceRecognitionService = new EnhancedFaceRecognitionService("Data/Models");
                _authService = new AuthService(connectionString, _databaseService);

                // إنشاء خدمة المزامنة (بدون محاولة الاتصال بـ Firebase تلقائيًا)
                string firebaseApiKey = "AIzaSyDummyApiKey123456789";
                string firebaseDatabaseUrl = "https://employee-attendance-12345-default-rtdb.firebaseio.com/";
                string serviceAccountPath = "Data/Config/firebase-service-account.json";

                _syncService = new SyncService(firebaseApiKey, firebaseDatabaseUrl, _databaseService, serviceAccountPath);

                // عرض نافذة تسجيل الدخول
                ShowLoginWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة التطبيق: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// عرض نافذة تسجيل الدخول
        /// </summary>
        private void ShowLoginWindow()
        {
            try
            {
                // إنشاء نافذة تسجيل الدخول
                var loginWindow = new LoginWindow(_authService);

                // عرض نافذة تسجيل الدخول كنافذة حوار
                bool? result = loginWindow.ShowDialog();

                // التحقق من نجاح تسجيل الدخول
                if (result == true && loginWindow.LoggedInUser != null)
                {
                    // تعيين المستخدم الحالي
                    _currentUser = loginWindow.LoggedInUser;

                    // تحديث معلومات المستخدم في الواجهة
                    UpdateUserInfo();

                    // تحديث الإحصائيات
                    LoadStatistics();
                }
                else
                {
                    // إغلاق التطبيق إذا لم يتم تسجيل الدخول
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في عرض نافذة تسجيل الدخول: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// تحديث معلومات المستخدم في الواجهة
        /// </summary>
        private void UpdateUserInfo()
        {
            if (_currentUser != null)
            {
                txtUserInfo.Text = $"مرحبًا، {_currentUser.FullName}";
            }
        }

        /// <summary>
        /// تحميل الإحصائيات
        /// </summary>
        private async void LoadStatistics()
        {
            try
            {
                // تحميل إحصائيات الموظفين
                await LoadEmployeeStatistics();

                // تحميل إحصائيات الحضور
                await LoadAttendanceStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الإحصائيات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// تحميل إحصائيات الموظفين
        /// </summary>
        private async Task LoadEmployeeStatistics()
        {
            try
            {
                // الحصول على قائمة الموظفين
                var employees = await _databaseService.GetAllEmployeesAsync();

                // عرض عدد الموظفين
                txtTotalEmployees.Text = employees.Count.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحميل إحصائيات الموظفين: {ex.Message}");
            }
        }

        /// <summary>
        /// تحميل إحصائيات الحضور
        /// </summary>
        private async Task LoadAttendanceStatistics()
        {
            try
            {
                // الحصول على سجلات الحضور لليوم الحالي
                var attendanceRecords = await _databaseService.GetAttendanceRecordsAsync(0, DateTime.Today, DateTime.Today);

                // عدد الحاضرين اليوم
                int presentCount = attendanceRecords.Count;
                txtPresentToday.Text = presentCount.ToString();

                // عدد المتأخرين اليوم
                int lateCount = 0;
                foreach (var record in attendanceRecords)
                {
                    if (record.IsLate)
                    {
                        lateCount++;
                    }
                }
                txtLateToday.Text = lateCount.ToString();

                // عدد الغائبين اليوم (عدد الموظفين - عدد الحاضرين)
                int totalEmployees = int.Parse(txtTotalEmployees.Text);
                int absentCount = totalEmployees - presentCount;
                txtAbsentToday.Text = absentCount > 0 ? absentCount.ToString() : "0";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحميل إحصائيات الحضور: {ex.Message}");
            }
        }

        private void btnAttendanceMonitor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AttendanceMonitor(_databaseService, _faceRecognitionService, _syncService);
                window.Owner = this;
                window.ShowDialog();

                // تحديث الإحصائيات بعد تسجيل الحضور
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة مراقبة الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مشرف أو مدير)
            if (!CheckUserPermission(UserRole.Supervisor))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new EmployeeManagement(_currentUser);
                window.Owner = this;
                window.ShowDialog();

                // تحديث الإحصائيات بعد تعديل بيانات الموظفين
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة إدارة الموظفين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر إدارة المرتبات
        /// </summary>
        private async void btnSalaryManagement_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مشرف أو مدير)
            if (!CheckUserPermission(UserRole.Supervisor))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // الحصول على قائمة الموظفين
                var employees = await _databaseService.GetAllEmployeesAsync();

                if (employees.Count == 0)
                {
                    MessageBox.Show("لا يوجد موظفين مسجلين في النظام", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // اختيار موظف
                var selectEmployeeWindow = new SelectEmployeeWindow(employees);
                if (selectEmployeeWindow.ShowDialog() == true && selectEmployeeWindow.SelectedEmployee != null)
                {
                    // فتح نافذة إدارة المرتبات للموظف المحدد
                    var window = new SalaryManagement(_currentUser, selectEmployeeWindow.SelectedEmployee);
                    window.Owner = this;
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة إدارة المرتبات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReportGenerator_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مشرف أو مدير)
            if (!CheckUserPermission(UserRole.Supervisor))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new ReportsGenerator(_currentUser);
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة توليد التقارير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مدير فقط)
            if (!CheckUserPermission(UserRole.Admin))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new EmployeeRegistration(_databaseService, _faceRecognitionService);
                window.Owner = this;
                window.ShowDialog();

                // تحديث الإحصائيات بعد إضافة موظف جديد
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة إضافة موظف جديد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnManualAttendance_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مشرف أو مدير)
            if (!CheckUserPermission(UserRole.Supervisor))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new ManualAttendance(_currentUser);
                window.Owner = this;
                window.ShowDialog();

                // تحديث الإحصائيات بعد تسجيل الحضور
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة تسجيل الحضور اليدوي: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مدير فقط)
            if (!CheckUserPermission(UserRole.Admin))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var window = new SystemSettings(_currentUser);
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSyncSettings_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صلاحيات المستخدم (مدير فقط)
            if (!CheckUserPermission(UserRole.Admin))
            {
                MessageBox.Show("ليس لديك صلاحية للوصول إلى هذه الميزة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // فتح نافذة إعدادات المزامنة
                SyncSettings syncSettingsWindow = new SyncSettings(_syncService);
                syncSettingsWindow.Owner = this;
                syncSettingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة إعدادات المزامنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// معالج حدث النقر على زر تسجيل الخروج
        /// </summary>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تأكيد تسجيل الخروج
                MessageBoxResult result = MessageBox.Show("هل أنت متأكد من رغبتك في تسجيل الخروج؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // تسجيل الخروج
                    _authService.Logout();

                    // إعادة تشغيل التطبيق
                    System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تسجيل الخروج: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// التحقق من صلاحيات المستخدم
        /// </summary>
        private bool CheckUserPermission(UserRole requiredRole)
        {
            // التحقق من وجود مستخدم حالي
            if (_currentUser == null)
            {
                return false;
            }

            // التحقق من صلاحيات المستخدم
            return _currentUser.Role >= requiredRole;
        }
    }
}