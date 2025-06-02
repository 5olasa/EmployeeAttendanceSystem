using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// نموذج عرض صفحة الملف الشخصي
    /// </summary>
    public class ProfileViewModel : BaseViewModel
    {
        private ImageSource _profileImage;
        private string _employeeName;
        private string _employeeNumber;
        private string _employeePosition;
        private string _email;
        private string _phone;
        private string _hireDate;
        private string _department;
        private string _manager;
        private string _attendanceRate;
        private string _attendanceDays;
        private string _absenceDays;
        private string _lateDays;
        private string _remainingVacationDays;

        /// <summary>
        /// أمر تحميل الملف الشخصي
        /// </summary>
        public ICommand LoadProfileCommand { get; }

        /// <summary>
        /// أمر تغيير كلمة المرور
        /// </summary>
        public ICommand ChangePasswordCommand { get; }

        /// <summary>
        /// أمر تحديث الصورة الشخصية
        /// </summary>
        public ICommand UpdateProfileImageCommand { get; }

        /// <summary>
        /// صورة الملف الشخصي
        /// </summary>
        public ImageSource ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        /// <summary>
        /// اسم الموظف
        /// </summary>
        public string EmployeeName
        {
            get => _employeeName;
            set => SetProperty(ref _employeeName, value);
        }

        /// <summary>
        /// رقم الموظف
        /// </summary>
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set => SetProperty(ref _employeeNumber, value);
        }

        /// <summary>
        /// المسمى الوظيفي
        /// </summary>
        public string EmployeePosition
        {
            get => _employeePosition;
            set => SetProperty(ref _employeePosition, value);
        }

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        /// <summary>
        /// تاريخ التوظيف
        /// </summary>
        public string HireDate
        {
            get => _hireDate;
            set => SetProperty(ref _hireDate, value);
        }

        /// <summary>
        /// القسم
        /// </summary>
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }

        /// <summary>
        /// المدير المباشر
        /// </summary>
        public string Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }

        /// <summary>
        /// نسبة الحضور
        /// </summary>
        public string AttendanceRate
        {
            get => _attendanceRate;
            set => SetProperty(ref _attendanceRate, value);
        }

        /// <summary>
        /// أيام الحضور
        /// </summary>
        public string AttendanceDays
        {
            get => _attendanceDays;
            set => SetProperty(ref _attendanceDays, value);
        }

        /// <summary>
        /// أيام الغياب
        /// </summary>
        public string AbsenceDays
        {
            get => _absenceDays;
            set => SetProperty(ref _absenceDays, value);
        }

        /// <summary>
        /// أيام التأخير
        /// </summary>
        public string LateDays
        {
            get => _lateDays;
            set => SetProperty(ref _lateDays, value);
        }

        /// <summary>
        /// أيام الإجازة المتبقية
        /// </summary>
        public string RemainingVacationDays
        {
            get => _remainingVacationDays;
            set => SetProperty(ref _remainingVacationDays, value);
        }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public ProfileViewModel()
        {
            Title = "الملف الشخصي";
            LoadProfileCommand = new Command(async () => await LoadProfileAsync());
            ChangePasswordCommand = new Command(async () => await ChangePasswordAsync());
            UpdateProfileImageCommand = new Command(async () => await UpdateProfileImageAsync());
        }

        /// <summary>
        /// تحميل الملف الشخصي
        /// </summary>
        private async Task LoadProfileAsync()
        {
            if (IsBusy)
                return;

            await RunBusyAsync(async () =>
            {
                try
                {
                    // في التطبيق الحقيقي، سنقوم بتحميل البيانات من الخادم
                    // var employeeService = new EmployeeService();
                    // var employee = await employeeService.GetEmployeeAsync(Preferences.Get("UserId", string.Empty));
                    // var attendanceService = new AttendanceService();
                    // var attendanceStats = await attendanceService.GetAttendanceStatsAsync(Preferences.Get("UserId", string.Empty));
                    
                    // محاكاة تأخير الاتصال بالخادم
                    await Task.Delay(1000);
                    
                    // تعيين بيانات الموظف (بيانات وهمية للعرض)
                    ProfileImage = "profile_placeholder.png";
                    EmployeeName = "أحمد محمد";
                    EmployeeNumber = "EMP001";
                    EmployeePosition = "مطور برمجيات";
                    Email = "ahmed.mohamed@example.com";
                    Phone = "0123456789";
                    HireDate = new DateTime(2020, 1, 1).ToString("dd MMMM yyyy", new CultureInfo("ar-SA"));
                    Department = "تكنولوجيا المعلومات";
                    Manager = "محمد علي";
                    
                    // تعيين إحصائيات الحضور (بيانات وهمية للعرض)
                    AttendanceRate = "95%";
                    AttendanceDays = "20 يوم";
                    AbsenceDays = "1 يوم";
                    LateDays = "2 يوم";
                    RemainingVacationDays = "15 يوم";
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("خطأ", $"حدث خطأ أثناء تحميل البيانات: {ex.Message}", "موافق");
                }
            });
        }

        /// <summary>
        /// تغيير كلمة المرور
        /// </summary>
        private async Task ChangePasswordAsync()
        {
            // عرض نافذة تغيير كلمة المرور
            string currentPassword = await Shell.Current.DisplayPromptAsync("تغيير كلمة المرور", "أدخل كلمة المرور الحالية", "متابعة", "إلغاء", "كلمة المرور الحالية", -1, Keyboard.Default, "");
            
            if (string.IsNullOrEmpty(currentPassword))
                return;
                
            string newPassword = await Shell.Current.DisplayPromptAsync("تغيير كلمة المرور", "أدخل كلمة المرور الجديدة", "متابعة", "إلغاء", "كلمة المرور الجديدة", -1, Keyboard.Default, "");
            
            if (string.IsNullOrEmpty(newPassword))
                return;
                
            string confirmPassword = await Shell.Current.DisplayPromptAsync("تغيير كلمة المرور", "تأكيد كلمة المرور الجديدة", "تغيير", "إلغاء", "تأكيد كلمة المرور", -1, Keyboard.Default, "");
            
            if (string.IsNullOrEmpty(confirmPassword))
                return;
                
            if (newPassword != confirmPassword)
            {
                await Shell.Current.DisplayAlert("خطأ", "كلمة المرور الجديدة وتأكيدها غير متطابقين", "موافق");
                return;
            }
            
            // في التطبيق الحقيقي، سنقوم بتغيير كلمة المرور في قاعدة البيانات
            // var authService = new AuthService();
            // var result = await authService.ChangePasswordAsync(Preferences.Get("UserId", string.Empty), currentPassword, newPassword);
            
            // محاكاة تأخير تغيير كلمة المرور
            await Task.Delay(1000);
            
            // عرض رسالة نجاح
            await Shell.Current.DisplayAlert("نجاح", "تم تغيير كلمة المرور بنجاح", "موافق");
        }

        /// <summary>
        /// تحديث الصورة الشخصية
        /// </summary>
        private async Task UpdateProfileImageAsync()
        {
            // عرض خيارات تحديث الصورة
            string action = await Shell.Current.DisplayActionSheet("تحديث الصورة الشخصية", "إلغاء", null, "التقاط صورة", "اختيار من المعرض");
            
            if (action == "إلغاء")
                return;
                
            // في التطبيق الحقيقي، سنقوم بالتقاط صورة أو اختيارها من المعرض
            // var mediaService = new MediaService();
            // var image = action == "التقاط صورة"
            //     ? await mediaService.TakePhotoAsync()
            //     : await mediaService.PickPhotoAsync();
            
            // if (image == null)
            //     return;
                
            // var employeeService = new EmployeeService();
            // await employeeService.UpdateProfileImageAsync(Preferences.Get("UserId", string.Empty), image);
            
            // محاكاة تأخير تحديث الصورة
            await Task.Delay(1000);
            
            // عرض رسالة نجاح
            await Shell.Current.DisplayAlert("نجاح", "تم تحديث الصورة الشخصية بنجاح", "موافق");
        }
    }
}
