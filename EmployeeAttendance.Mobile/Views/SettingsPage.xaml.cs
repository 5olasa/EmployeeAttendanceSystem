using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// صفحة الإعدادات
    /// </summary>
    public partial class SettingsPage : ContentPage
    {
        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // تحميل الإعدادات
            if (BindingContext is SettingsViewModel viewModel)
            {
                viewModel.LoadSettingsCommand.Execute(null);
            }
        }
    }
}
