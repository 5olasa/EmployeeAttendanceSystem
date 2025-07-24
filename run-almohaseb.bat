@echo off
chcp 65001 >nul
echo.
echo ========================================
echo    المحاسب العربي - الإصدار المصري
echo    Al-Mohaseb Al-Araby - Egyptian Edition
echo ========================================
echo.

REM التحقق من وجود .NET SDK
echo [1/4] التحقق من .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ خطأ: .NET SDK غير مثبت
    echo.
    echo يرجى تحميل وتثبيت .NET 8.0 SDK من:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)
echo ✅ .NET SDK متوفر

REM التحقق من وجود ملف الحل
echo [2/4] التحقق من ملفات المشروع...
if not exist "AlMohasebAlAraby.sln" (
    echo ❌ خطأ: ملف الحل غير موجود
    echo تأكد من أنك في المجلد الصحيح للمشروع
    pause
    exit /b 1
)
echo ✅ ملفات المشروع موجودة

REM بناء المشروع
echo [3/4] بناء المشروع...
dotnet build AlMohasebAlAraby.sln --configuration Release --verbosity quiet
if %errorlevel% neq 0 (
    echo ❌ خطأ في بناء المشروع
    echo.
    echo تفاصيل الخطأ:
    dotnet build AlMohasebAlAraby.sln --configuration Release
    pause
    exit /b 1
)
echo ✅ تم بناء المشروع بنجاح

REM إنشاء المجلدات المطلوبة
echo [4/4] إعداد البيئة...
if not exist "Data" mkdir Data
if not exist "Data\Database" mkdir Data\Database
if not exist "Data\Reports" mkdir Data\Reports
if not exist "Data\Exports" mkdir Data\Exports
if not exist "Data\Backups" mkdir Data\Backups
if not exist "Data\Logs" mkdir Data\Logs
echo ✅ تم إعداد البيئة

echo.
echo 🚀 تشغيل المحاسب العربي...
echo.
echo بيانات تسجيل الدخول الافتراضية:
echo اسم المستخدم: admin
echo كلمة المرور: admin123
echo.

REM تشغيل التطبيق
cd AlMohasebAlAraby.Desktop
dotnet run --configuration Release

REM في حالة إغلاق التطبيق
echo.
echo تم إغلاق التطبيق.
pause
