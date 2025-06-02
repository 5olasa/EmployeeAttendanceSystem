using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// الفئة الأساسية لنماذج العرض
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _title = string.Empty;

        /// <summary>
        /// حدث تغيير الخاصية
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// مؤشر الانشغال
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                    OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        /// <summary>
        /// مؤشر عدم الانشغال
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// عنوان الصفحة
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// تعيين قيمة الخاصية وإثارة حدث تغيير الخاصية
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// إثارة حدث تغيير الخاصية
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// تنفيذ مهمة مع عرض مؤشر الانشغال
        /// </summary>
        public async Task RunBusyAsync(Func<Task> action)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
