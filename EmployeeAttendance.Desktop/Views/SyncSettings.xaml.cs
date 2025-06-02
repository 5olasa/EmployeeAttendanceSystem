using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using EmployeeAttendance.Shared.Services;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// نافذة إعدادات المزامنة
    /// </summary>
    public partial class SyncSettings : Window
    {
        private readonly SyncService _syncService;
        private readonly string _configFilePath = "Data/Config/sync_settings.json";

        // إعدادات المزامنة
        private class SyncSettingsData
        {
            public bool EnableSync { get; set; } = true;
            public string ApiKey { get; set; } = "";
            public string DatabaseUrl { get; set; } = "";
            public string ServiceAccountPath { get; set; } = "";
            public int SyncInterval { get; set; } = 15;
            public int SyncDays { get; set; } = 30;
            public DateTime? LastEmployeeSync { get; set; } = null;
            public DateTime? LastAttendanceSync { get; set; } = null;
        }

        private SyncSettingsData _settings;

        public SyncSettings(SyncService syncService)
        {
            InitializeComponent();

            _syncService = syncService;
            _settings = new SyncSettingsData();

            // التأكد من وجود مجلد الإعدادات
            Directory.CreateDirectory("Data/Config");
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        /// <summary>
        /// تحميل الإعدادات
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                // التحقق من وجود ملف الإعدادات
                if (File.Exists(_configFilePath))
                {
                    // قراءة الإعدادات من الملف
                    string json = File.ReadAllText(_configFilePath);
                    _settings = JsonConvert.DeserializeObject<SyncSettingsData>(json) ?? new SyncSettingsData();
                }

                // تعيين قيم الإعدادات في واجهة المستخدم
                chkEnableSync.IsChecked = _settings.EnableSync;
                txtApiKey.Text = _settings.ApiKey;
                txtDatabaseUrl.Text = _settings.DatabaseUrl;
                txtServiceAccountPath.Text = _settings.ServiceAccountPath;
                txtSyncInterval.Text = _settings.SyncInterval.ToString();
                txtSyncDays.Text = _settings.SyncDays.ToString();

                // تعيين قيم آخر مزامنة
                txtLastEmployeeSync.Text = _settings.LastEmployeeSync.HasValue
                    ? _settings.LastEmployeeSync.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : "لم تتم المزامنة بعد";

                txtLastAttendanceSync.Text = _settings.LastAttendanceSync.HasValue
                    ? _settings.LastAttendanceSync.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : "لم تتم المزامنة بعد";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حفظ الإعدادات
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                // تحديث قيم الإعدادات من واجهة المستخدم
                _settings.EnableSync = chkEnableSync.IsChecked ?? false;
                _settings.ApiKey = txtApiKey.Text;
                _settings.DatabaseUrl = txtDatabaseUrl.Text;
                _settings.ServiceAccountPath = txtServiceAccountPath.Text;

                if (int.TryParse(txtSyncInterval.Text, out int syncInterval))
                {
                    _settings.SyncInterval = syncInterval;
                }

                if (int.TryParse(txtSyncDays.Text, out int syncDays))
                {
                    _settings.SyncDays = syncDays;
                }

                // حفظ الإعدادات في ملف
                string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_configFilePath, json);

                MessageBox.Show("تم حفظ الإعدادات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر استعراض ملف حساب الخدمة
        /// </summary>
        private void btnBrowseServiceAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // فتح مربع حوار اختيار ملف
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "ملفات JSON|*.json",
                    Title = "اختيار ملف حساب الخدمة"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // نسخ الملف إلى مجلد الإعدادات
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string destPath = Path.Combine("Data/Config", fileName);

                    File.Copy(openFileDialog.FileName, destPath, true);

                    // تعيين مسار الملف
                    txtServiceAccountPath.Text = destPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في اختيار ملف حساب الخدمة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر اختبار الاتصال
        /// </summary>
        private async void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحديث إعدادات Firebase من واجهة المستخدم
                _syncService.UpdateSettings(txtApiKey.Text, txtDatabaseUrl.Text, txtServiceAccountPath.Text);

                // تعطيل الأزرار
                btnTestConnection.IsEnabled = false;
                pbSync.Visibility = Visibility.Visible;
                txtConnectionStatus.Text = "جاري اختبار الاتصال...";

                // اختبار الاتصال
                bool isConnected = await _syncService.CheckFirebaseConnectionAsync();

                // عرض نتيجة الاختبار
                if (isConnected)
                {
                    txtConnectionStatus.Text = "تم الاتصال بنجاح";
                    MessageBox.Show("تم الاتصال بـ Firebase بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    txtConnectionStatus.Text = "فشل الاتصال";
                    MessageBox.Show("فشل الاتصال بـ Firebase. تأكد من صحة بيانات الاتصال.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في اختبار الاتصال: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtConnectionStatus.Text = "فشل الاتصال";
            }
            finally
            {
                // إعادة تفعيل الأزرار
                btnTestConnection.IsEnabled = true;
                pbSync.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// حدث النقر على زر مزامنة بيانات الموظفين
        /// </summary>
        private async void btnSyncEmployees_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحديث إعدادات Firebase من واجهة المستخدم
                _syncService.UpdateSettings(txtApiKey.Text, txtDatabaseUrl.Text, txtServiceAccountPath.Text);

                // تعطيل الأزرار
                btnSyncEmployees.IsEnabled = false;
                pbSync.Visibility = Visibility.Visible;

                // التحقق من الاتصال أولاً
                bool isConnected = await _syncService.CheckFirebaseConnectionAsync();
                if (!isConnected)
                {
                    MessageBox.Show("لا يمكن الاتصال بـ Firebase. تأكد من صحة بيانات الاتصال.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // مزامنة بيانات الموظفين
                bool success = await _syncService.SyncEmployeesAsync();

                // عرض نتيجة المزامنة
                if (success)
                {
                    _settings.LastEmployeeSync = DateTime.Now;
                    txtLastEmployeeSync.Text = _settings.LastEmployeeSync.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    MessageBox.Show("تمت مزامنة بيانات الموظفين بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("فشل في مزامنة بيانات الموظفين", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في مزامنة بيانات الموظفين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // إعادة تفعيل الأزرار
                btnSyncEmployees.IsEnabled = true;
                pbSync.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// حدث النقر على زر مزامنة سجلات الحضور
        /// </summary>
        private async void btnSyncAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحديث إعدادات Firebase من واجهة المستخدم
                _syncService.UpdateSettings(txtApiKey.Text, txtDatabaseUrl.Text, txtServiceAccountPath.Text);

                // تعطيل الأزرار
                btnSyncAttendance.IsEnabled = false;
                pbSync.Visibility = Visibility.Visible;

                // التحقق من الاتصال أولاً
                bool isConnected = await _syncService.CheckFirebaseConnectionAsync();
                if (!isConnected)
                {
                    MessageBox.Show("لا يمكن الاتصال بـ Firebase. تأكد من صحة بيانات الاتصال.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // تحديد فترة المزامنة
                int syncDays = 30;
                if (int.TryParse(txtSyncDays.Text, out int days))
                {
                    syncDays = days;
                }

                DateTime startDate = DateTime.Today.AddDays(-syncDays);
                DateTime endDate = DateTime.Today;

                // مزامنة سجلات الحضور
                bool success = await _syncService.SyncAttendanceRecordsAsync(startDate, endDate);

                // عرض نتيجة المزامنة
                if (success)
                {
                    _settings.LastAttendanceSync = DateTime.Now;
                    txtLastAttendanceSync.Text = _settings.LastAttendanceSync.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    MessageBox.Show("تمت مزامنة سجلات الحضور بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("فشل في مزامنة سجلات الحضور", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في مزامنة سجلات الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // إعادة تفعيل الأزرار
                btnSyncAttendance.IsEnabled = true;
                pbSync.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// حدث النقر على زر حفظ الإعدادات
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
        }

        /// <summary>
        /// حدث النقر على زر إلغاء
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
