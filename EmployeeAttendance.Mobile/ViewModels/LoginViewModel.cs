using EmployeeAttendance.Mobile.Models;
using EmployeeAttendance.Mobile.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeAttendance.Mobile.ViewModels
{
    /// <summary>
    /// نموذج عرض صفحة تسجيل الدخول
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly ISettingsService _settingsService;

        private string _employeeNumber;
        private string _password;
        private bool _rememberMe;
        private string _errorMessage;
        private bool _isError;

        /// <summary>
        /// أمر تسجيل الدخول
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// رقم الموظف
        /// </summary>
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set => SetProperty(ref _employeeNumber, value);
        }

        /// <summary>
        /// كلمة المرور
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// تذكرني
        /// </summary>
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        /// <summary>
        /// رسالة الخطأ
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                IsError = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// مؤشر وجود خطأ
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        /// <summary>
        /// المنشئ الافتراضي
        /// </summary>
        public LoginViewModel(IApiService apiService, ISettingsService settingsService)
        {
            _apiService = apiService;
            _settingsService = settingsService;

            Title = "تسجيل الدخول";
            LoginCommand = new Command(async () => await LoginAsync());

            // تعيين قيمة "تذكرني" من الإعدادات
            RememberMe = _settingsService.GetRememberLoginEnabled();
        }

        /// <summary>
        /// تحميل بيانات تسجيل الدخول المحفوظة
        /// </summary>
        public void LoadSavedCredentials()
        {
            try
            {
                // تحميل بيانات تسجيل الدخول المحفوظة
                var (savedEmployeeNumber, savedPassword) = _settingsService.GetSavedLoginCredentials();

                if (!string.IsNullOrEmpty(savedEmployeeNumber) && !string.IsNullOrEmpty(savedPassword))
                {
                    EmployeeNumber = savedEmployeeNumber;
                    Password = savedPassword;
                    RememberMe = true;
                }
                else
                {
                    // بيانات وهمية للعرض (في التطبيق الحقيقي، سنقوم بإزالة هذا الجزء)
                    EmployeeNumber = "EMP001";
                    Password = "123456";
                    RememberMe = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"حدث خطأ أثناء تحميل البيانات المحفوظة: {ex.Message}";
            }
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(EmployeeNumber) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "الرجاء إدخال رقم الموظف وكلمة المرور";
                return;
            }

            ErrorMessage = string.Empty;

            await RunBusyAsync(async () =>
            {
                try
                {
                    // محاولة تسجيل الدخول عبر API
                    User user = await _apiService.LoginAsync(EmployeeNumber, Password);

                    if (user != null && !string.IsNullOrEmpty(user.Token))
                    {
                        // حفظ بيانات تسجيل الدخول إذا تم اختيار "تذكرني"
                        _settingsService.SetRememberLoginEnabled(RememberMe);

                        if (RememberMe)
                        {
                            _settingsService.SaveLoginCredentials(EmployeeNumber, Password);
                        }
                        else
                        {
                            _settingsService.ClearSavedLoginCredentials();
                        }

                        // حفظ بيانات الجلسة
                        _settingsService.SaveSessionData(user.Token, user.Id);

                        // الانتقال إلى الصفحة الرئيسية
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        ErrorMessage = "رقم الموظف أو كلمة المرور غير صحيحة";
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    if (httpEx.Message.Contains("401") || httpEx.Message.Contains("Unauthorized"))
                    {
                        ErrorMessage = "رقم الموظف أو كلمة المرور غير صحيحة";
                    }
                    else if (httpEx.Message.Contains("timeout") || httpEx.Message.Contains("connection"))
                    {
                        ErrorMessage = "لا يمكن الاتصال بالخادم. تحقق من اتصال الإنترنت";
                    }
                    else
                    {
                        ErrorMessage = "حدث خطأ في الاتصال بالخادم";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"حدث خطأ أثناء تسجيل الدخول: {ex.Message}";
                }
            });
        }
    }
}
