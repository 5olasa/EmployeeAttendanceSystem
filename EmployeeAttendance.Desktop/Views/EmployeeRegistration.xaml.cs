using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using EmployeeAttendance.Desktop.Services;
using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;
using Microsoft.Win32;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// نافذة تسجيل موظف جديد
    /// </summary>
    public partial class EmployeeRegistration : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly EnhancedFaceRecognitionService _faceRecognitionService;
        private readonly CameraService _cameraService;
        private Bitmap _capturedImage;
        private byte[] _faceEncodingData;

        public EmployeeRegistration(DatabaseService databaseService, EnhancedFaceRecognitionService faceRecognitionService)
        {
            InitializeComponent();

            _databaseService = databaseService;
            _faceRecognitionService = faceRecognitionService;
            _cameraService = new CameraService();

            // تعيين التاريخ الافتراضي
            dpHireDate.SelectedDate = DateTime.Today;

            // تحديد الشفت الافتراضي
            cmbShift.SelectedIndex = 0;

            // تعيين القيم الافتراضية
            txtVacationDays.Text = "21";

            // تسجيل أحداث الكاميرا
            _cameraService.NewFrameReceived += CameraService_NewFrameReceived;
            _cameraService.ErrorOccurred += CameraService_ErrorOccurred;
        }

        /// <summary>
        /// حدث التقاط إطار جديد من الكاميرا
        /// </summary>
        private void CameraService_NewFrameReceived(object sender, Bitmap frame)
        {
            try
            {
                // تحديث صورة الكاميرا
                Dispatcher.Invoke(() =>
                {
                    imgFace.Source = ConvertBitmapToBitmapImage(frame);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في معالجة الإطار الجديد: {ex.Message}");
            }
        }

        /// <summary>
        /// حدث حدوث خطأ في الكاميرا
        /// </summary>
        private void CameraService_ErrorOccurred(object sender, Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                txtFaceStatus.Text = $"خطأ في الكاميرا: {ex.Message}";
            });
        }

        /// <summary>
        /// حدث النقر على زر التقاط صورة
        /// </summary>
        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // الحصول على قائمة الكاميرات المتاحة
                var cameras = _cameraService.GetAvailableCameras();

                if (cameras == null || cameras.Count == 0)
                {
                    MessageBox.Show("لا توجد كاميرات متاحة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // إظهار شريط التقدم
                pbFaceDetection.Visibility = Visibility.Visible;
                pbFaceDetection.IsIndeterminate = true;

                // تعيين حالة التعرف على الوجه
                txtFaceStatus.Text = "جاري التقاط الصورة...";

                // تشغيل الكاميرا إذا لم تكن قيد التشغيل
                _cameraService.Start();

                // انتظار لحظة لبدء تشغيل الكاميرا
                await Task.Delay(1000);

                // التقاط صورة من الكاميرا
                _capturedImage = await _cameraService.CaptureImageAsync();

                if (_capturedImage == null)
                {
                    MessageBox.Show("فشل في التقاط صورة من الكاميرا", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // عرض الصورة
                imgFace.Source = ConvertBitmapToBitmapImage(_capturedImage);

                // إيقاف الكاميرا
                _cameraService.Stop();

                // اكتشاف الوجه
                await DetectFaceAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التقاط الصورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);

                // إخفاء شريط التقدم
                pbFaceDetection.Visibility = Visibility.Collapsed;

                // تعيين حالة التعرف على الوجه
                txtFaceStatus.Text = "فشل في التقاط الصورة";
            }
        }

        /// <summary>
        /// حدث النقر على زر اختيار صورة
        /// </summary>
        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // فتح مربع حوار اختيار ملف
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "ملفات الصور|*.jpg;*.jpeg;*.png;*.bmp",
                    Title = "اختيار صورة الوجه"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // إظهار شريط التقدم
                    pbFaceDetection.Visibility = Visibility.Visible;
                    pbFaceDetection.IsIndeterminate = true;

                    // تعيين حالة التعرف على الوجه
                    txtFaceStatus.Text = "جاري تحميل الصورة...";

                    // تحميل الصورة
                    _capturedImage = new Bitmap(openFileDialog.FileName);

                    // عرض الصورة
                    imgFace.Source = ConvertBitmapToBitmapImage(_capturedImage);

                    // اكتشاف الوجه
                    await DetectFaceAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الصورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);

                // إخفاء شريط التقدم
                pbFaceDetection.Visibility = Visibility.Collapsed;

                // تعيين حالة التعرف على الوجه
                txtFaceStatus.Text = "فشل في تحميل الصورة";
            }
        }

        /// <summary>
        /// اكتشاف الوجه في الصورة
        /// </summary>
        private async Task DetectFaceAsync()
        {
            try
            {
                // تعيين حالة التعرف على الوجه
                txtFaceStatus.Text = "جاري اكتشاف الوجه...";

                // اكتشاف الوجه
                var faces = await _faceRecognitionService.DetectFacesAsync(_capturedImage);

                if (faces.Count > 0)
                {
                    // تعيين حالة التعرف على الوجه
                    txtFaceStatus.Text = "تم اكتشاف الوجه بنجاح";

                    // استخراج بيانات الوجه
                    _faceEncodingData = await _faceRecognitionService.ExtractFaceEncodingAsync(_capturedImage);

                    // تحديث صورة الوجه بعد اقتصاص الوجه
                    using (MemoryStream ms = new MemoryStream(_faceEncodingData))
                    {
                        Bitmap faceImage = new Bitmap(ms);
                        imgFace.Source = ConvertBitmapToBitmapImage(faceImage);
                    }
                }
                else
                {
                    // تعيين حالة التعرف على الوجه
                    txtFaceStatus.Text = "لم يتم اكتشاف وجه في الصورة";
                    _faceEncodingData = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في اكتشاف الوجه: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);

                // تعيين حالة التعرف على الوجه
                txtFaceStatus.Text = "فشل في اكتشاف الوجه";
                _faceEncodingData = null;
            }
            finally
            {
                // إخفاء شريط التقدم
                pbFaceDetection.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// حدث النقر على زر حفظ
        /// </summary>
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // التحقق من صحة البيانات
                if (string.IsNullOrWhiteSpace(txtEmployeeNumber.Text))
                {
                    MessageBox.Show("الرجاء إدخال الرقم الوظيفي", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmployeeNumber.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("الرجاء إدخال اسم الموظف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtName.Focus();
                    return;
                }

                if (_capturedImage == null)
                {
                    MessageBox.Show("الرجاء التقاط صورة للموظف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_faceEncodingData == null)
                {
                    MessageBox.Show("لم يتم اكتشاف وجه في الصورة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // إنشاء كائن الموظف
                var employee = new Employee
                {
                    EmployeeNumber = txtEmployeeNumber.Text,
                    Name = txtName.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    HireDate = dpHireDate.SelectedDate ?? DateTime.Today,
                    ShiftId = cmbShift.SelectedIndex + 1,
                    MonthlySalary = decimal.Parse(txtSalary.Text),
                    AvailableVacationDays = int.Parse(txtVacationDays.Text),
                    FaceEncodingData = _faceEncodingData,
                    FaceImageBase64 = _faceRecognitionService.ConvertImageToBase64(_capturedImage),
                    LastUpdated = DateTime.Now
                };

                // حفظ الموظف في قاعدة البيانات
                int employeeId = await _databaseService.AddEmployeeAsync(employee);

                // حفظ صورة الوجه
                string imagePath = await _faceRecognitionService.SaveFaceImageAsync(employeeId, _capturedImage);

                // تحديث مسار الصورة
                employee.Id = employeeId;
                employee.FaceImagePath = imagePath;
                await _databaseService.UpdateEmployeeAsync(employee);

                MessageBox.Show("تم تسجيل الموظف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);

                // إغلاق النافذة
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ بيانات الموظف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر إلغاء
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// تحويل Bitmap إلى BitmapImage
        /// </summary>
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
