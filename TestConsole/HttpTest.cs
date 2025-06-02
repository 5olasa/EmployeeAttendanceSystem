using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class HttpTest
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task TestAPI()
    {
        Console.WriteLine("🌐 اختبار Web API");
        Console.WriteLine("==================");
        
        string baseUrl = "http://localhost:5000";
        
        try
        {
            // اختبار 1: Health Check
            Console.WriteLine("\n🏥 اختبار 1: Health Check");
            var healthResponse = await client.GetAsync($"{baseUrl}/api/health");
            if (healthResponse.IsSuccessStatusCode)
            {
                var healthContent = await healthResponse.Content.ReadAsStringAsync();
                Console.WriteLine("✅ الخادم يعمل بشكل صحيح");
                Console.WriteLine($"   الاستجابة: {healthContent}");
            }
            else
            {
                Console.WriteLine($"❌ فشل في الوصول للخادم: {healthResponse.StatusCode}");
                return;
            }
            
            // اختبار 2: تسجيل الدخول
            Console.WriteLine("\n🔐 اختبار 2: تسجيل الدخول");
            var loginData = new
            {
                EmployeeNumber = "EMP001",
                Password = "123456"
            };
            
            var loginJson = JsonSerializer.Serialize(loginData);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
            
            var loginResponse = await client.PostAsync($"{baseUrl}/api/auth/login", loginContent);
            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadAsStringAsync();
                Console.WriteLine("✅ تم تسجيل الدخول بنجاح");
                Console.WriteLine($"   النتيجة: {loginResult}");
                
                // استخراج التوكن (بشكل مبسط)
                if (loginResult.Contains("token"))
                {
                    Console.WriteLine("✅ تم الحصول على التوكن");
                }
            }
            else
            {
                var errorContent = await loginResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ فشل في تسجيل الدخول: {loginResponse.StatusCode}");
                Console.WriteLine($"   الخطأ: {errorContent}");
            }
            
            // اختبار 3: الحصول على الموظفين (بدون مصادقة للاختبار)
            Console.WriteLine("\n👥 اختبار 3: الحصول على الموظفين");
            var employeesResponse = await client.GetAsync($"{baseUrl}/api/employees");
            Console.WriteLine($"   حالة الاستجابة: {employeesResponse.StatusCode}");
            
            if (employeesResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("⚠️ مطلوب مصادقة (هذا متوقع)");
            }
            else if (employeesResponse.IsSuccessStatusCode)
            {
                var employeesContent = await employeesResponse.Content.ReadAsStringAsync();
                Console.WriteLine("✅ تم الحصول على بيانات الموظفين");
                Console.WriteLine($"   البيانات: {employeesContent.Substring(0, Math.Min(100, employeesContent.Length))}...");
            }
            
            Console.WriteLine("\n🎉 انتهى اختبار API");
            
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ خطأ في الشبكة: {ex.Message}");
            Console.WriteLine("💡 تأكد من تشغيل الخادم على http://localhost:5000");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ خطأ عام: {ex.Message}");
        }
    }
}
