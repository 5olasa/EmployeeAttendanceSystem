using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// صفحة الملف الشخصي
    /// </summary>
    public partial class ProfilePage : ContentPage
    {
        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // تحديث بيانات الصفحة
            if (BindingContext is ProfileViewModel viewModel)
            {
                viewModel.LoadProfileCommand.Execute(null);
            }
        }
    }
}
