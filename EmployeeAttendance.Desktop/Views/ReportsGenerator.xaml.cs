using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// واجهة توليد التقارير
    /// </summary>
    public partial class ReportsGenerator : Window
    {
        private readonly DatabaseService _databaseService;
        private List<Employee> _allEmployees;
        private DataTable _reportData;

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// إنشاء نافذة توليد التقارير
        /// </summary>
        public ReportsGenerator(User currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            _databaseService = new DatabaseService(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeAttendance.mdf;Integrated Security=True");
            
            // تعيين التاريخ الافتراضي
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today;
            
            // تحميل البيانات عند تحميل النافذة
            Loaded += ReportsGenerator_Loaded;
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private async void ReportsGenerator_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // تحميل قائمة الموظفين
                await LoadEmployeesAsync();
                
                // تحديث واجهة المستخدم بناءً على نوع التقرير المحدد
                UpdateUIBasedOnReportType();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// تحميل قائمة الموظفين
        /// </summary>
        private async System.Threading.Tasks.Task LoadEmployeesAsync()
        {
            try
            {
                // في التطبيق الحقيقي، سنقوم بالحصول على البيانات من قاعدة البيانات
                // _allEmployees = await _databaseService.GetAllEmployeesAsync();
                
                // إنشاء بيانات وهمية للموظفين
                await System.Threading.Tasks.Task.Delay(500); // تأخير وهمي لمحاكاة الاتصال بقاعدة البيانات
                _allEmployees = GenerateDummyEmployees();
                
                // إضافة الموظفين إلى القائمة المنسدلة
                foreach (var employee in _allEmployees)
                {
                    cmbEmployee.Items.Add(new ComboBoxItem { Content = employee.Name, Tag = employee.Id });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات الموظفين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// إنشاء بيانات وهمية للموظفين
        /// </summary>
        private List<Employee> GenerateDummyEmployees()
        {
            var employees = new List<Employee>();
            
            // إضافة بعض الموظفين الوهميين
            employees.Add(new Employee
            {
                Id = 1,
                EmployeeNumber = "EMP001",
                Name = "أحمد محمد",
                Email = "ahmed@example.com",
                Phone = "0501234567",
                HireDate = new DateTime(2020, 5, 15),
                ShiftId = 1,
                MonthlySalary = 10000,
                AvailableVacationDays = 21
            });
            
            employees.Add(new Employee
            {
                Id = 2,
                EmployeeNumber = "EMP002",
                Name = "سارة علي",
                Email = "sara@example.com",
                Phone = "0507654321",
                HireDate = new DateTime(2019, 3, 10),
                ShiftId = 2,
                MonthlySalary = 12000,
                AvailableVacationDays = 24
            });
            
            employees.Add(new Employee
            {
                Id = 3,
                EmployeeNumber = "EMP003",
                Name = "محمد خالد",
                Email = "mohammed@example.com",
                Phone = "0509876543",
                HireDate = new DateTime(2021, 1, 20),
                ShiftId = 1,
                MonthlySalary = 9000,
                AvailableVacationDays = 18
            });
            
            return employees;
        }

        /// <summary>
        /// تحديث واجهة المستخدم بناءً على نوع التقرير المحدد
        /// </summary>
        private void UpdateUIBasedOnReportType()
        {
            // الحصول على نوع التقرير المحدد
            var selectedReportType = cmbReportType.SelectedIndex;
            
            // تحديث واجهة المستخدم بناءً على نوع التقرير
            switch (selectedReportType)
            {
                case 0: // تقرير الحضور
                    // تمكين جميع عناصر واجهة المستخدم
                    cmbTimePeriod.IsEnabled = true;
                    cmbEmployee.IsEnabled = true;
                    break;
                    
                case 1: // تقرير المرتبات
                    // تمكين عناصر واجهة المستخدم المناسبة
                    cmbTimePeriod.IsEnabled = true;
                    cmbEmployee.IsEnabled = true;
                    break;
                    
                case 2: // تقرير الموظفين
                    // تعطيل عناصر واجهة المستخدم غير المناسبة
                    cmbTimePeriod.IsEnabled = false;
                    cmbEmployee.IsEnabled = false;
                    break;
            }
            
            // تحديث واجهة المستخدم بناءً على الفترة الزمنية المحددة
            UpdateUIBasedOnTimePeriod();
        }

        /// <summary>
        /// تحديث واجهة المستخدم بناءً على الفترة الزمنية المحددة
        /// </summary>
        private void UpdateUIBasedOnTimePeriod()
        {
            // الحصول على الفترة الزمنية المحددة
            var selectedTimePeriod = cmbTimePeriod.SelectedIndex;
            
            // تحديث واجهة المستخدم بناءً على الفترة الزمنية
            switch (selectedTimePeriod)
            {
                case 0: // اليوم
                case 1: // الأسبوع الحالي
                case 2: // الشهر الحالي
                    // إخفاء حقول تاريخ البداية والنهاية
                    pnlStartDate.Visibility = Visibility.Collapsed;
                    pnlEndDate.Visibility = Visibility.Collapsed;
                    break;
                    
                case 3: // فترة محددة
                    // إظهار حقول تاريخ البداية والنهاية
                    pnlStartDate.Visibility = Visibility.Visible;
                    pnlEndDate.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// حدث تغيير نوع التقرير
        /// </summary>
        private void cmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUIBasedOnReportType();
        }

        /// <summary>
        /// حدث تغيير الفترة الزمنية
        /// </summary>
        private void cmbTimePeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUIBasedOnTimePeriod();
        }

        /// <summary>
        /// حدث تغيير تاريخ البداية
        /// </summary>
        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // التحقق من أن تاريخ البداية قبل تاريخ النهاية
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue && 
                dpStartDate.SelectedDate.Value > dpEndDate.SelectedDate.Value)
            {
                dpEndDate.SelectedDate = dpStartDate.SelectedDate;
            }
        }

        /// <summary>
        /// حدث تغيير تاريخ النهاية
        /// </summary>
        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // التحقق من أن تاريخ النهاية بعد تاريخ البداية
            if (dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue && 
                dpEndDate.SelectedDate.Value < dpStartDate.SelectedDate.Value)
            {
                dpStartDate.SelectedDate = dpEndDate.SelectedDate;
            }
        }

        /// <summary>
        /// حدث تغيير الموظف
        /// </summary>
        private void cmbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // يمكن إضافة منطق إضافي هنا إذا لزم الأمر
        }

        /// <summary>
        /// حدث النقر على زر توليد التقرير
        /// </summary>
        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // عرض مؤشر التحميل
                loadingPanel.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;
                
                // توليد التقرير
                GenerateReport();
                
                // تمكين زر التصدير
                btnExportReport.IsEnabled = true;
                
                // تحديث حالة العرض
                txtStatus.Text = "تم توليد التقرير بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء توليد التقرير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "حدث خطأ أثناء توليد التقرير";
            }
            finally
            {
                // إخفاء مؤشر التحميل
                loadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// توليد التقرير
        /// </summary>
        private void GenerateReport()
        {
            // الحصول على نوع التقرير المحدد
            var selectedReportType = cmbReportType.SelectedIndex;
            
            // توليد التقرير المناسب
            switch (selectedReportType)
            {
                case 0: // تقرير الحضور
                    GenerateAttendanceReport();
                    break;
                    
                case 1: // تقرير المرتبات
                    GenerateSalaryReport();
                    break;
                    
                case 2: // تقرير الموظفين
                    GenerateEmployeeReport();
                    break;
            }
            
            // عرض البيانات في جدول المعاينة
            dgReportPreview.ItemsSource = _reportData.DefaultView;
            
            // إخفاء رسالة عدم وجود بيانات إذا كان هناك بيانات
            txtNoData.Visibility = _reportData.Rows.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// توليد تقرير الحضور
        /// </summary>
        private void GenerateAttendanceReport()
        {
            // إنشاء جدول البيانات
            _reportData = new DataTable();
            
            // إضافة الأعمدة
            _reportData.Columns.Add("الرقم الوظيفي", typeof(string));
            _reportData.Columns.Add("اسم الموظف", typeof(string));
            _reportData.Columns.Add("التاريخ", typeof(string));
            _reportData.Columns.Add("وقت الحضور", typeof(string));
            _reportData.Columns.Add("وقت الانصراف", typeof(string));
            _reportData.Columns.Add("عدد ساعات العمل", typeof(double));
            _reportData.Columns.Add("حالة التأخير", typeof(string));
            
            // إضافة بيانات وهمية
            var random = new Random();
            var startDate = GetReportStartDate();
            var endDate = GetReportEndDate();
            
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                foreach (var employee in GetSelectedEmployees())
                {
                    // تخطي أيام الإجازة (الجمعة والسبت)
                    if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                        continue;
                        
                    var checkInTime = new DateTime(date.Year, date.Month, date.Day, 8, random.Next(0, 30), 0);
                    var checkOutTime = new DateTime(date.Year, date.Month, date.Day, 16, random.Next(0, 30), 0);
                    var workHours = (checkOutTime - checkInTime).TotalHours;
                    var isLate = checkInTime.Hour >= 8 && checkInTime.Minute > 15;
                    
                    _reportData.Rows.Add(
                        employee.EmployeeNumber,
                        employee.Name,
                        date.ToString("dd/MM/yyyy"),
                        checkInTime.ToString("hh:mm tt"),
                        checkOutTime.ToString("hh:mm tt"),
                        Math.Round(workHours, 2),
                        isLate ? "متأخر" : "في الموعد"
                    );
                }
            }
        }

        /// <summary>
        /// توليد تقرير المرتبات
        /// </summary>
        private void GenerateSalaryReport()
        {
            // إنشاء جدول البيانات
            _reportData = new DataTable();
            
            // إضافة الأعمدة
            _reportData.Columns.Add("الرقم الوظيفي", typeof(string));
            _reportData.Columns.Add("اسم الموظف", typeof(string));
            _reportData.Columns.Add("الشهر", typeof(string));
            _reportData.Columns.Add("السنة", typeof(int));
            _reportData.Columns.Add("الراتب الأساسي", typeof(decimal));
            _reportData.Columns.Add("البدلات", typeof(decimal));
            _reportData.Columns.Add("المكافآت", typeof(decimal));
            _reportData.Columns.Add("الخصومات", typeof(decimal));
            _reportData.Columns.Add("صافي المرتب", typeof(decimal));
            _reportData.Columns.Add("حالة الدفع", typeof(string));
            
            // إضافة بيانات وهمية
            var random = new Random();
            var startDate = GetReportStartDate();
            var endDate = GetReportEndDate();
            
            // الحصول على الشهور المطلوبة
            var months = new List<Tuple<int, int>>();
            for (var date = new DateTime(startDate.Year, startDate.Month, 1); 
                 date <= new DateTime(endDate.Year, endDate.Month, 1); 
                 date = date.AddMonths(1))
            {
                months.Add(new Tuple<int, int>(date.Month, date.Year));
            }
            
            foreach (var month in months)
            {
                foreach (var employee in GetSelectedEmployees())
                {
                    var basicSalary = employee.MonthlySalary;
                    var allowances = basicSalary * 0.35m;
                    var bonus = random.Next(0, 100) < 30 ? basicSalary * 0.1m : 0; // 30% احتمالية وجود مكافأة
                    var deductions = basicSalary * (decimal)random.Next(0, 10) / 100; // 0-10% خصومات
                    var netSalary = basicSalary + allowances + bonus - deductions;
                    var paymentStatus = month.Item1 < DateTime.Now.Month || month.Item2 < DateTime.Now.Year ? 
                                       "تم الدفع" : "قيد الانتظار";
                    
                    _reportData.Rows.Add(
                        employee.EmployeeNumber,
                        employee.Name,
                        GetMonthName(month.Item1),
                        month.Item2,
                        basicSalary,
                        allowances,
                        bonus,
                        deductions,
                        netSalary,
                        paymentStatus
                    );
                }
            }
        }

        /// <summary>
        /// توليد تقرير الموظفين
        /// </summary>
        private void GenerateEmployeeReport()
        {
            // إنشاء جدول البيانات
            _reportData = new DataTable();
            
            // إضافة الأعمدة
            _reportData.Columns.Add("الرقم الوظيفي", typeof(string));
            _reportData.Columns.Add("اسم الموظف", typeof(string));
            _reportData.Columns.Add("البريد الإلكتروني", typeof(string));
            _reportData.Columns.Add("رقم الهاتف", typeof(string));
            _reportData.Columns.Add("تاريخ التوظيف", typeof(string));
            _reportData.Columns.Add("نوع الشفت", typeof(int));
            _reportData.Columns.Add("الراتب الشهري", typeof(decimal));
            _reportData.Columns.Add("أيام الإجازة المتاحة", typeof(int));
            
            // إضافة بيانات الموظفين
            foreach (var employee in _allEmployees)
            {
                _reportData.Rows.Add(
                    employee.EmployeeNumber,
                    employee.Name,
                    employee.Email,
                    employee.Phone,
                    employee.HireDate.ToString("dd/MM/yyyy"),
                    employee.ShiftId,
                    employee.MonthlySalary,
                    employee.AvailableVacationDays
                );
            }
        }

        /// <summary>
        /// الحصول على تاريخ بداية التقرير
        /// </summary>
        private DateTime GetReportStartDate()
        {
            // الحصول على الفترة الزمنية المحددة
            var selectedTimePeriod = cmbTimePeriod.SelectedIndex;
            
            switch (selectedTimePeriod)
            {
                case 0: // اليوم
                    return DateTime.Today;
                    
                case 1: // الأسبوع الحالي
                    return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    
                case 2: // الشهر الحالي
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    
                case 3: // فترة محددة
                    return dpStartDate.SelectedDate ?? DateTime.Today;
                    
                default:
                    return DateTime.Today;
            }
        }

        /// <summary>
        /// الحصول على تاريخ نهاية التقرير
        /// </summary>
        private DateTime GetReportEndDate()
        {
            // الحصول على الفترة الزمنية المحددة
            var selectedTimePeriod = cmbTimePeriod.SelectedIndex;
            
            switch (selectedTimePeriod)
            {
                case 0: // اليوم
                    return DateTime.Today;
                    
                case 1: // الأسبوع الحالي
                    return DateTime.Today.AddDays(6 - (int)DateTime.Today.DayOfWeek);
                    
                case 2: // الشهر الحالي
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                    
                case 3: // فترة محددة
                    return dpEndDate.SelectedDate ?? DateTime.Today;
                    
                default:
                    return DateTime.Today;
            }
        }

        /// <summary>
        /// الحصول على الموظفين المحددين
        /// </summary>
        private List<Employee> GetSelectedEmployees()
        {
            // الحصول على الموظف المحدد
            var selectedEmployeeIndex = cmbEmployee.SelectedIndex;
            
            // إذا كان "جميع الموظفين" محددًا، إرجاع جميع الموظفين
            if (selectedEmployeeIndex == 0)
                return _allEmployees;
                
            // وإلا، إرجاع الموظف المحدد فقط
            var selectedEmployeeId = (int)((ComboBoxItem)cmbEmployee.SelectedItem).Tag;
            return _allEmployees.Where(e => e.Id == selectedEmployeeId).ToList();
        }

        /// <summary>
        /// الحصول على اسم الشهر
        /// </summary>
        private string GetMonthName(int month)
        {
            string[] monthNames = new string[] 
            { 
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", 
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" 
            };
            
            return monthNames[month - 1];
        }

        /// <summary>
        /// حدث النقر على زر تصدير التقرير
        /// </summary>
        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // الحصول على تنسيق التصدير المحدد
                var selectedExportFormat = cmbExportFormat.SelectedIndex;
                
                // تصدير التقرير
                switch (selectedExportFormat)
                {
                    case 0: // PDF
                        MessageBox.Show("سيتم تصدير التقرير بتنسيق PDF", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                        
                    case 1: // Excel
                        MessageBox.Show("سيتم تصدير التقرير بتنسيق Excel", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
                
                // تحديث حالة العرض
                txtStatus.Text = "تم تصدير التقرير بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تصدير التقرير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "حدث خطأ أثناء تصدير التقرير";
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
