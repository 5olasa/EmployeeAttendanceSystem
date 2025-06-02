using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EmployeeAttendance.Mobile.Services;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// نموذج عرض صفحة تسجيل الحضور
    /// </summary>
    public class AttendanceViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly ISettingsService _settingsService;

        private string _employeeName;
        private string _employeeNumber;
        private string _employeePosition;
        private ImageSource _cameraPreview;
        private bool _isFaceDetected;
        private string _cameraStatus;
        private bool _canCheckIn;
        private bool _canCheckOut;
        private string _busyMessage;

        /// <summary>
        /// أمر بدء تشغيل الكاميرا
        /// </summary>
        public ICommand StartCameraCommand { get; }

        /// <summary>
        /// أمر إيقاف تشغيل الكاميرا
        /// </summary>
        public ICommand StopCameraCommand { get; }

        /// <summary>
        /// أمر تسجيل الحضور
        /// </summary>
        public ICommand CheckInCommand { get; }

        /// <summary>
        /// أمر تسجيل الانصراف
        /// </summary>
        public ICommand CheckOutCommand { get; }

        /// <summary>
        /// اسم الموظف
        /// </summary>
        public string EmployeeName
        {
            get => _employeeName;
            set => SetProperty(ref _employeeName, value);
        }

        /// <summary>
        /// رقم الموظف
        /// </summary>
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set => SetProperty(ref _employeeNumber, value);
        }

        /// <summary>
        /// المسمى الوظيفي
        /// </summary>
        public string EmployeePosition
        {
            get => _employeePosition;
            set => SetProperty(ref _employeePosition, value);
        }

        /// <summary>
        /// معاينة الكاميرا
        /// </summary>
        public ImageSource CameraPreview
        {
            get => _cameraPreview;
            set => SetProperty(ref _cameraPreview, value);
        }

        /// <summary>
        /// مؤشر اكتشاف الوجه
        /// </summary>
        public bool IsFaceDetected
        {
            get => _isFaceDetected;
            set => SetProperty(ref _isFaceDetected, value);
        }

        /// <summary>
        /// حالة الكاميرا
        /// </summary>
        public string CameraStatus
        {
            get => _cameraStatus;
            set => SetProperty(ref _cameraStatus, value);
        }

        /// <summary>
        /// إمكانية تسجيل الحضور
        /// </summary>
        public bool CanCheckIn
        {
            get => _canCheckIn;
            set => SetProperty(ref _canCheckIn, value);
        }

        /// <summary>
        /// إمكانية تسجيل الانصراف
        /// </summary>
        public bool CanCheckOut
        {
            get => _canCheckOut;
            set => SetProperty(ref _canCheckOut, value);
        }

        /// <summary>
        /// رسالة الانشغال
        /// </summary>
        public string BusyMessage
        {
            get => _busyMessage;
            set => SetProperty(ref _busyMessage, value);
        }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public AttendanceViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            _settingsService = settingsService;

            Title = "تسجيل الحضور";
            StartCameraCommand = new Command(async () => await StartCameraAsync());
            StopCameraCommand = new Command(StopCamera);
            CheckInCommand = new Command(async () => await CheckInAsync());
            CheckOutCommand = new Command(async () => await CheckOutAsync());

            // تعيين حالة الكاميرا
            CameraStatus = "جاري تهيئة الكاميرا...";
            IsFaceDetected = false;

            // تحميل بيانات الموظف وحالة الحضور
            _ = LoadEmployeeDataAsync();
        }

        /// <summary>
        /// تحميل بيانات الموظف
        /// </summary>
        private async Task LoadEmployeeDataAsync()
        {
            try
            {
                // الحصول على معرف الموظف من الإعدادات
                var (_, userId) = _settingsService.GetSessionData();
                if (string.IsNullOrEmpty(userId))
                {
                    // إذا لم يتم العثور على معرف الموظف، استخدم بيانات وهمية
                    EmployeeName = "أحمد محمد";
                    EmployeeNumber = "EMP001";
                    EmployeePosition = "مطور برمجيات";
                    UpdateAttendanceStatus();
                    return;
                }

                // تحميل بيانات الموظف من الخادم
                var employee = await _apiService.GetEmployeeAsync(userId);
                if (employee != null)
                {
                    EmployeeName = employee.Name;
                    EmployeeNumber = employee.EmployeeNumber;
                    EmployeePosition = "موظف"; // يمكن إضافة حقل المسمى الوظيفي لاحقاً
                }

                // تحميل حالة الحضور لليوم الحالي
                await LoadTodayAttendanceStatusAsync(userId);
            }
            catch (Exception ex)
            {
                // في حالة الخطأ، استخدم بيانات وهمية
                EmployeeName = "أحمد محمد";
                EmployeeNumber = "EMP001";
                EmployeePosition = "مطور برمجيات";
                UpdateAttendanceStatus();

                Console.WriteLine($"Error loading employee data: {ex.Message}");
            }
        }

        /// <summary>
        /// تحميل حالة الحضور لليوم الحالي
        /// </summary>
        private async Task LoadTodayAttendanceStatusAsync(string employeeId)
        {
            try
            {
                var records = await _apiService.GetAttendanceRecordsAsync(employeeId, DateTime.Now.Month, DateTime.Now.Year);
                var todayRecord = records?.FirstOrDefault(r => r.Date.Date == DateTime.Today);

                if (todayRecord != null)
                {
                    bool hasCheckedIn = todayRecord.CheckInTime.HasValue;
                    bool hasCheckedOut = todayRecord.CheckOutTime.HasValue;
                    UpdateAttendanceStatus(hasCheckedIn, hasCheckedOut);
                }
                else
                {
                    UpdateAttendanceStatus();
                }
            }
            catch (Exception ex)
            {
                UpdateAttendanceStatus();
                Console.WriteLine($"Error loading attendance status: {ex.Message}");
            }
        }

        /// <summary>
        /// بدء تشغيل الكاميرا
        /// </summary>
        private async Task StartCameraAsync()
        {
            if (IsBusy)
                return;

            BusyMessage = "جاري تهيئة الكاميرا...";
            await RunBusyAsync(async () =>
            {
                try
                {
                    // في التطبيق الحقيقي، سنقوم بتهيئة الكاميرا وبدء التقاط الصور
                    // var cameraService = new CameraService();
                    // await cameraService.InitializeAsync();
                    // cameraService.FrameCaptured += OnFrameCaptured;
                    // cameraService.FaceDetected += OnFaceDetected;
                    // await cameraService.StartPreviewAsync();
                    
                    // محاكاة تأخير تهيئة الكاميرا
                    await Task.Delay(2000);
                    
                    // تعيين صورة وهمية للمعاينة
                    CameraPreview = "camera_preview.png";
                    
                    // تعيين حالة الكاميرا
                    CameraStatus = "الرجاء النظر إلى الكاميرا...";
                    
                    // بدء محاكاة اكتشاف الوجه
                    StartFaceDetectionSimulation();
                }
                catch (Exception ex)
                {
                    CameraStatus = $"خطأ في تهيئة الكاميرا: {ex.Message}";
                    await Shell.Current.DisplayAlert("خطأ", $"حدث خطأ أثناء تهيئة الكاميرا: {ex.Message}", "موافق");
                }
            });
        }

        /// <summary>
        /// إيقاف تشغيل الكاميرا
        /// </summary>
        private void StopCamera()
        {
            try
            {
                // في التطبيق الحقيقي، سنقوم بإيقاف تشغيل الكاميرا
                // var cameraService = new CameraService();
                // cameraService.FrameCaptured -= OnFrameCaptured;
                // cameraService.FaceDetected -= OnFaceDetected;
                // cameraService.StopPreview();
                
                // إيقاف محاكاة اكتشاف الوجه
                StopFaceDetectionSimulation();
                
                // تعيين حالة الكاميرا
                CameraStatus = "تم إيقاف تشغيل الكاميرا";
                IsFaceDetected = false;
            }
            catch (Exception ex)
            {
                CameraStatus = $"خطأ في إيقاف تشغيل الكاميرا: {ex.Message}";
            }
        }

        /// <summary>
        /// تسجيل الحضور
        /// </summary>
        private async Task CheckInAsync()
        {
            if (IsBusy || !CanCheckIn)
                return;

            BusyMessage = "جاري تسجيل الحضور...";
            await RunBusyAsync(async () =>
            {
                try
                {
                    // الحصول على معرف الموظف من الإعدادات
                    var (_, userId) = _settingsService.GetSessionData();
                    if (string.IsNullOrEmpty(userId))
                    {
                        await Shell.Current.DisplayAlert("خطأ", "لم يتم العثور على بيانات الموظف", "موافق");
                        return;
                    }

                    // تسجيل الحضور عبر API
                    var attendance = await _apiService.CheckInAsync(userId, "dummy_face_data");

                    if (attendance != null)
                    {
                        // تحديث حالة تسجيل الحضور والانصراف
                        UpdateAttendanceStatus(true, false);

                        // عرض رسالة نجاح
                        await Shell.Current.DisplayAlert("نجاح", "تم تسجيل الحضور بنجاح", "موافق");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("خطأ", "فشل في تسجيل الحضور", "موافق");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    if (httpEx.Message.Contains("400"))
                    {
                        await Shell.Current.DisplayAlert("خطأ", "تم تسجيل الحضور مسبقاً لهذا اليوم", "موافق");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("خطأ", "لا يمكن الاتصال بالخادم", "موافق");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"حدث خطأ أثناء تسجيل الحضور: {ex.Message}", "موافق");
                }
            });
        }

        /// <summary>
        /// تسجيل الانصراف
        /// </summary>
        private async Task CheckOutAsync()
        {
            if (IsBusy || !CanCheckOut)
                return;

            BusyMessage = "جاري تسجيل الانصراف...";
            await RunBusyAsync(async () =>
            {
                try
                {
                    // الحصول على معرف الموظف من الإعدادات
                    var (_, userId) = _settingsService.GetSessionData();
                    if (string.IsNullOrEmpty(userId))
                    {
                        await Shell.Current.DisplayAlert("خطأ", "لم يتم العثور على بيانات الموظف", "موافق");
                        return;
                    }

                    // تسجيل الانصراف عبر API
                    var attendance = await _apiService.CheckOutAsync(userId, "dummy_face_data");

                    if (attendance != null)
                    {
                        // تحديث حالة تسجيل الحضور والانصراف
                        UpdateAttendanceStatus(true, true);

                        // عرض رسالة نجاح
                        await Shell.Current.DisplayAlert("نجاح", "تم تسجيل الانصراف بنجاح", "موافق");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("خطأ", "فشل في تسجيل الانصراف", "موافق");
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    if (httpEx.Message.Contains("400"))
                    {
                        await Shell.Current.DisplayAlert("خطأ", "لم يتم تسجيل الحضور بعد أو تم تسجيل الانصراف مسبقاً", "موافق");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("خطأ", "لا يمكن الاتصال بالخادم", "موافق");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"حدث خطأ أثناء تسجيل الانصراف: {ex.Message}", "موافق");
                }
            });
        }

        /// <summary>
        /// تحديث حالة تسجيل الحضور والانصراف
        /// </summary>
        private void UpdateAttendanceStatus(bool hasCheckedIn = false, bool hasCheckedOut = false)
        {
            // في التطبيق الحقيقي، سنقوم بالتحقق من حالة تسجيل الحضور والانصراف من قاعدة البيانات
            // var attendanceService = new AttendanceService();
            // var todayAttendance = await attendanceService.GetTodayAttendanceAsync(Preferences.Get("UserId", string.Empty));
            // bool hasCheckedIn = todayAttendance != null && todayAttendance.CheckInTime.HasValue;
            // bool hasCheckedOut = todayAttendance != null && todayAttendance.CheckOutTime.HasValue;
            
            // تعيين إمكانية تسجيل الحضور والانصراف
            CanCheckIn = !hasCheckedIn;
            CanCheckOut = hasCheckedIn && !hasCheckedOut;
        }

        #region Face Detection Simulation

        private System.Timers.Timer _faceDetectionTimer;

        /// <summary>
        /// بدء محاكاة اكتشاف الوجه
        /// </summary>
        private void StartFaceDetectionSimulation()
        {
            _faceDetectionTimer = new System.Timers.Timer(2000);
            _faceDetectionTimer.Elapsed += (s, e) =>
            {
                // محاكاة اكتشاف الوجه بشكل عشوائي
                var random = new Random();
                IsFaceDetected = random.Next(100) < 70; // 70% احتمالية اكتشاف الوجه
                
                // تحديث حالة الكاميرا
                CameraStatus = IsFaceDetected ? "تم التعرف على الوجه" : "الرجاء النظر إلى الكاميرا...";
            };
            _faceDetectionTimer.Start();
        }

        /// <summary>
        /// إيقاف محاكاة اكتشاف الوجه
        /// </summary>
        private void StopFaceDetectionSimulation()
        {
            _faceDetectionTimer?.Stop();
            _faceDetectionTimer?.Dispose();
            _faceDetectionTimer = null;
        }

        #endregion
    }
}
