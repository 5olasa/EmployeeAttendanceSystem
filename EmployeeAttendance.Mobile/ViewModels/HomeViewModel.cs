using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EmployeeAttendance.Mobile.Services;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// نموذج عرض الصفحة الرئيسية
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly ISettingsService _settingsService;

        private string _welcomeMessage;
        private string _currentDate;
        private string _checkInTime;
        private string _checkOutTime;

        /// <summary>
        /// أمر تحميل البيانات
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// أمر الانتقال إلى صفحة تسجيل الحضور
        /// </summary>
        public ICommand NavigateToAttendanceCommand { get; }

        /// <summary>
        /// أمر الانتقال إلى صفحة سجل الحضور
        /// </summary>
        public ICommand NavigateToHistoryCommand { get; }

        /// <summary>
        /// أمر الانتقال إلى صفحة الملف الشخصي
        /// </summary>
        public ICommand NavigateToProfileCommand { get; }

        /// <summary>
        /// أمر الانتقال إلى صفحة الإعدادات
        /// </summary>
        public ICommand NavigateToSettingsCommand { get; }

        /// <summary>
        /// رسالة الترحيب
        /// </summary>
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        /// <summary>
        /// التاريخ الحالي
        /// </summary>
        public string CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        /// <summary>
        /// وقت الحضور
        /// </summary>
        public string CheckInTime
        {
            get => _checkInTime;
            set => SetProperty(ref _checkInTime, value);
        }

        /// <summary>
        /// وقت الانصراف
        /// </summary>
        public string CheckOutTime
        {
            get => _checkOutTime;
            set => SetProperty(ref _checkOutTime, value);
        }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public HomeViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            _settingsService = settingsService;

            Title = "الرئيسية";
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            NavigateToAttendanceCommand = new Command(async () => await NavigateToAttendanceAsync());
            NavigateToHistoryCommand = new Command(async () => await NavigateToHistoryAsync());
            NavigateToProfileCommand = new Command(async () => await NavigateToProfileAsync());
            NavigateToSettingsCommand = new Command(async () => await NavigateToSettingsAsync());

            // تحميل البيانات عند إنشاء ViewModel
            _ = LoadDataAsync();
        }

        /// <summary>
        /// تحميل البيانات
        /// </summary>
        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            await RunBusyAsync(async () =>
            {
                try
                {
                    // تعيين التاريخ الحالي
                    CurrentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("ar-SA"));

                    // الحصول على معرف الموظف من الإعدادات
                    var (_, userId) = _settingsService.GetSessionData();
                    if (string.IsNullOrEmpty(userId))
                    {
                        // إذا لم يتم العثور على معرف الموظف، استخدم بيانات افتراضية
                        WelcomeMessage = GetWelcomeMessage() + " مستخدم";
                        CheckInTime = "لم يتم تسجيل الحضور بعد";
                        CheckOutTime = "لم يتم تسجيل الانصراف بعد";
                        return;
                    }

                    // تحميل بيانات الموظف من الخادم
                    var employee = await _apiService.GetEmployeeAsync(userId);
                    if (employee != null)
                    {
                        WelcomeMessage = GetWelcomeMessage() + " " + employee.Name;
                    }
                    else
                    {
                        WelcomeMessage = GetWelcomeMessage() + " مستخدم";
                    }

                    // تحميل سجلات الحضور لليوم الحالي
                    await LoadTodayAttendanceAsync(userId);
                }
                catch (Exception ex)
                {
                    // في حالة الخطأ، استخدم بيانات افتراضية
                    WelcomeMessage = GetWelcomeMessage() + " مستخدم";
                    CheckInTime = "لم يتم تسجيل الحضور بعد";
                    CheckOutTime = "لم يتم تسجيل الانصراف بعد";

                    Console.WriteLine($"Error loading home data: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// تحميل سجلات الحضور لليوم الحالي
        /// </summary>
        private async Task LoadTodayAttendanceAsync(string employeeId)
        {
            try
            {
                var records = await _apiService.GetAttendanceRecordsAsync(employeeId, DateTime.Now.Month, DateTime.Now.Year);
                var todayRecord = records?.FirstOrDefault(r => r.Date.Date == DateTime.Today);

                if (todayRecord != null)
                {
                    CheckInTime = todayRecord.CheckInTime?.ToString("HH:mm") ?? "لم يتم تسجيل الحضور بعد";
                    CheckOutTime = todayRecord.CheckOutTime?.ToString("HH:mm") ?? "لم يتم تسجيل الانصراف بعد";
                }
                else
                {
                    CheckInTime = "لم يتم تسجيل الحضور بعد";
                    CheckOutTime = "لم يتم تسجيل الانصراف بعد";
                }
            }
            catch (Exception ex)
            {
                CheckInTime = "لم يتم تسجيل الحضور بعد";
                CheckOutTime = "لم يتم تسجيل الانصراف بعد";
                Console.WriteLine($"Error loading today attendance: {ex.Message}");
            }
        }

        /// <summary>
        /// الحصول على رسالة الترحيب حسب الوقت
        /// </summary>
        private string GetWelcomeMessage()
        {
            var hour = DateTime.Now.Hour;
            
            if (hour >= 5 && hour < 12)
                return "صباح الخير،";
            else if (hour >= 12 && hour < 17)
                return "مساء الخير،";
            else
                return "مساء الخير،";
        }

        /// <summary>
        /// الانتقال إلى صفحة تسجيل الحضور
        /// </summary>
        private async Task NavigateToAttendanceAsync()
        {
            await Shell.Current.GoToAsync("//AttendancePage");
        }

        /// <summary>
        /// الانتقال إلى صفحة سجل الحضور
        /// </summary>
        private async Task NavigateToHistoryAsync()
        {
            await Shell.Current.GoToAsync("//AttendanceHistoryPage");
        }

        /// <summary>
        /// الانتقال إلى صفحة الملف الشخصي
        /// </summary>
        private async Task NavigateToProfileAsync()
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        /// <summary>
        /// الانتقال إلى صفحة الإعدادات
        /// </summary>
        private async Task NavigateToSettingsAsync()
        {
            await Shell.Current.GoToAsync("//SettingsPage");
        }
    }
}
