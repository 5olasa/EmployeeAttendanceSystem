using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// واجهة تسجيل الحضور اليدوي
    /// </summary>
    public partial class ManualAttendance : Window
    {
        private readonly DatabaseService _databaseService;
        private List<Employee> _allEmployees;
        private ObservableCollection<AttendanceRecord> _attendanceRecords;

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// إنشاء نافذة تسجيل الحضور اليدوي
        /// </summary>
        public ManualAttendance(User currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            _databaseService = new DatabaseService(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeAttendance.mdf;Integrated Security=True");
            _attendanceRecords = new ObservableCollection<AttendanceRecord>();
            
            // تعيين التاريخ الافتراضي
            dpDate.SelectedDate = DateTime.Today;
            
            // تعيين مصدر بيانات جدول الحضور
            dgAttendance.ItemsSource = _attendanceRecords;
            
            // تحميل البيانات عند تحميل النافذة
            Loaded += ManualAttendance_Loaded;
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private async void ManualAttendance_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // عرض مؤشر التحميل
                loadingPanel.Visibility = Visibility.Visible;
                
                // تحميل قائمة الموظفين
                await LoadEmployeesAsync();
                
                // تحميل سجل الحضور اليومي
                await LoadDailyAttendanceAsync();
                
                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // إخفاء مؤشر التحميل
                loadingPanel.Visibility = Visibility.Collapsed;
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
                
                // مسح القائمة المنسدلة
                cmbEmployee.Items.Clear();
                
                // إضافة الموظفين إلى القائمة المنسدلة
                foreach (var employee in _allEmployees)
                {
                    cmbEmployee.Items.Add(new ComboBoxItem { Content = employee.Name, Tag = employee.Id });
                }
                
                // تحديد الموظف الأول
                if (cmbEmployee.Items.Count > 0)
                {
                    cmbEmployee.SelectedIndex = 0;
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
        /// تحميل سجل الحضور اليومي
        /// </summary>
        private async System.Threading.Tasks.Task LoadDailyAttendanceAsync()
        {
            try
            {
                // في التطبيق الحقيقي، سنقوم بالحصول على البيانات من قاعدة البيانات
                // var attendanceRecords = await _databaseService.GetDailyAttendanceAsync(dpDate.SelectedDate.Value);
                
                // إنشاء بيانات وهمية لسجل الحضور
                await System.Threading.Tasks.Task.Delay(500); // تأخير وهمي لمحاكاة الاتصال بقاعدة البيانات
                var attendanceRecords = GenerateDummyAttendanceRecords();
                
                // مسح سجل الحضور الحالي
                _attendanceRecords.Clear();
                
                // إضافة سجلات الحضور
                foreach (var record in attendanceRecords)
                {
                    _attendanceRecords.Add(record);
                }
                
                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل سجل الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// إنشاء بيانات وهمية لسجل الحضور
        /// </summary>
        private List<AttendanceRecord> GenerateDummyAttendanceRecords()
        {
            var records = new List<AttendanceRecord>();
            
            // إضافة بعض سجلات الحضور الوهمية
            if (_allEmployees != null && _allEmployees.Count > 0)
            {
                var random = new Random();
                
                // إضافة سجل حضور للموظف الأول
                records.Add(new AttendanceRecord
                {
                    EmployeeNumber = _allEmployees[0].EmployeeNumber,
                    EmployeeName = _allEmployees[0].Name,
                    Date = dpDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    CheckInTime = "08:15",
                    CheckOutTime = "16:30",
                    Notes = "تم تسجيل الحضور والانصراف يدويًا"
                });
            }
            
            return records;
        }

        /// <summary>
        /// تحديث حالة العرض
        /// </summary>
        private void UpdateDisplayStatus()
        {
            // إظهار أو إخفاء رسالة عدم وجود بيانات
            txtNoData.Visibility = _attendanceRecords.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            
            // تحديث حالة العرض
            txtStatus.Text = $"عدد السجلات: {_attendanceRecords.Count}";
        }

        /// <summary>
        /// حدث تغيير الموظف
        /// </summary>
        private void cmbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // يمكن إضافة منطق إضافي هنا إذا لزم الأمر
        }

        /// <summary>
        /// حدث النقر على زر تسجيل الحضور
        /// </summary>
        private async void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // التحقق من اختيار موظف
                if (cmbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("الرجاء اختيار موظف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // الحصول على الموظف المحدد
                var selectedEmployeeItem = (ComboBoxItem)cmbEmployee.SelectedItem;
                var selectedEmployeeId = (int)selectedEmployeeItem.Tag;
                var selectedEmployee = _allEmployees.Find(emp => emp.Id == selectedEmployeeId);
                
                // الحصول على وقت الحضور
                var checkInHour = int.Parse(((ComboBoxItem)cmbCheckInHour.SelectedItem).Content.ToString());
                var checkInMinute = int.Parse(((ComboBoxItem)cmbCheckInMinute.SelectedItem).Content.ToString());
                var checkInTime = $"{checkInHour:D2}:{checkInMinute:D2}";
                
                // التحقق من عدم وجود سجل حضور للموظف في نفس اليوم
                var existingRecord = FindAttendanceRecord(selectedEmployee.EmployeeNumber);
                if (existingRecord != null)
                {
                    // تحديث سجل الحضور الموجود
                    existingRecord.CheckInTime = checkInTime;
                    existingRecord.Notes = txtNotes.Text;
                    
                    MessageBox.Show("تم تحديث وقت الحضور بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // إنشاء سجل حضور جديد
                    var newRecord = new AttendanceRecord
                    {
                        EmployeeNumber = selectedEmployee.EmployeeNumber,
                        EmployeeName = selectedEmployee.Name,
                        Date = dpDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                        CheckInTime = checkInTime,
                        CheckOutTime = "",
                        Notes = txtNotes.Text
                    };
                    
                    // في التطبيق الحقيقي، سنقوم بحفظ السجل في قاعدة البيانات
                    // await _databaseService.AddAttendanceRecordAsync(newRecord);
                    
                    // إضافة السجل إلى القائمة
                    _attendanceRecords.Add(newRecord);
                    
                    MessageBox.Show("تم تسجيل الحضور بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تسجيل الحضور: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر تسجيل الانصراف
        /// </summary>
        private async void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // التحقق من اختيار موظف
                if (cmbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("الرجاء اختيار موظف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // الحصول على الموظف المحدد
                var selectedEmployeeItem = (ComboBoxItem)cmbEmployee.SelectedItem;
                var selectedEmployeeId = (int)selectedEmployeeItem.Tag;
                var selectedEmployee = _allEmployees.Find(emp => emp.Id == selectedEmployeeId);
                
                // الحصول على وقت الانصراف
                var checkOutHour = int.Parse(((ComboBoxItem)cmbCheckOutHour.SelectedItem).Content.ToString());
                var checkOutMinute = int.Parse(((ComboBoxItem)cmbCheckOutMinute.SelectedItem).Content.ToString());
                var checkOutTime = $"{checkOutHour:D2}:{checkOutMinute:D2}";
                
                // التحقق من وجود سجل حضور للموظف في نفس اليوم
                var existingRecord = FindAttendanceRecord(selectedEmployee.EmployeeNumber);
                if (existingRecord != null)
                {
                    // تحديث سجل الحضور الموجود
                    existingRecord.CheckOutTime = checkOutTime;
                    existingRecord.Notes = txtNotes.Text;
                    
                    MessageBox.Show("تم تحديث وقت الانصراف بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // إنشاء سجل حضور جديد
                    var newRecord = new AttendanceRecord
                    {
                        EmployeeNumber = selectedEmployee.EmployeeNumber,
                        EmployeeName = selectedEmployee.Name,
                        Date = dpDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                        CheckInTime = "",
                        CheckOutTime = checkOutTime,
                        Notes = txtNotes.Text
                    };
                    
                    // في التطبيق الحقيقي، سنقوم بحفظ السجل في قاعدة البيانات
                    // await _databaseService.AddAttendanceRecordAsync(newRecord);
                    
                    // إضافة السجل إلى القائمة
                    _attendanceRecords.Add(newRecord);
                    
                    MessageBox.Show("تم تسجيل الانصراف بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تسجيل الانصراف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// حدث النقر على زر تسجيل الحضور والانصراف
        /// </summary>
        private async void btnBoth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // التحقق من اختيار موظف
                if (cmbEmployee.SelectedItem == null)
                {
                    MessageBox.Show("الرجاء اختيار موظف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // الحصول على الموظف المحدد
                var selectedEmployeeItem = (ComboBoxItem)cmbEmployee.SelectedItem;
                var selectedEmployeeId = (int)selectedEmployeeItem.Tag;
                var selectedEmployee = _allEmployees.Find(emp => emp.Id == selectedEmployeeId);
                
                // الحصول على وقت الحضور والانصراف
                var checkInHour = int.Parse(((ComboBoxItem)cmbCheckInHour.SelectedItem).Content.ToString());
                var checkInMinute = int.Parse(((ComboBoxItem)cmbCheckInMinute.SelectedItem).Content.ToString());
                var checkInTime = $"{checkInHour:D2}:{checkInMinute:D2}";
                
                var checkOutHour = int.Parse(((ComboBoxItem)cmbCheckOutHour.SelectedItem).Content.ToString());
                var checkOutMinute = int.Parse(((ComboBoxItem)cmbCheckOutMinute.SelectedItem).Content.ToString());
                var checkOutTime = $"{checkOutHour:D2}:{checkOutMinute:D2}";
                
                // التحقق من عدم وجود سجل حضور للموظف في نفس اليوم
                var existingRecord = FindAttendanceRecord(selectedEmployee.EmployeeNumber);
                if (existingRecord != null)
                {
                    // تحديث سجل الحضور الموجود
                    existingRecord.CheckInTime = checkInTime;
                    existingRecord.CheckOutTime = checkOutTime;
                    existingRecord.Notes = txtNotes.Text;
                    
                    MessageBox.Show("تم تحديث وقت الحضور والانصراف بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // إنشاء سجل حضور جديد
                    var newRecord = new AttendanceRecord
                    {
                        EmployeeNumber = selectedEmployee.EmployeeNumber,
                        EmployeeName = selectedEmployee.Name,
                        Date = dpDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                        CheckInTime = checkInTime,
                        CheckOutTime = checkOutTime,
                        Notes = txtNotes.Text
                    };
                    
                    // في التطبيق الحقيقي، سنقوم بحفظ السجل في قاعدة البيانات
                    // await _databaseService.AddAttendanceRecordAsync(newRecord);
                    
                    // إضافة السجل إلى القائمة
                    _attendanceRecords.Add(newRecord);
                    
                    MessageBox.Show("تم تسجيل الحضور والانصراف بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تسجيل الحضور والانصراف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// البحث عن سجل حضور لموظف
        /// </summary>
        private AttendanceRecord FindAttendanceRecord(string employeeNumber)
        {
            foreach (var record in _attendanceRecords)
            {
                if (record.EmployeeNumber == employeeNumber)
                {
                    return record;
                }
            }
            
            return null;
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
    /// نموذج سجل الحضور
    /// </summary>
    public class AttendanceRecord
    {
        /// <summary>
        /// الرقم الوظيفي للموظف
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// اسم الموظف
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// تاريخ الحضور
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// وقت الحضور
        /// </summary>
        public string CheckInTime { get; set; }

        /// <summary>
        /// وقت الانصراف
        /// </summary>
        public string CheckOutTime { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        public string Notes { get; set; }
    }
}
