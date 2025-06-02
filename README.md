# نظام حضور الموظفين (Employee Attendance System)

نظام متكامل لإدارة حضور الموظفين باستخدام تقنية التعرف على الوجه. يتكون النظام من تطبيق سطح مكتب لنظام Windows، تطبيق محمول لنظام Android، وخادم Web API.

## 🎉 **الجديد: تم ربط النظام بقاعدة البيانات!**

تم تطوير وربط النظام بقاعدة بيانات SQL Server حقيقية مع Web API متكامل.

## المميزات

- ✅ تسجيل حضور وانصراف الموظفين باستخدام تقنية التعرف على الوجه
- ✅ إدارة بيانات الموظفين
- ✅ إدارة الرواتب
- ✅ إنشاء التقارير
- ✅ تسجيل الحضور يدويًا
- ✅ واجهة مستخدم باللغة العربية
- ✅ مزامنة البيانات بين التطبيقات
- 🆕 **Web API متكامل مع JWT Authentication**
- 🆕 **قاعدة بيانات SQL Server حقيقية**
- 🆕 **معالجة أخطاء محسنة**

## متطلبات النظام

### خادم Web API
- نظام التشغيل Windows 10 أو أحدث / Linux / macOS
- .NET 9.0 SDK
- SQL Server أو SQL Server Express
- 4 GB RAM (الحد الأدنى)

### تطبيق سطح المكتب (Windows)
- نظام التشغيل Windows 10 أو أحدث
- .NET 9.0 SDK
- كاميرا ويب متصلة بالجهاز (للتعرف على الوجه)

### تطبيق الهاتف المحمول (Android)
- نظام التشغيل Android 5.0 (API Level 21) أو أحدث
- كاميرا أمامية (للتعرف على الوجه)
- اتصال بالإنترنت للوصول إلى Web API

## 🚀 التشغيل السريع

### 1. إعداد قاعدة البيانات
```bash
# تشغيل سكريبت الإعداد التلقائي
setup-database.bat
```

### 2. تشغيل خادم Web API
```bash
# تشغيل الخادم
start-server.bat
```
الخادم سيعمل على: `http://localhost:5000`

### 3. تشغيل تطبيق الموبايل
```bash
cd EmployeeAttendance.Mobile
dotnet build -f net9.0-android
```

### 4. تسجيل الدخول
- **رقم الموظف:** `EMP001`
- **كلمة المرور:** `123456`

## متطلبات التطوير

### خادم Web API
- Visual Studio 2022 أو أحدث
- .NET 9.0 SDK
- SQL Server Management Studio (اختياري)

### تطبيق سطح المكتب (Windows)
- Visual Studio 2022 أو أحدث
- .NET 9.0 SDK
- حزم NuGet المطلوبة (يتم تثبيتها تلقائيًا عند استعادة الحزم)

### تطبيق الهاتف المحمول (Android)
- Visual Studio 2022 أو أحدث
- .NET MAUI workload
- Android SDK
- Java JDK (وليس JRE)

## إعداد بيئة التطوير

