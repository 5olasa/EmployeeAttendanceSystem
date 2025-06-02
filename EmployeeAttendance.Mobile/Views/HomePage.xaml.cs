using EmployeeAttendance.Mobile.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace EmployeeAttendance.Mobile.Views
{
    /// <summary>
    /// الصفحة الرئيسية
    /// </summary>
    public partial class HomePage : ContentPage
    {
        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }

        /// <summary>
        /// يتم استدعاؤها عند ظهور الصفحة
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // تحديث بيانات الصفحة
            if (BindingContext is HomeViewModel viewModel)
            {
                viewModel.LoadDataCommand.Execute(null);
            }
        }
    }
}
