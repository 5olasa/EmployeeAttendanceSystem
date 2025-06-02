# استخدام .NET 8 SDK للبناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# نسخ ملفات المشروع
COPY *.sln .
COPY EmployeeAttendance.API/*.csproj ./EmployeeAttendance.API/
COPY EmployeeAttendance.Shared/*.csproj ./EmployeeAttendance.Shared/

# استعادة الحزم
RUN dotnet restore

# نسخ باقي الملفات
COPY . .

# بناء المشروع
WORKDIR /app/EmployeeAttendance.API
RUN dotnet publish -c Release -o out

# استخدام .NET 8 Runtime للتشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/EmployeeAttendance.API/out .

# تعيين المنفذ
ENV ASPNETCORE_URLS=http://+:$PORT

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "EmployeeAttendance.API.dll"]
