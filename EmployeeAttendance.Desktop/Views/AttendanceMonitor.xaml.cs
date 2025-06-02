using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using EmployeeAttendance.Desktop.Services;
using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// نافذة مراقبة الحضور
    /// </summary>
    public partial class AttendanceMonitor : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly EnhancedFaceRecognitionService _faceRecognitionService;
        private readonly SyncService _syncService;
        private readonly CameraService _cameraService;

        private Bitmap _currentFrame;
        private Employee _recognizedEmployee;
        private Attendance _todayAttendance;
        private List<Attendance> _todayAttendances;

        public AttendanceMonitor(DatabaseService databaseService, EnhancedFaceRecognitionService faceRecognitionService, SyncService syncService)
        {
            InitializeComponent();

            _databaseService = databaseService;
            _faceRecognitionService = faceRecognitionService;
            _syncService = syncService;
            _cameraService = new CameraService();

            // تعيين التاريخ الحالي
            txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            // تعطيل أزرار الحضور والانصراف
            btnCheckIn.IsEnabled = false;
            btnCheckOut.IsEnabled = false;

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
                // تحديث الإطار الحالي
                _currentFrame = (Bitmap)frame.Clone();

                // تحديث صورة الكاميرا
                Dispatcher.Invoke(() =>
                {
                    imgCamera.Source = ConvertBitmapToBitmapImage(_currentFrame);
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
                txtRecognitionStatus.Text = $"خطأ في الكاميرا: {ex.Message}";
            });
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحميل سجلات الحضور لليوم الحالي
                await LoadTodayAttendancesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل سجلات الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// تحميل سجلات الحضور لليوم الحالي
        /// </summary>
        private async Task LoadTodayAttendancesAsync()
        {
            // في التطبيق الحقيقي، سنقوم بتحميل سجلات الحضور من قاعدة البيانات
            // هنا نقوم بإنشاء بيانات وهمية

            _todayAttendances = await _databaseService.GetAttendanceRecordsAsync(0, DateTime.Today, DateTime.Today);

            // عرض سجلات الحضور في الجدول
            dgAttendance.ItemsSource = _todayAttendances;
        }

        /// <summary>
        /// حدث النقر على زر تشغيل الكاميرا
        /// </summary>
        private void btnStartCamera_Click(object sender, RoutedEventArgs e)
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

                if (btnStartCamera.Content.ToString() == "تشغيل الكاميرا")
                {
                    // تشغيل الكاميرا
                    _cameraService.Start();
                    btnStartCamera.Content = "إيقاف الكاميرا";
                    txtRecognitionStatus.Text = "جاري البحث عن وجه...";
                }
                else
                {
                    // إيقاف الكاميرا
                    _cameraService.Stop();
                    btnStartCamera.Content = "تشغيل الكاميرا";
                    txtRecognitionStatus.Text = "تم إيقاف الكاميرا";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تشغيل الكاميرا: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر التقاط صورة
        /// </summary>
        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentFrame == null)
                {
                    MessageBox.Show("الرجاء تشغيل الكاميرا أولاً", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // إظهار شريط التقدم
                pbRecognition.Visibility = Visibility.Visible;
                pbRecognition.IsIndeterminate = true;

                // تعيين حالة التعرف
                txtRecognitionStatus.Text = "جاري التعرف على الوجه...";

                // التقاط صورة من الكاميرا
                Bitmap capturedImage = _cameraService.CaptureImage();

                if (capturedImage == null)
                {
                    MessageBox.Show("فشل في التقاط صورة من الكاميرا", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // التعرف على الوجه
                _recognizedEmployee = await _faceRecognitionService.RecognizeFaceAsync(capturedImage);

                if (_recognizedEmployee != null)
                {
                    // تعيين حالة التعرف
                    txtRecognitionStatus.Text = $"تم التعرف على الموظف: {_recognizedEmployee.Name}";

                    // عرض معلومات الموظف
                    DisplayEmployeeInfo(_recognizedEmployee);

                    // البحث عن سجل حضور اليوم للموظف
                    _todayAttendance = _todayAttendances.FirstOrDefault(a => a.EmployeeId == _recognizedEmployee.Id);

                    // تحديث حالة اليوم
                    UpdateDayStatus();

                    // تفعيل أزرار الحضور والانصراف
                    UpdateAttendanceButtons();
                }
                else
                {
                    // تعيين حالة التعرف
                    txtRecognitionStatus.Text = "لم يتم التعرف على الوجه";

                    // مسح معلومات الموظف
                    ClearEmployeeInfo();

                    // تعطيل أزرار الحضور والانصراف
                    btnCheckIn.IsEnabled = false;
                    btnCheckOut.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التعرف على الوجه: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);

                // تعيين حالة التعرف
                txtRecognitionStatus.Text = "فشل في التعرف على الوجه";

                // مسح معلومات الموظف
                ClearEmployeeInfo();

                // تعطيل أزرار الحضور والانصراف
                btnCheckIn.IsEnabled = false;
                btnCheckOut.IsEnabled = false;
            }
            finally
            {
                // إخفاء شريط التقدم
                pbRecognition.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// عرض معلومات الموظف
        /// </summary>
        private void DisplayEmployeeInfo(Employee employee)
        {
            txtEmployeeNumber.Text = employee.EmployeeNumber;
            txtEmployeeName.Text = employee.Name;

            // تحديد نوع الشفت
            switch (employee.ShiftId)
            {
                case 1:
                    txtShift.Text = "صباحي (8:00 - 16:00)";
                    break;
                case 2:
                    txtShift.Text = "مسائي (16:00 - 00:00)";
                    break;
                case 3:
                    txtShift.Text = "ليلي (00:00 - 8:00)";
                    break;
                default:
                    txtShift.Text = "غير محدد";
                    break;
            }
        }

        /// <summary>
        /// مسح معلومات الموظف
        /// </summary>
        private void ClearEmployeeInfo()
        {
            txtEmployeeNumber.Text = "-";
            txtEmployeeName.Text = "-";
            txtShift.Text = "-";
            txtDayStatus.Text = "-";
            txtCheckInTime.Text = "-";
            txtCheckOutTime.Text = "-";
            txtWorkHours.Text = "-";
        }

        /// <summary>
        /// تحديث حالة اليوم
        /// </summary>
        private void UpdateDayStatus()
        {
            if (_todayAttendance == null)
            {
                txtDayStatus.Text = "لم يتم تسجيل الحضور";
                txtCheckInTime.Text = "-";
                txtCheckOutTime.Text = "-";
                txtWorkHours.Text = "-";
            }
            else
            {
                if (_todayAttendance.CheckInTime.HasValue && _todayAttendance.CheckOutTime.HasValue)
                {
                    txtDayStatus.Text = "تم تسجيل الحضور والانصراف";
                    txtCheckInTime.Text = _todayAttendance.CheckInTime.Value.ToString("HH:mm:ss");
                    txtCheckOutTime.Text = _todayAttendance.CheckOutTime.Value.ToString("HH:mm:ss");
                    txtWorkHours.Text = _todayAttendance.WorkHours.ToString("N2");
                }
                else if (_todayAttendance.CheckInTime.HasValue)
                {
                    txtDayStatus.Text = "تم تسجيل الحضور فقط";
                    txtCheckInTime.Text = _todayAttendance.CheckInTime.Value.ToString("HH:mm:ss");
                    txtCheckOutTime.Text = "-";
                    txtWorkHours.Text = "-";
                }
                else
                {
                    txtDayStatus.Text = "حالة غير معروفة";
                    txtCheckInTime.Text = "-";
                    txtCheckOutTime.Text = "-";
                    txtWorkHours.Text = "-";
                }
            }
        }

        /// <summary>
        /// تحديث أزرار الحضور والانصراف
        /// </summary>
        private void UpdateAttendanceButtons()
        {
            if (_todayAttendance == null)
            {
                // لم يتم تسجيل الحضور بعد
                btnCheckIn.IsEnabled = true;
                btnCheckOut.IsEnabled = false;
            }
            else if (_todayAttendance.CheckInTime.HasValue && !_todayAttendance.CheckOutTime.HasValue)
            {
                // تم تسجيل الحضور ولم يتم تسجيل الانصراف
                btnCheckIn.IsEnabled = false;
                btnCheckOut.IsEnabled = true;
            }
            else if (_todayAttendance.CheckInTime.HasValue && _todayAttendance.CheckOutTime.HasValue)
            {
                // تم تسجيل الحضور والانصراف
                btnCheckIn.IsEnabled = false;
                btnCheckOut.IsEnabled = false;
            }
            else
            {
                // حالة غير معروفة
                btnCheckIn.IsEnabled = false;
                btnCheckOut.IsEnabled = false;
            }
        }

        /// <summary>
        /// حدث النقر على زر تسجيل حضور
        /// </summary>
        private async void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_recognizedEmployee == null)
                {
                    MessageBox.Show("الرجاء التعرف على الموظف أولاً", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // إنشاء سجل حضور جديد
                var attendance = new Attendance
                {
                    EmployeeId = _recognizedEmployee.Id,
                    Date = DateTime.Today,
                    CheckInTime = DateTime.Now,
                    IsManualCheckIn = false,
                    Device = "جهاز الحضور الرئيسي",
                    LastUpdated = DateTime.Now
                };

                // حفظ صورة الحضور
                string imagePath = await _faceRecognitionService.SaveFaceImageAsync(_recognizedEmployee.Id, _currentFrame);
                attendance.CheckInImagePath = imagePath;

                // تحديد ما إذا كان الموظف متأخرًا
                TimeSpan shiftStartTime;
                switch (_recognizedEmployee.ShiftId)
                {
                    case 1: // صباحي
                        shiftStartTime = new TimeSpan(8, 0, 0);
                        break;
                    case 2: // مسائي
                        shiftStartTime = new TimeSpan(16, 0, 0);
                        break;
                    case 3: // ليلي
                        shiftStartTime = new TimeSpan(0, 0, 0);
                        break;
                    default:
                        shiftStartTime = new TimeSpan(8, 0, 0);
                        break;
                }

                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                if (currentTime > shiftStartTime)
                {
                    attendance.IsLate = true;
                    attendance.LateMinutes = (int)(currentTime - shiftStartTime).TotalMinutes;
                }

                // تسجيل الحضور في قاعدة البيانات
                int attendanceId = await _databaseService.RecordCheckInAsync(attendance);
                attendance.Id = attendanceId;

                // إضافة السجل إلى قائمة سجلات اليوم
                attendance.Employee = _recognizedEmployee;
                _todayAttendances.Add(attendance);
                _todayAttendance = attendance;

                // تحديث الجدول
                dgAttendance.ItemsSource = null;
                dgAttendance.ItemsSource = _todayAttendances;

                // تحديث حالة اليوم
                UpdateDayStatus();

                // تحديث أزرار الحضور والانصراف
                UpdateAttendanceButtons();

                MessageBox.Show("تم تسجيل الحضور بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تسجيل الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر تسجيل انصراف
        /// </summary>
        private async void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_recognizedEmployee == null || _todayAttendance == null)
                {
                    MessageBox.Show("الرجاء التعرف على الموظف أولاً", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // حفظ صورة الانصراف
                string imagePath = await _faceRecognitionService.SaveFaceImageAsync(_recognizedEmployee.Id, _currentFrame);

                // تسجيل الانصراف في قاعدة البيانات
                bool success = await _databaseService.RecordCheckOutAsync(
                    _todayAttendance.Id,
                    DateTime.Now,
                    imagePath,
                    false,
                    "جهاز الحضور الرئيسي"
                );

                if (success)
                {
                    // تحديث سجل الحضور
                    _todayAttendance.CheckOutTime = DateTime.Now;
                    _todayAttendance.CheckOutImagePath = imagePath;
                    _todayAttendance.IsManualCheckOut = false;
                    _todayAttendance.LastUpdated = DateTime.Now;

                    // تحديث الجدول
                    dgAttendance.ItemsSource = null;
                    dgAttendance.ItemsSource = _todayAttendances;

                    // تحديث حالة اليوم
                    UpdateDayStatus();

                    // تحديث أزرار الحضور والانصراف
                    UpdateAttendanceButtons();

                    MessageBox.Show("تم تسجيل الانصراف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("فشل في تسجيل الانصراف", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تسجيل الانصراف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
