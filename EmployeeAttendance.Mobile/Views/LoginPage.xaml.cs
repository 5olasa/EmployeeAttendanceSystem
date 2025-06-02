using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// صفحة تسجيل الدخول
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // التحقق من وجود بيانات تسجيل دخول محفوظة
            // إذا كانت موجودة، يتم تعبئة حقول النموذج
            _viewModel.LoadSavedCredentials();
        }
    }
}
