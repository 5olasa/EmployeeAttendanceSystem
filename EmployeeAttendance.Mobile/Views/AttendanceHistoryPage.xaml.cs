using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// صفحة سجل الحضور
    /// </summary>
    public partial class AttendanceHistoryPage : ContentPage
    {
        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public AttendanceHistoryPage()
        {
            InitializeComponent();
            BindingContext = new AttendanceHistoryViewModel();
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // تحديث بيانات الصفحة
            if (BindingContext is AttendanceHistoryViewModel viewModel)
            {
                viewModel.LoadDataCommand.Execute(null);
            }
        }
    }
}
