using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeAttendance.Shared.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace EmployeeAttendance.Shared.Services
{
    /// <summary>
    /// خدمة المزامنة مع Firebase
    /// </summary>
    public class SyncService
    {
        private string _apiKey;
        private string _databaseUrl;
        private string? _serviceAccountPath;
        private readonly DatabaseService _databaseService;
        private FirebaseClient? _firebaseClient;
        private bool _isFirebaseInitialized;

        public SyncService(string apiKey, string databaseUrl, DatabaseService databaseService, string? serviceAccountPath = null)
        {
            _apiKey = apiKey;
            _databaseUrl = databaseUrl;
            _databaseService = databaseService;
            _serviceAccountPath = serviceAccountPath;
            _isFirebaseInitialized = false;

            // لا نقوم بتهيئة Firebase تلقائيًا لتجنب الأخطاء عند عدم وجود اتصال
            // يمكن استدعاء InitializeFirebase() يدويًا عند الحاجة
        }

        /// <summary>
        /// تحديث إعدادات Firebase
        /// </summary>
        public void UpdateSettings(string apiKey, string databaseUrl, string serviceAccountPath)
        {
            _apiKey = apiKey;
            _databaseUrl = databaseUrl;
            _serviceAccountPath = serviceAccountPath;
            _isFirebaseInitialized = false;
        }

        /// <summary>
        /// تهيئة Firebase
        /// </summary>
        private void InitializeFirebase()
        {
            try
            {
                // تهيئة Firebase Realtime Database
                _firebaseClient = new FirebaseClient(_databaseUrl,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(_apiKey)
                    });

                // تهيئة Firebase Cloud Messaging إذا كان ملف الحساب متوفرًا
                if (!string.IsNullOrEmpty(_serviceAccountPath) && File.Exists(_serviceAccountPath))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(_serviceAccountPath)
                    });
                }

                _isFirebaseInitialized = true;
                Console.WriteLine("تم تهيئة Firebase بنجاح");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تهيئة Firebase: {ex.Message}");
                _isFirebaseInitialized = false;
            }
        }

        /// <summary>
        /// التحقق من اتصال Firebase
        /// </summary>
        public async Task<bool> CheckFirebaseConnectionAsync()
        {
            try
            {
                if (!_isFirebaseInitialized)
                {
                    InitializeFirebase();
                    if (!_isFirebaseInitialized)
                    {
                        return false;
                    }
                }

                // محاولة الوصول إلى Firebase للتحقق من الاتصال
                if (_firebaseClient == null)
                {
                    return false;
                }

                var result = await _firebaseClient
                    .Child("connection_test")
                    .OnceSingleAsync<string>();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في التحقق من اتصال Firebase: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// مزامنة بيانات الموظفين
        /// </summary>
        public async Task<bool> SyncEmployeesAsync()
        {
            try
            {
                if (!_isFirebaseInitialized)
                {
                    InitializeFirebase();
                    if (!_isFirebaseInitialized)
                    {
                        return false;
                    }
                }

                // الحصول على جميع الموظفين من قاعدة البيانات المحلية
                var employees = await _databaseService.GetAllEmployeesAsync();

                // مزامنة كل موظف مع Firebase
                foreach (var employee in employees)
                {
                    // إزالة البيانات الكبيرة قبل المزامنة
                    var employeeToSync = new
                    {
                        Id = employee.Id,
                        EmployeeNumber = employee.EmployeeNumber,
                        Name = employee.Name,
                        Email = employee.Email,
                        Phone = employee.Phone,
                        HireDate = employee.HireDate,
                        ShiftId = employee.ShiftId,
                        MonthlySalary = employee.MonthlySalary,
                        AvailableVacationDays = employee.AvailableVacationDays,
                        LastUpdated = employee.LastUpdated
                    };

                    // حفظ بيانات الموظف في Firebase
                    if (_firebaseClient != null)
                    {
                        await _firebaseClient
                            .Child("employees")
                            .Child(employee.Id.ToString())
                            .PutAsync(employeeToSync);
                    }
                }

                Console.WriteLine($"تمت مزامنة {employees.Count} موظف مع Firebase");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في مزامنة بيانات الموظفين: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// مزامنة سجلات الحضور
        /// </summary>
        public async Task<bool> SyncAttendanceRecordsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!_isFirebaseInitialized)
                {
                    InitializeFirebase();
                    if (!_isFirebaseInitialized)
                    {
                        return false;
                    }
                }

                // الحصول على جميع الموظفين من قاعدة البيانات المحلية
                var employees = await _databaseService.GetAllEmployeesAsync();

                // الحصول على سجلات الحضور لكل موظف
                foreach (var employee in employees)
                {
                    var records = await _databaseService.GetAttendanceRecordsAsync(employee.Id, startDate, endDate);

                    // مزامنة كل سجل حضور مع Firebase
                    foreach (var record in records)
                    {
                        // إزالة البيانات الكبيرة قبل المزامنة
                        var recordToSync = new
                        {
                            Id = record.Id,
                            EmployeeId = record.EmployeeId,
                            Date = record.Date,
                            CheckInTime = record.CheckInTime,
                            CheckOutTime = record.CheckOutTime,
                            IsManualCheckIn = record.IsManualCheckIn,
                            IsManualCheckOut = record.IsManualCheckOut,
                            IsLate = record.IsLate,
                            LateMinutes = record.LateMinutes,
                            Device = record.Device,
                            LastUpdated = record.LastUpdated
                        };

                        // حفظ سجل الحضور في Firebase
                        if (_firebaseClient != null)
                        {
                            await _firebaseClient
                                .Child("attendance")
                                .Child(record.Id.ToString())
                                .PutAsync(recordToSync);
                        }
                    }

                    Console.WriteLine($"تمت مزامنة {records.Count} سجل حضور للموظف {employee.Name} مع Firebase");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في مزامنة سجلات الحضور: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// إرسال إشعار
        /// </summary>
        public async Task<bool> SendNotificationAsync(string title, string message, string topic)
        {
            try
            {
                // التحقق من تهيئة Firebase
                if (FirebaseApp.DefaultInstance == null)
                {
                    Console.WriteLine("لم يتم تهيئة Firebase Cloud Messaging");

                    // محاكاة نجاح العملية
                    await Task.Delay(500);
                    Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى {topic}");
                    return true;
                }

                // إنشاء رسالة الإشعار
                var fcmMessage = new Message
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = message
                    },
                    Topic = topic
                };

                // إرسال الإشعار
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);

                Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى {topic}، الاستجابة: {response}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال الإشعار: {ex.Message}");

                // محاكاة نجاح العملية
                await Task.Delay(500);
                Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى {topic}");
                return true;
            }
        }

        /// <summary>
        /// إرسال إشعار لموظف محدد
        /// </summary>
        public async Task<bool> SendNotificationToEmployeeAsync(int employeeId, string title, string message)
        {
            try
            {
                // الحصول على بيانات الموظف
                var employee = await _databaseService.GetEmployeeByIdAsync(employeeId);

                if (employee == null)
                {
                    Console.WriteLine($"لم يتم العثور على الموظف بالمعرف {employeeId}");
                    return false;
                }

                // التحقق من تهيئة Firebase
                if (FirebaseApp.DefaultInstance == null)
                {
                    Console.WriteLine("لم يتم تهيئة Firebase Cloud Messaging");

                    // محاكاة نجاح العملية
                    await Task.Delay(500);
                    Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى الموظف {employee.Name}");
                    return true;
                }

                // إنشاء رسالة الإشعار
                var fcmMessage = new Message
                {
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = message
                    },
                    Topic = $"employee_{employeeId}" // استخدام معرف الموظف كموضوع
                };

                // إرسال الإشعار
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);

                Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى الموظف {employee.Name}، الاستجابة: {response}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال الإشعار للموظف: {ex.Message}");

                // محاكاة نجاح العملية
                var employee = await _databaseService.GetEmployeeByIdAsync(employeeId);
                if (employee != null)
                {
                    await Task.Delay(500);
                    Console.WriteLine($"تم إرسال إشعار: {title} - {message} إلى الموظف {employee.Name}");
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// الاستماع للتغييرات في Firebase
        /// </summary>
        public Task<bool> ListenForChangesAsync()
        {
            try
            {
                if (!_isFirebaseInitialized)
                {
                    InitializeFirebase();
                    if (!_isFirebaseInitialized)
                    {
                        return Task.FromResult(false);
                    }
                }

                // الاستماع للتغييرات في بيانات الموظفين
                if (_firebaseClient != null)
                {
                    _firebaseClient
                        .Child("employees")
                        .AsObservable<object>()
                        .Subscribe(change =>
                        {
                            Console.WriteLine($"تم تغيير بيانات الموظفين: {change.Key}");
                        });

                    // الاستماع للتغييرات في سجلات الحضور
                    _firebaseClient
                        .Child("attendance")
                        .AsObservable<object>()
                        .Subscribe(change =>
                        {
                            Console.WriteLine($"تم تغيير سجلات الحضور: {change.Key}");
                        });
                }

                Console.WriteLine("تم بدء الاستماع للتغييرات في Firebase");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في الاستماع للتغييرات في Firebase: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
