using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// واجهة إدارة المرتبات
    /// </summary>
    public partial class SalaryManagement : Window
    {
        private readonly DatabaseService _databaseService;
        private readonly Employee _employee;
        private List<Salary> _allSalaries;
        private CollectionViewSource _salariesViewSource;

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// إنشاء نافذة إدارة المرتبات
        /// </summary>
        public SalaryManagement(User currentUser, Employee employee)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            _employee = employee;
            _databaseService = new DatabaseService(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeAttendance.mdf;Integrated Security=True");
            _salariesViewSource = new CollectionViewSource();
            dgSalaries.ItemsSource = _salariesViewSource.View;

            // تعيين بيانات الموظف
            txtEmployeeName.Text = _employee.Name;
            txtEmployeeNumber.Text = _employee.EmployeeNumber;
            txtBasicSalary.Text = _employee.MonthlySalary.ToString("N2");

            // تهيئة قائمة السنوات
            InitializeYearsList();

            // تحميل البيانات عند تحميل النافذة
            Loaded += SalaryManagement_Loaded;
        }

        /// <summary>
        /// تهيئة قائمة السنوات
        /// </summary>
        private void InitializeYearsList()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear + 1; year++)
            {
                cmbYear.Items.Add(year.ToString());
            }
            cmbYear.SelectedItem = currentYear.ToString();
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private async void SalaryManagement_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSalariesAsync();
        }

        /// <summary>
        /// تحميل بيانات المرتبات
        /// </summary>
        private async Task LoadSalariesAsync()
        {
            try
            {
                // عرض مؤشر التحميل
                loadingPanel.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;

                // الحصول على بيانات المرتبات
                // في التطبيق الحقيقي، سنقوم بالحصول على البيانات من قاعدة البيانات
                // هنا نقوم بإنشاء بيانات وهمية
                await Task.Delay(500);
                _allSalaries = GenerateDummySalaries();

                // تطبيق الفلتر حسب الشهر والسنة المحددين
                ApplyMonthYearFilter();

                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات المرتبات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // إخفاء مؤشر التحميل
                loadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// إنشاء بيانات وهمية للمرتبات
        /// </summary>
        private List<Salary> GenerateDummySalaries()
        {
            var salaries = new List<Salary>();
            int currentYear = DateTime.Now.Year;

            // إنشاء مرتبات للأشهر الستة الماضية
            for (int month = 1; month <= 12; month++)
            {
                // تخطي الأشهر المستقبلية
                if (month > DateTime.Now.Month && currentYear == DateTime.Now.Year)
                    continue;

                var salary = new Salary
                {
                    Id = month,
                    EmployeeId = _employee.Id,
                    Employee = _employee,
                    Month = month,
                    Year = currentYear,
                    BasicSalary = _employee.MonthlySalary,
                    HousingAllowance = _employee.MonthlySalary * 0.25m,
                    TransportationAllowance = _employee.MonthlySalary * 0.1m,
                    OtherAllowances = _employee.MonthlySalary * 0.05m,
                    Bonuses = month % 3 == 0 ? _employee.MonthlySalary * 0.1m : 0, // مكافأة كل 3 أشهر
                    AbsenceDeduction = month % 5 == 0 ? _employee.MonthlySalary * 0.05m : 0, // خصم غياب عشوائي
                    LateDeduction = month % 4 == 0 ? _employee.MonthlySalary * 0.02m : 0, // خصم تأخير عشوائي
                    Status = month < DateTime.Now.Month ? SalaryStatus.Paid : SalaryStatus.Pending,
                    PaymentDate = month < DateTime.Now.Month ? new DateTime(currentYear, month, 28) : null
                };

                salaries.Add(salary);
            }

            return salaries;
        }

        /// <summary>
        /// تطبيق الفلتر حسب الشهر والسنة
        /// </summary>
        private void ApplyMonthYearFilter()
        {
            if (_allSalaries == null) return;

            int selectedMonth = cmbMonth.SelectedIndex + 1;
            int selectedYear = int.Parse(cmbYear.SelectedItem.ToString());

            var filteredList = _allSalaries;

            // إذا تم تحديد شهر معين
            if (selectedMonth > 0)
            {
                filteredList = filteredList.Where(s => s.Month == selectedMonth && s.Year == selectedYear).ToList();
            }
            else
            {
                // إذا تم تحديد سنة فقط
                filteredList = filteredList.Where(s => s.Year == selectedYear).ToList();
            }

            _salariesViewSource.Source = filteredList;
        }

        /// <summary>
        /// تحديث حالة العرض
        /// </summary>
        private void UpdateDisplayStatus()
        {
            var currentSalaries = _salariesViewSource.Source as List<Salary>;

            if (currentSalaries == null || currentSalaries.Count == 0)
            {
                txtNoData.Visibility = Visibility.Visible;
                txtStatus.Text = "لا توجد مرتبات للعرض";
            }
            else
            {
                txtNoData.Visibility = Visibility.Collapsed;
                txtStatus.Text = $"إجمالي المرتبات: {currentSalaries.Count}";
            }
        }

        /// <summary>
        /// حدث تغيير الشهر
        /// </summary>
        private void cmbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyMonthYearFilter();
            UpdateDisplayStatus();
        }

        /// <summary>
        /// حدث تغيير السنة
        /// </summary>
        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbYear.SelectedItem != null)
            {
                ApplyMonthYearFilter();
                UpdateDisplayStatus();
            }
        }

        /// <summary>
        /// حدث النقر على زر إضافة مرتب
        /// </summary>
        private void btnAddSalary_Click(object sender, RoutedEventArgs e)
        {
            // في التطبيق الحقيقي، سنقوم بفتح نافذة لإضافة مرتب جديد
            MessageBox.Show("سيتم فتح نافذة إضافة مرتب جديد", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// حدث النقر على زر تحديث
        /// </summary>
        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadSalariesAsync();
        }

        /// <summary>
        /// حدث النقر على زر تعديل مرتب
        /// </summary>
        private void btnEditSalary_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int salaryId)
            {
                var salary = _allSalaries.FirstOrDefault(s => s.Id == salaryId);
                if (salary != null)
                {
                    // في التطبيق الحقيقي، سنقوم بفتح نافذة لتعديل المرتب
                    MessageBox.Show($"سيتم فتح نافذة تعديل المرتب لشهر {GetMonthName(salary.Month)} {salary.Year}", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// حدث النقر على زر دفع مرتب
        /// </summary>
        private void btnPaySalary_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int salaryId)
            {
                var salary = _allSalaries.FirstOrDefault(s => s.Id == salaryId);
                if (salary != null)
                {
                    var result = MessageBox.Show(
                        $"هل أنت متأكد من تأكيد دفع مرتب {GetMonthName(salary.Month)} {salary.Year} للموظف {_employee.Name}؟",
                        "تأكيد الدفع",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // في التطبيق الحقيقي، سنقوم بتحديث حالة المرتب في قاعدة البيانات
                        salary.Status = SalaryStatus.Paid;
                        salary.PaymentDate = DateTime.Now;

                        // تحديث العرض
                        _salariesViewSource.View.Refresh();
                        MessageBox.Show("تم تأكيد دفع المرتب بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        /// <summary>
        /// حدث النقر على زر حذف مرتب
        /// </summary>
        private void btnDeleteSalary_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int salaryId)
            {
                var salary = _allSalaries.FirstOrDefault(s => s.Id == salaryId);
                if (salary != null)
                {
                    var result = MessageBox.Show(
                        $"هل أنت متأكد من حذف مرتب {GetMonthName(salary.Month)} {salary.Year} للموظف {_employee.Name}؟",
                        "تأكيد الحذف",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // في التطبيق الحقيقي، سنقوم بحذف المرتب من قاعدة البيانات
                        _allSalaries.Remove(salary);

                        // تحديث العرض
                        ApplyMonthYearFilter();
                        UpdateDisplayStatus();
                        MessageBox.Show("تم حذف المرتب بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
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
        /// حدث تغيير التحديد في جدول المرتبات
        /// </summary>
        private void dgSalaries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // يمكن إضافة منطق إضافي هنا إذا لزم الأمر
        }

        /// <summary>
        /// حدث النقر على زر الإغلاق
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    /// <summary>
    /// محول لتحويل حالة المرتب إلى حالة الظهور
    /// </summary>
    public class SalaryStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SalaryStatus status)
            {
                return status == SalaryStatus.Pending ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
