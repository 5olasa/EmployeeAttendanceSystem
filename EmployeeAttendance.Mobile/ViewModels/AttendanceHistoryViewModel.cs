using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// نموذج عرض صفحة سجل الحضور
    /// </summary>
    public class AttendanceHistoryViewModel : BaseViewModel
    {
        private ObservableCollection<AttendanceRecordViewModel> _attendanceRecords;
        private List<string> _months;
        private List<int> _years;
        private string _selectedMonth;
        private int _selectedYear;
        private bool _isRefreshing;

        /// <summary>
        /// أمر تحميل البيانات
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// أمر البحث
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// أمر التحديث
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// سجلات الحضور
        /// </summary>
        public ObservableCollection<AttendanceRecordViewModel> AttendanceRecords
        {
            get => _attendanceRecords;
            set => SetProperty(ref _attendanceRecords, value);
        }

        /// <summary>
        /// قائمة الشهور
        /// </summary>
        public List<string> Months
        {
            get => _months;
            set => SetProperty(ref _months, value);
        }

        /// <summary>
        /// قائمة السنوات
        /// </summary>
        public List<int> Years
        {
            get => _years;
            set => SetProperty(ref _years, value);
        }

        /// <summary>
        /// الشهر المحدد
        /// </summary>
        public string SelectedMonth
        {
            get => _selectedMonth;
            set => SetProperty(ref _selectedMonth, value);
        }

        /// <summary>
        /// السنة المحددة
        /// </summary>
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        /// <summary>
        /// مؤشر التحديث
        /// </summary>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public AttendanceHistoryViewModel()
        {
            Title = "سجل الحضور";
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            SearchCommand = new Command(async () => await SearchAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());

            // تهيئة قائمة سجلات الحضور
            AttendanceRecords = new ObservableCollection<AttendanceRecordViewModel>();

            // تهيئة قائمة الشهور
            Months = new List<string>
            {
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };

            // تهيئة قائمة السنوات
            Years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 2; i <= currentYear; i++)
            {
                Years.Add(i);
            }

            // تعيين الشهر والسنة الحاليين
            SelectedMonth = Months[DateTime.Now.Month - 1];
            SelectedYear = currentYear;
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
                    // في التطبيق الحقيقي، سنقوم بتحميل البيانات من الخادم
                    // var attendanceService = new AttendanceService();
                    // var records = await attendanceService.GetAttendanceRecordsAsync(
                    //     Preferences.Get("UserId", string.Empty),
                    //     Months.IndexOf(SelectedMonth) + 1,
                    //     SelectedYear);
                    
                    // محاكاة تأخير الاتصال بالخادم
                    await Task.Delay(1000);
                    
                    // إنشاء بيانات وهمية
                    var records = GenerateDummyAttendanceRecords();
                    
                    // تحديث قائمة سجلات الحضور
                    AttendanceRecords.Clear();
                    foreach (var record in records)
                    {
                        AttendanceRecords.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل البيانات: {ex.Message}", "موافق");
                }
            });
        }

        /// <summary>
        /// البحث عن سجلات الحضور
        /// </summary>
        private async Task SearchAsync()
        {
            await LoadDataAsync();
        }

        /// <summary>
        /// تحديث البيانات
        /// </summary>
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadDataAsync();
            IsRefreshing = false;
        }

        /// <summary>
        /// إنشاء بيانات وهمية لسجلات الحضور
        /// </summary>
        private List<AttendanceRecordViewModel> GenerateDummyAttendanceRecords()
        {
            var records = new List<AttendanceRecordViewModel>();
            var random = new Random();
            
            // الحصول على عدد أيام الشهر المحدد
            int month = Months.IndexOf(SelectedMonth) + 1;
            int daysInMonth = DateTime.DaysInMonth(SelectedYear, month);
            
            // إنشاء سجلات الحضور لكل يوم في الشهر
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(SelectedYear, month, day);
                
                // تخطي أيام الإجازة (الجمعة والسبت)
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                    continue;
                    
                // تخطي الأيام المستقبلية
                if (date > DateTime.Now)
                    continue;
                
                // إنشاء سجل حضور وهمي
                var checkInHour = 8;
                var checkInMinute = random.Next(0, 30);
                var isLate = checkInMinute > 15;
                
                var checkOutHour = 16;
                var checkOutMinute = random.Next(0, 30);
                
                var record = new AttendanceRecordViewModel
                {
                    Date = date.ToString("dd MMMM yyyy", new CultureInfo("ar-SA")),
                    DayOfWeek = date.ToString("dddd", new CultureInfo("ar-SA")),
                    CheckInTime = $"{checkInHour:D2}:{checkInMinute:D2} ص",
                    CheckOutTime = $"{checkOutHour:D2}:{checkOutMinute:D2} م",
                    CheckInStatusColor = isLate ? Colors.Red : Colors.Green,
                    CheckOutStatusColor = Colors.Green
                };
                
                records.Add(record);
            }
            
            // ترتيب السجلات حسب التاريخ (الأحدث أولاً)
            return records.OrderByDescending(r => DateTime.ParseExact(r.Date, "dd MMMM yyyy", new CultureInfo("ar-SA"))).ToList();
        }
    }

    /// <summary>
    /// نموذج عرض سجل الحضور
    /// </summary>
    public class AttendanceRecordViewModel : BaseViewModel
    {
        private string _date;
        private string _dayOfWeek;
        private string _checkInTime;
        private string _checkOutTime;
        private Color _checkInStatusColor;
        private Color _checkOutStatusColor;

        /// <summary>
        /// التاريخ
        /// </summary>
        public string Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        /// <summary>
        /// يوم الأسبوع
        /// </summary>
        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
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
        /// لون حالة الحضور
        /// </summary>
        public Color CheckInStatusColor
        {
            get => _checkInStatusColor;
            set => SetProperty(ref _checkInStatusColor, value);
        }

        /// <summary>
        /// لون حالة الانصراف
        /// </summary>
        public Color CheckOutStatusColor
        {
            get => _checkOutStatusColor;
            set => SetProperty(ref _checkOutStatusColor, value);
        }
    }
}
