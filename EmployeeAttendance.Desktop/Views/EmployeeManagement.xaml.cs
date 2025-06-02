using EmployeeAttendance.Shared.Models;
using EmployeeAttendance.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// واجهة إدارة الموظفين
    /// </summary>
    public partial class EmployeeManagement : Window
    {
        private readonly DatabaseService _databaseService;
        private List<Employee> _allEmployees;
        private CollectionViewSource _employeesViewSource;

        /// <summary>
        /// المستخدم الحالي
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// إنشاء نافذة إدارة الموظفين
        /// </summary>
        public EmployeeManagement(User currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            _databaseService = new DatabaseService(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EmployeeAttendance.mdf;Integrated Security=True");
            _employeesViewSource = new CollectionViewSource();
            dgEmployees.ItemsSource = _employeesViewSource.View;

            // تحميل البيانات عند تحميل النافذة
            Loaded += EmployeeManagement_Loaded;
        }

        /// <summary>
        /// حدث تحميل النافذة
        /// </summary>
        private async void EmployeeManagement_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadEmployeesAsync();
        }

        /// <summary>
        /// تحميل بيانات الموظفين
        /// </summary>
        private async Task LoadEmployeesAsync()
        {
            try
            {
                // عرض مؤشر التحميل
                loadingPanel.Visibility = Visibility.Visible;
                txtNoData.Visibility = Visibility.Collapsed;

                // في التطبيق الحقيقي، سنقوم بالحصول على البيانات من قاعدة البيانات
                // _allEmployees = await _databaseService.GetAllEmployeesAsync();

                // إنشاء بيانات وهمية للموظفين
                await Task.Delay(500); // تأخير وهمي لمحاكاة الاتصال بقاعدة البيانات
                _allEmployees = GenerateDummyEmployees();

                // تحديث مصدر البيانات
                _employeesViewSource.Source = _allEmployees;

                // تحديث حالة العرض
                UpdateDisplayStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل بيانات الموظفين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // إخفاء مؤشر التحميل
                loadingPanel.Visibility = Visibility.Collapsed;
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

            employees.Add(new Employee
            {
                Id = 4,
                EmployeeNumber = "EMP004",
                Name = "فاطمة عبدالله",
                Email = "fatima@example.com",
                Phone = "0503456789",
                HireDate = new DateTime(2018, 7, 5),
                ShiftId = 3,
                MonthlySalary = 15000,
                AvailableVacationDays = 30
            });

            employees.Add(new Employee
            {
                Id = 5,
                EmployeeNumber = "EMP005",
                Name = "عبدالرحمن سعيد",
                Email = "abdulrahman@example.com",
                Phone = "0508765432",
                HireDate = new DateTime(2022, 2, 15),
                ShiftId = 2,
                MonthlySalary = 8000,
                AvailableVacationDays = 15
            });

            return employees;
        }

        /// <summary>
        /// تحديث حالة العرض
        /// </summary>
        private void UpdateDisplayStatus()
        {
            if (_allEmployees == null || _allEmployees.Count == 0)
            {
                txtNoData.Visibility = Visibility.Visible;
                txtStatus.Text = "إجمالي الموظفين: 0";
            }
            else
            {
                txtNoData.Visibility = Visibility.Collapsed;
                txtStatus.Text = $"إجمالي الموظفين: {_allEmployees.Count}";
            }
        }

        /// <summary>
        /// البحث في بيانات الموظفين
        /// </summary>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allEmployees == null) return;

            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                _employeesViewSource.Source = _allEmployees;
            }
            else
            {
                var filteredList = _allEmployees.Where(emp =>
                    emp.Name.ToLower().Contains(searchText) ||
                    emp.EmployeeNumber.ToLower().Contains(searchText) ||
                    emp.Email.ToLower().Contains(searchText) ||
                    emp.Phone.Contains(searchText)
                ).ToList();

                _employeesViewSource.Source = filteredList;
                txtStatus.Text = $"تم العثور على {filteredList.Count} موظف";
            }
        }

        /// <summary>
        /// حدث النقر على زر إضافة موظف
        /// </summary>
        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("سيتم فتح نافذة إضافة موظف جديد", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
            // في التطبيق الحقيقي، سنقوم بفتح نافذة إضافة موظف جديد
            // var employeeRegistration = new EmployeeRegistration(CurrentUser);
            // employeeRegistration.EmployeeAdded += EmployeeRegistration_EmployeeAdded;
            // employeeRegistration.ShowDialog();
        }

        /// <summary>
        /// حدث إضافة موظف جديد
        /// </summary>
        private async void EmployeeRegistration_EmployeeAdded(object sender, EventArgs e)
        {
            await LoadEmployeesAsync();
        }

        /// <summary>
        /// حدث النقر على زر تحديث
        /// </summary>
        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadEmployeesAsync();
        }

        /// <summary>
        /// حدث النقر على زر تعديل
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.Id == employeeId);
                if (employee != null)
                {
                    MessageBox.Show($"سيتم فتح نافذة تعديل بيانات الموظف {employee.Name}", "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
                    // في التطبيق الحقيقي، سنقوم بفتح نافذة تعديل بيانات الموظف
                    // var employeeRegistration = new EmployeeRegistration(CurrentUser, employee);
                    // employeeRegistration.EmployeeAdded += EmployeeRegistration_EmployeeAdded;
                    // employeeRegistration.ShowDialog();
                }
            }
        }

        /// <summary>
        /// حدث النقر على زر المرتبات
        /// </summary>
        private void btnSalary_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.Id == employeeId);
                if (employee != null)
                {
                    var salaryManagement = new SalaryManagement(CurrentUser, employee);
                    salaryManagement.ShowDialog();
                }
            }
        }

        /// <summary>
        /// حدث النقر على زر حذف
        /// </summary>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int employeeId)
            {
                var employee = _allEmployees.FirstOrDefault(emp => emp.Id == employeeId);
                if (employee != null)
                {
                    var result = MessageBox.Show(
                        $"هل أنت متأكد من حذف الموظف {employee.Name}؟\nسيتم حذف جميع البيانات المرتبطة بهذا الموظف.",
                        "تأكيد الحذف",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            loadingPanel.Visibility = Visibility.Visible;
                            // في التطبيق الحقيقي، سنقوم بحذف الموظف من قاعدة البيانات
                            // await _databaseService.DeleteEmployeeAsync(employeeId);

                            // حذف الموظف من القائمة المحلية
                            _allEmployees.Remove(employee);
                            _employeesViewSource.Source = _allEmployees;

                            // تحديث حالة العرض
                            UpdateDisplayStatus();

                            MessageBox.Show("تم حذف الموظف بنجاح", "تم", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"حدث خطأ أثناء حذف الموظف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            loadingPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// حدث تغيير التحديد في جدول الموظفين
        /// </summary>
        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
}
