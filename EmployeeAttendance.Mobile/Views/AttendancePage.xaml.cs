using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// صفحة تسجيل الحضور
    /// </summary>
    public partial class AttendancePage : ContentPage
    {
        private AttendanceViewModel _viewModel;

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public AttendancePage()
        {
            InitializeComponent();
            _viewModel = new AttendanceViewModel();
            BindingContext = _viewModel;
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // بدء تشغيل الكاميرا
            _viewModel.StartCameraCommand.Execute(null);
        }

        /// <summary>
        /// يتم استدعاؤها عند اختفاء الصفحة
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // إيقاف تشغيل الكاميرا
            _viewModel.StopCameraCommand.Execute(null);
        }
    }
}
