using EmployeeAttendance.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EmployeeAttendance.Desktop.Views
{
    /// <summary>
    /// نافذة اختيار موظف
    /// </summary>
    public partial class SelectEmployeeWindow : Window
    {
        private List<Employee> _allEmployees;
        private List<Employee> _filteredEmployees;

        /// <summary>
        /// الموظف المحدد
        /// </summary>
        public Employee SelectedEmployee { get; private set; }

        /// <summary>
        /// إنشاء نافذة اختيار موظف
        /// </summary>
        public SelectEmployeeWindow(List<Employee> employees)
        {
            InitializeComponent();
            
            _allEmployees = employees;
            _filteredEmployees = new List<Employee>(_allEmployees);
            
            // عرض قائمة الموظفين
            lvEmployees.ItemsSource = _filteredEmployees;
        }

        /// <summary>
        /// حدث تغيير النص في مربع البحث
        /// </summary>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchText))
            {
                // إذا كان مربع البحث فارغًا، عرض جميع الموظفين
                _filteredEmployees = new List<Employee>(_allEmployees);
            }
            else
            {
                // تصفية الموظفين حسب النص المدخل
                _filteredEmployees = _allEmployees.Where(emp =>
                    emp.Name.ToLower().Contains(searchText) ||
                    emp.EmployeeNumber.ToLower().Contains(searchText)
                ).ToList();
            }
            
            // تحديث قائمة الموظفين
            lvEmployees.ItemsSource = _filteredEmployees;
        }

        /// <summary>
        /// حدث تغيير التحديد في قائمة الموظفين
        /// </summary>
        private void lvEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // تمكين زر الاختيار إذا تم تحديد موظف
            btnSelect.IsEnabled = lvEmployees.SelectedItem != null;
        }

        /// <summary>
        /// حدث النقر على زر اختيار
        /// </summary>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvEmployees.SelectedItem is Employee selectedEmployee)
            {
                SelectedEmployee = selectedEmployee;
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// حدث النقر على زر إلغاء
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
