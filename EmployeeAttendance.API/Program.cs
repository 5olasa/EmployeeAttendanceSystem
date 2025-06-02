using EmployeeAttendance.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// إضافة الخدمات
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// إضافة CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// إضافة خدمة قاعدة البيانات
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SQLite";

if (databaseProvider == "SQLite")
{
    var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection")
        ?? "Data Source=EmployeeAttendance.db";
    builder.Services.AddSingleton(new SqliteDatabaseService(sqliteConnection));
}
else
{
    // SQL Server (يحتاج تثبيت SQL Server)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=localhost;Database=EmployeeAttendance;Trusted_Connection=true;TrustServerCertificate=true;";
    // builder.Services.AddSingleton(new DatabaseService(connectionString));

    // استخدام SQLite كبديل إذا لم يكن SQL Server متوفر
    var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection")
        ?? "Data Source=EmployeeAttendance.db";
    builder.Services.AddSingleton(new SqliteDatabaseService(sqliteConnection));
}

// إضافة JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// تكوين HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// إضافة endpoint للتحقق من صحة الخادم
app.MapGet("/api/health", () => new { Status = "OK", Timestamp = DateTime.Now });

// تشغيل اختبارات سريعة
Console.WriteLine("🚀 بدء تشغيل خادم نظام حضور الموظفين");
await TestApp.RunTests();

app.Run();
