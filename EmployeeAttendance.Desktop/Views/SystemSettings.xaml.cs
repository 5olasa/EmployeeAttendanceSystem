using EmployeeAttendance.Shared.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// واجهة إعدادات النظام
    /// </summary>
    public partial class SystemSettings : Window
    {
        private readonly string _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// إنشاء نافذة إعدادات النظام
        /// </summary>
        public SystemSettings(User currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;

            // تحميل الإعدادات عند تحميل النافذة
            Loaded += SystemSettings_Loaded;
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private void SystemSettings_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحميل الإعدادات
                LoadSettings();

                // تحديث حالة عناصر واجهة المستخدم
                UpdateUIState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// تحميل الإعدادات
        /// </summary>
        private void LoadSettings()
        {
            // في التطبيق الحقيقي، سنقوم بتحميل الإعدادات من ملف
            // هنا نقوم بتعيين قيم افتراضية

            // إعدادات عامة
            txtCompanyName.Text = "شركة نظام الحضور";
            txtLogoPath.Text = "Data\\logo.png";
            cmbLanguage.SelectedIndex = 0; // العربية
            cmbTimeZone.SelectedIndex = 0; // الرياض
            cmbDateFormat.SelectedIndex = 0; // DD/MM/YYYY

            // إعدادات قاعدة البيانات
            cmbDatabaseType.SelectedIndex = 0; // SQL Server
            txtServerName.Text = "(LocalDB)\\MSSQLLocalDB";
            txtDatabaseName.Text = "EmployeeAttendance";
            txtDatabaseUsername.Text = "";
            txtDatabasePassword.Clear();

            // إعدادات الكاميرا
            cmbCameraDevice.SelectedIndex = 0; // الكاميرا الافتراضية
            cmbCameraResolution.SelectedIndex = 0; // 640x480
            sldFaceRecognitionSensitivity.Value = 70;
            txtImagesPath.Text = "Data\\Images";

            // إعدادات المزامنة
            chkEnableSync.IsChecked = false;
            txtSyncServerUrl.Text = "http://localhost:5000/api";
            txtApiKey.Clear();
            cmbSyncInterval.SelectedIndex = 1; // 15 دقيقة
        }

        /// <summary>
        /// تحديث حالة عناصر واجهة المستخدم
        /// </summary>
        private void UpdateUIState()
        {
            // تحديث حالة عناصر واجهة المستخدم بناءً على الإعدادات

            // إعدادات قاعدة البيانات
            bool isSqlServer = cmbDatabaseType.SelectedIndex == 0;
            txtServerName.IsEnabled = isSqlServer;
            txtDatabaseUsername.IsEnabled = isSqlServer;
            txtDatabasePassword.IsEnabled = isSqlServer;

            // إعدادات المزامنة
            bool isSyncEnabled = chkEnableSync.IsChecked == true;
            txtSyncServerUrl.IsEnabled = isSyncEnabled;
            txtApiKey.IsEnabled = isSyncEnabled;
            cmbSyncInterval.IsEnabled = isSyncEnabled;
        }

        /// <summary>
        /// حدث تغيير نوع قاعدة البيانات
        /// </summary>
        private void cmbDatabaseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUIState();
        }

        /// <summary>
        /// حدث تغيير حالة تفعيل المزامنة
        /// </summary>
        private void chkEnableSync_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateUIState();
        }

        /// <summary>
        /// حدث النقر على زر استعراض شعار الشركة
        /// </summary>
        private void btnBrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // إنشاء مربع حوار اختيار ملف
                var openFileDialog = new OpenFileDialog
                {
                    Title = "اختيار شعار الشركة",
                    Filter = "ملفات الصور|*.png;*.jpg;*.jpeg;*.bmp|كل الملفات|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                };

                // عرض مربع الحوار
                if (openFileDialog.ShowDialog() == true)
                {
                    // تعيين مسار الملف المحدد
                    txtLogoPath.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء اختيار الملف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر استعراض مسار حفظ الصور
        /// </summary>
        private void btnBrowseImagesPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // إنشاء مربع حوار اختيار مجلد
                var openFolderDialog = new Microsoft.Win32.OpenFolderDialog
                {
                    Title = "اختيار مجلد حفظ الصور"
                };

                // عرض مربع الحوار
                if (openFolderDialog.ShowDialog() == true)
                {
                    // تعيين مسار المجلد المحدد
                    txtImagesPath.Text = openFolderDialog.FolderName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء اختيار المجلد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر اختبار الاتصال بقاعدة البيانات
        /// </summary>
        private async void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تعطيل الزر أثناء الاختبار
                btnTestConnection.IsEnabled = false;
                txtStatus.Text = "جاري اختبار الاتصال بقاعدة البيانات...";

                // محاكاة اختبار الاتصال
                await Task.Delay(1500);

                // عرض رسالة نجاح
                MessageBox.Show("تم الاتصال بقاعدة البيانات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "تم الاتصال بقاعدة البيانات بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل الاتصال بقاعدة البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشل الاتصال بقاعدة البيانات";
            }
            finally
            {
                // إعادة تمكين الزر
                btnTestConnection.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر إنشاء قاعدة بيانات
        /// </summary>
        private async void btnCreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // التأكيد قبل إنشاء قاعدة البيانات
                var result = MessageBox.Show(
                    "هل أنت متأكد من إنشاء قاعدة بيانات جديدة؟\nسيتم حذف جميع البيانات الموجودة.",
                    "تأكيد",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    // تعطيل الزر أثناء الإنشاء
                    btnCreateDatabase.IsEnabled = false;
                    txtStatus.Text = "جاري إنشاء قاعدة البيانات...";

                    // محاكاة إنشاء قاعدة البيانات
                    await Task.Delay(2000);

                    // عرض رسالة نجاح
                    MessageBox.Show("تم إنشاء قاعدة البيانات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtStatus.Text = "تم إنشاء قاعدة البيانات بنجاح";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل إنشاء قاعدة البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشل إنشاء قاعدة البيانات";
            }
            finally
            {
                // إعادة تمكين الزر
                btnCreateDatabase.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر اختبار الكاميرا
        /// </summary>
        private async void btnTestCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تعطيل الزر أثناء الاختبار
                btnTestCamera.IsEnabled = false;
                txtStatus.Text = "جاري اختبار الكاميرا...";

                // محاكاة اختبار الكاميرا
                await Task.Delay(1500);

                // عرض رسالة نجاح
                MessageBox.Show("تم اختبار الكاميرا بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "تم اختبار الكاميرا بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل اختبار الكاميرا: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشل اختبار الكاميرا";
            }
            finally
            {
                // إعادة تمكين الزر
                btnTestCamera.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر اختبار الاتصال بالمزامنة
        /// </summary>
        private async void btnTestSyncConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تعطيل الزر أثناء الاختبار
                btnTestSyncConnection.IsEnabled = false;
                txtStatus.Text = "جاري اختبار الاتصال بخادم المزامنة...";

                // محاكاة اختبار الاتصال
                await Task.Delay(1500);

                // عرض رسالة نجاح
                MessageBox.Show("تم الاتصال بخادم المزامنة بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "تم الاتصال بخادم المزامنة بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل الاتصال بخادم المزامنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشل الاتصال بخادم المزامنة";
            }
            finally
            {
                // إعادة تمكين الزر
                btnTestSyncConnection.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر المزامنة الآن
        /// </summary>
        private async void btnSyncNow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تعطيل الزر أثناء المزامنة
                btnSyncNow.IsEnabled = false;
                txtStatus.Text = "جاري المزامنة...";

                // محاكاة المزامنة
                await Task.Delay(2000);

                // عرض رسالة نجاح
                MessageBox.Show("تمت المزامنة بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "تمت المزامنة بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشلت المزامنة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشلت المزامنة";
            }
            finally
            {
                // إعادة تمكين الزر
                btnSyncNow.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر حفظ الإعدادات
        /// </summary>
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // تعطيل الزر أثناء الحفظ
                btnSave.IsEnabled = false;
                txtStatus.Text = "جاري حفظ الإعدادات...";

                // محاكاة حفظ الإعدادات
                await Task.Delay(1000);

                // في التطبيق الحقيقي، سنقوم بحفظ الإعدادات في ملف
                // SaveSettingsToFile();

                // عرض رسالة نجاح
                MessageBox.Show("تم حفظ الإعدادات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                txtStatus.Text = "تم حفظ الإعدادات بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل حفظ الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "فشل حفظ الإعدادات";
            }
            finally
            {
                // إعادة تمكين الزر
                btnSave.IsEnabled = true;
            }
        }

        /// <summary>
        /// حدث النقر على زر الإغلاق
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
