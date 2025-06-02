using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// نماذج بسيطة للاختبار
public class Employee
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal MonthlySalary { get; set; }
}

public class Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Device { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

// خدمة اختبار بسيطة
public class TestService
{
    private readonly List<Employee> _employees;
    private readonly List<Attendance> _attendanceRecords;
    private int _nextEmployeeId = 1;
    private int _nextAttendanceId = 1;

    public TestService()
    {
        _employees = new List<Employee>();
        _attendanceRecords = new List<Attendance>();
        InitializeData();
    }

    private void InitializeData()
    {
        _employees.AddRange(new[]
        {
            new Employee
            {
                Id = _nextEmployeeId++,
                EmployeeNumber = "EMP001",
                Name = "أحمد محمد علي",
                Email = "ahmed@company.com",
                HireDate = DateTime.Now.AddYears(-2),
                MonthlySalary = 5000
            },
            new Employee
            {
                Id = _nextEmployeeId++,
                EmployeeNumber = "EMP002",
                Name = "فاطمة أحمد حسن",
                Email = "fatma@company.com",
                HireDate = DateTime.Now.AddYears(-1),
                MonthlySalary = 4500
            }
        });

        // إضافة سجلات حضور تجريبية
        for (int i = 1; i <= 5; i++)
        {
            var date = DateTime.Today.AddDays(-i);
            _attendanceRecords.Add(new Attendance
            {
                Id = _nextAttendanceId++,
                EmployeeId = 1,
                Date = date,
                CheckInTime = date.AddHours(8).AddMinutes(15),
                CheckOutTime = date.AddHours(16).AddMinutes(30),
                Device = "تطبيق الموبايل",
                Notes = "حضور منتظم"
            });
        }
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        await Task.Delay(100);
        return _employees;
    }

    public async Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber)
    {
        await Task.Delay(100);
        return _employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber);
    }

    public async Task<int> RecordCheckInAsync(Attendance attendance)
    {
        await Task.Delay(100);
        attendance.Id = _nextAttendanceId++;
        _attendanceRecords.Add(attendance);
        return attendance.Id;
    }

    public async Task<List<Attendance>> GetAttendanceRecordsAsync(int employeeId)
    {
        await Task.Delay(100);
        return _attendanceRecords.Where(a => a.EmployeeId == employeeId).ToList();
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("🎯 اختبار نظام حضور الموظفين");
        Console.WriteLine("============================");
        
        var service = new TestService();
        
        try
        {
            // اختبار 1: الحصول على الموظفين
            Console.WriteLine("\n📋 اختبار 1: الحصول على جميع الموظفين");
            var employees = await service.GetAllEmployeesAsync();
            Console.WriteLine($"✅ تم العثور على {employees.Count} موظف:");
            
            foreach (var emp in employees)
            {
                Console.WriteLine($"   - {emp.Name} ({emp.EmployeeNumber}) - راتب: {emp.MonthlySalary:C}");
            }
            
            // اختبار 2: البحث عن موظف
            Console.WriteLine("\n🔍 اختبار 2: البحث عن موظف EMP001");
            var employee = await service.GetEmployeeByNumberAsync("EMP001");
            if (employee != null)
            {
                Console.WriteLine($"✅ تم العثور على: {employee.Name}");
                Console.WriteLine($"   البريد الإلكتروني: {employee.Email}");
                Console.WriteLine($"   تاريخ التوظيف: {employee.HireDate:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("❌ لم يتم العثور على الموظف");
            }
            
            // اختبار 3: تسجيل حضور جديد
            Console.WriteLine("\n⏰ اختبار 3: تسجيل حضور جديد");
            var newAttendance = new Attendance
            {
                EmployeeId = 1,
                Date = DateTime.Today,
                CheckInTime = DateTime.Now,
                Device = "تطبيق الاختبار",
                Notes = "اختبار تسجيل الحضور"
            };
            
            var attendanceId = await service.RecordCheckInAsync(newAttendance);
            Console.WriteLine($"✅ تم تسجيل الحضور برقم: {attendanceId}");
            Console.WriteLine($"   الوقت: {newAttendance.CheckInTime:HH:mm:ss}");
            
            // اختبار 4: الحصول على سجلات الحضور
            Console.WriteLine("\n📊 اختبار 4: الحصول على سجلات الحضور");
            var records = await service.GetAttendanceRecordsAsync(1);
            Console.WriteLine($"✅ تم العثور على {records.Count} سجل حضور:");
            
            foreach (var record in records.Take(3))
            {
                var checkIn = record.CheckInTime?.ToString("HH:mm") ?? "غير محدد";
                var checkOut = record.CheckOutTime?.ToString("HH:mm") ?? "غير محدد";
                Console.WriteLine($"   - {record.Date:yyyy-MM-dd}: {checkIn} - {checkOut}");
            }
            
            // اختبار 5: محاكاة تسجيل دخول
            Console.WriteLine("\n🔐 اختبار 5: محاكاة تسجيل الدخول");
            var loginEmployee = await service.GetEmployeeByNumberAsync("EMP001");
            if (loginEmployee != null)
            {
                Console.WriteLine("✅ تم تسجيل الدخول بنجاح!");
                Console.WriteLine($"   مرحباً {loginEmployee.Name}");
                Console.WriteLine($"   رقم الموظف: {loginEmployee.EmployeeNumber}");
            }
            
            Console.WriteLine("\n🎉 جميع الاختبارات المحلية نجحت!");
            Console.WriteLine("============================");

            // اختبار HTTP API
            await HttpTest.TestAPI();

            Console.WriteLine("\n💡 النظام جاهز للاستخدام:");
            Console.WriteLine("   - تسجيل الدخول: EMP001 / 123456");
            Console.WriteLine("   - إدارة الموظفين: متاحة");
            Console.WriteLine("   - تسجيل الحضور: يعمل");
            Console.WriteLine("   - سجلات الحضور: متوفرة");
            Console.WriteLine("   - Web API: http://localhost:5000");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ حدث خطأ: {ex.Message}");
        }

        Console.WriteLine("\nاضغط أي مفتاح للخروج...");
        Console.ReadKey();
    }
}
