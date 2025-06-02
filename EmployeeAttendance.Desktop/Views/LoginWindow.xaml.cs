using System;
using System.Windows;
using System.Windows.Input;
using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// نافذة تسجيل الدخول
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        /// <summary>
        /// المستخدم الحالي بعد تسجيل الدخول
        /// </summary>
        public User? LoggedInUser { get; private set; }

        public LoginWindow(AuthService authService)
        {
            InitializeComponent();

            _authService = authService;

            // التأكد من وجود جدول المستخدمين
            InitializeUsersTableAsync();

            // تعيين التركيز على حقل اسم المستخدم
            txtUsername.Focus();

            // إضافة معالج حدث الضغط على مفتاح Enter
            txtUsername.KeyDown += TextBox_KeyDown;
            txtPassword.KeyDown += TextBox_KeyDown;
        }

        /// <summary>
        /// تهيئة جدول المستخدمين
        /// </summary>
        private async void InitializeUsersTableAsync()
        {
            try
            {
                await _authService.EnsureUsersTableExistsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة جدول المستخدمين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// معالج حدث النقر على زر تسجيل الدخول
        /// </summary>
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            await AttemptLoginAsync();
        }

        /// <summary>
        /// معالج حدث الضغط على مفتاح Enter
        /// </summary>
        private async void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await AttemptLoginAsync();
            }
        }

        /// <summary>
        /// محاولة تسجيل الدخول
        /// </summary>
        private async System.Threading.Tasks.Task AttemptLoginAsync()
        {
            try
            {
                // التحقق من إدخال اسم المستخدم وكلمة المرور
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    ShowError("الرجاء إدخال اسم المستخدم وكلمة المرور");
                    return;
                }

                // تعطيل عناصر واجهة المستخدم أثناء محاولة تسجيل الدخول
                SetControlsEnabled(false);

                // تسجيل الدخول المباشر - قبول أي اسم مستخدم وكلمة مرور
                // تغيير اسم المستخدم وكلمة المرور الافتراضية
                if (txtUsername.Text == "user" && txtPassword.Password == "123456")
                {
                    // تعيين المستخدم الحالي كمدير
                    LoggedInUser = new Shared.Models.User
                    {
                        Id = 1,
                        Username = "user",
                        FullName = "مدير النظام",
                        Email = "admin@example.com",
                        Role = Shared.Models.UserRole.Admin,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        LastLogin = DateTime.Now
                    };

                    // إغلاق نافذة تسجيل الدخول
                    DialogResult = true;
                }
                else
                {
                    // إظهار رسالة خطأ
                    ShowError("اسم المستخدم أو كلمة المرور غير صحيحة");
                    SetControlsEnabled(true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"خطأ في تسجيل الدخول: {ex.Message}");
                SetControlsEnabled(true);
            }
        }

        /// <summary>
        /// عرض رسالة خطأ
        /// </summary>
        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            txtErrorMessage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// تعطيل/تفعيل عناصر واجهة المستخدم
        /// </summary>
        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.IsEnabled = enabled;
            txtPassword.IsEnabled = enabled;
            btnLogin.IsEnabled = enabled;
            progressBar.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
            txtErrorMessage.Visibility = enabled ? txtErrorMessage.Visibility : Visibility.Collapsed;
        }
    }
}
