@echo off
chcp 65001 >nul
title المحاسب العربي - الإصدار المصري
color 0A

echo.
echo ╔══════════════════════════════════════════════════════════════╗
echo ║                    المحاسب العربي - الإصدار المصري                    ║
echo ║                Al-Mohaseb Al-Araby - Egyptian Edition        ║
echo ║                                                              ║
echo ║              نظام محاسبة شامل للسوق المصري                      ║
echo ╚══════════════════════════════════════════════════════════════╝
echo.

echo 🔍 فحص المتطلبات...
echo.

REM التحقق من .NET
echo [1/5] التحقق من .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET SDK غير مثبت
    echo.
    echo 📥 يرجى تحميل .NET 8.0 SDK من:
    echo    https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    echo 💡 بعد التثبيت، أعد تشغيل هذا الملف
    echo.
    pause
    exit /b 1
) else (
    for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
    echo ✅ .NET SDK الإصدار: %DOTNET_VERSION%
)

REM التحقق من Git
echo [2/5] التحقق من Git...
git --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠️  Git غير مثبت (اختياري)
) else (
    echo ✅ Git متوفر
)

REM التحقق من ملفات المشروع
echo [3/5] التحقق من ملفات المشروع...
if not exist "AlMohasebAlAraby.sln" (
    echo ❌ ملفات المشروع غير موجودة
    echo.
    echo 📂 تأكد من وجود الملفات التالية:
    echo    - AlMohasebAlAraby.sln
    echo    - AlMohasebAlAraby.Desktop\
    echo    - AlMohasebAlAraby.Shared\
    echo.
    echo 💡 إذا لم تكن قد حملت المشروع بعد:
    echo    git clone https://github.com/5olasa/EmployeeAttendanceSystem.git
    echo    git checkout feature/al-mohaseb-al-araby-transformation
    echo.
    pause
    exit /b 1
) else (
    echo ✅ ملفات المشروع موجودة
)

REM إنشاء المجلدات
echo [4/5] إعداد مجلدات البيانات...
if not exist "Data" (
    mkdir Data
    mkdir Data\Database
    mkdir Data\Reports
    mkdir Data\Exports
    mkdir Data\Backups
    mkdir Data\Logs
    mkdir Data\Images
    mkdir Data\Templates
    echo ✅ تم إنشاء مجلدات البيانات
) else (
    echo ✅ مجلدات البيانات موجودة
)

REM بناء المشروع
echo [5/5] بناء المشروع...
echo    جاري البناء... يرجى الانتظار
dotnet build AlMohasebAlAraby.sln --configuration Release --verbosity minimal --nologo
if %errorlevel% neq 0 (
    echo.
    echo ❌ فشل في بناء المشروع
    echo.
    echo 🔧 محاولة بناء مع تفاصيل الأخطاء:
    dotnet build AlMohasebAlAraby.sln --configuration Release
    echo.
    pause
    exit /b 1
) else (
    echo ✅ تم بناء المشروع بنجاح
)

echo.
echo ╔══════════════════════════════════════════════════════════════╗
echo ║                         🚀 تشغيل التطبيق                         ║
echo ╚══════════════════════════════════════════════════════════════╝
echo.
echo 🔑 بيانات تسجيل الدخول الافتراضية:
echo    👤 اسم المستخدم: admin
echo    🔒 كلمة المرور: admin123
echo.
echo 💡 نصائح:
echo    • التطبيق يعمل بالكامل دون إنترنت
echo    • قاعدة البيانات ستُنشأ تلقائياً في مجلد Data
echo    • يمكنك تغيير بيانات الشركة من الإعدادات
echo.

echo ⏳ جاري تشغيل المحاسب العربي...
echo.

REM تشغيل التطبيق
cd AlMohasebAlAraby.Desktop
start "المحاسب العربي" dotnet run --configuration Release --no-build

echo ✅ تم تشغيل التطبيق
echo.
echo 📝 ملاحظة: إذا لم يظهر التطبيق، تحقق من Windows Defender
echo           أو تشغيل هذا الملف كمدير
echo.
echo اضغط أي مفتاح للخروج...
pause >nul