### تثبيت .NET SDK
1. قم بتنزيل وتثبيت [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. تحقق من التثبيت بتنفيذ الأمر التالي في موجه الأوامر:
   ```
   dotnet --version
   ```

### تثبيت MAUI Workload
1. قم بتنفيذ الأمر التالي في موجه الأوامر:
   ```
   dotnet workload install maui
   ```

### تثبيت Android SDK
1. قم بتنزيل وتثبيت [Android Studio](https://developer.android.com/studio)
2. أثناء التثبيت، تأكد من اختيار تثبيت Android SDK
3. بعد التثبيت، افتح Android Studio وانتقل إلى SDK Manager لتثبيت الإصدارات المطلوبة من Android SDK
4. قم بتثبيت الإصدارات التالية:
   - Android SDK Platform 33 (Android 13.0)
   - Android SDK Build-Tools 33.0.0
   - Android SDK Command-line Tools
   - Android SDK Platform-Tools
   - Android Emulator

### تثبيت Java JDK
1. قم بتنزيل وتثبيت [Java JDK](https://www.oracle.com/java/technologies/downloads/) (الإصدار 11 أو أحدث)
2. قم بإعداد متغير البيئة JAVA_HOME ليشير إلى مجلد JDK

### تكوين مسارات Android SDK وJava JDK
1. افتح ملف `Directory.Build.props` في مجلد المشروع
2. قم بتعديل المسارات لتشير إلى مجلدات Android SDK وJava JDK على جهازك:
   ```xml
   <AndroidSdkDirectory>C:\path\to\android-sdk</AndroidSdkDirectory>
   <JavaSdkDirectory>C:\path\to\jdk</JavaSdkDirectory>
   ```

## بناء المشروع

### تطبيق سطح المكتب (Windows)
1. افتح موجه الأوامر في مجلد المشروع
2. قم بتنفيذ الأمر التالي:
   ```
   dotnet build EmployeeAttendance.Desktop
   ```

### تطبيق الهاتف المحمول (Android)
1. افتح موجه الأوامر في مجلد المشروع
2. قم بتنفيذ الأمر التالي:
   ```
   dotnet build EmployeeAttendance.Mobile -f net9.0-android /p:AndroidSdkDirectory="C:\path\to\android-sdk" /p:JavaSdkDirectory="C:\path\to\jdk"
   ```

   قم بتغيير المسارات لتشير إلى مجلدات Android SDK وJava JDK على جهازك.

## إنشاء ملف APK للتطبيق المحمول

1. افتح موجه الأوامر في مجلد المشروع
2. قم بتنفيذ السكريبت التالي:
   ```
   .\build-apk.bat
   ```
3. سيتم إنشاء ملف APK في المجلد `EmployeeAttendance.Mobile\bin\Release\apk`

## تشغيل التطبيق المحمول في المحاكي

1. افتح موجه الأوامر في مجلد المشروع
2. قم بتنفيذ السكريبت التالي:
   ```
   .\run-emulator.bat
   ```
3. اتبع التعليمات على الشاشة لاختيار المحاكي وتشغيل التطبيق

## هيكل المشروع

- **EmployeeAttendance.API**: 🆕 خادم Web API (.NET 9)
  - Controllers للمصادقة والموظفين والحضور
  - JWT Authentication
  - Swagger Documentation
- **EmployeeAttendance.Desktop**: تطبيق سطح المكتب لنظام Windows
- **EmployeeAttendance.Mobile**: تطبيق الهاتف المحمول لنظام Android
  - 🆕 مربوط بـ Web API
  - 🆕 معالجة أخطاء محسنة
- **EmployeeAttendance.Shared**: مكتبة مشتركة تحتوي على النماذج والخدمات المشتركة
- **Database**: 🆕 سكريبتات قاعدة البيانات
  - إنشاء الجداول والإجراءات المخزنة
  - بيانات تجريبية

## 📡 API Documentation

### Authentication Endpoints
- `POST /api/auth/login` - تسجيل الدخول
- `POST /api/auth/changepassword` - تغيير كلمة المرور

### Employee Endpoints
- `GET /api/employees` - جميع الموظفين
- `GET /api/employees/{id}` - موظف محدد
- `POST /api/employees` - إضافة موظف جديد
- `PUT /api/employees/{id}` - تحديث بيانات موظف
- `POST /api/employees/{id}/image` - تحديث صورة الموظف

### Attendance Endpoints
- `POST /api/attendance/checkin` - تسجيل الحضور
- `POST /api/attendance/checkout` - تسجيل الانصراف
- `GET /api/attendance/{employeeId}` - سجلات الحضور
- `GET /api/attendance/stats/{employeeId}` - إحصائيات الحضور

### Health Check
- `GET /api/health` - فحص حالة الخادم

**Swagger UI متوفر على:** `http://localhost:5000/swagger`

## 🔧 الإعدادات

### قاعدة البيانات
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeAttendance;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### JWT Settings
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "EmployeeAttendanceAPI",
    "Audience": "EmployeeAttendanceApp"
  }
}
```

## المساهمة في المشروع

1. قم بعمل Fork للمشروع
2. قم بإنشاء فرع جديد للميزة التي تريد إضافتها
3. قم بإجراء التغييرات اللازمة
4. قم بإرسال طلب سحب (Pull Request)

## 🆘 استكشاف الأخطاء

### مشاكل قاعدة البيانات
**المشكلة:** خطأ في الاتصال بقاعدة البيانات
```
Cannot connect to SQL Server
```
**الحل:**
1. تأكد من تشغيل SQL Server
2. تحقق من connection string في `appsettings.json`
3. تأكد من وجود قاعدة البيانات `EmployeeAttendance`

### مشاكل Web API
**المشكلة:** لا يمكن الوصول إلى الخادم
```
Connection refused on localhost:5000
```
**الحل:**
1. تأكد من تشغيل Web API باستخدام `start-server.bat`
2. تحقق من عدم استخدام المنفذ 5000 من تطبيق آخر
3. جرب استخدام المنفذ 5001 (HTTPS)

### مشاكل تطبيق الموبايل
**المشكلة:** خطأ في تسجيل الدخول
```
Login failed: Unauthorized
```
**الحل:**
1. استخدم البيانات الصحيحة: `EMP001` / `123456`
2. تأكد من تشغيل Web API
3. تحقق من عنوان الخادم في إعدادات التطبيق

**المشكلة:** خطأ في بناء التطبيق
```
MAUI workload not found
```
**الحل:**
```bash
dotnet workload install maui
```

## 📚 موارد إضافية

- [دليل ربط قاعدة البيانات](DATABASE_INTEGRATION_GUIDE.md)
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/sql-server/)

## الترخيص

هذا المشروع مرخص تحت رخصة MIT. انظر ملف LICENSE للمزيد من التفاصيل.
