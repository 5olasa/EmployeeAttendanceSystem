-- إنشاء قاعدة البيانات
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'EmployeeAttendance')
BEGIN
    CREATE DATABASE EmployeeAttendance;
END
GO

USE EmployeeAttendance;
GO

-- إنشاء جدول الشفتات
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shifts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Shifts] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL,
        [Description] NVARCHAR(255) NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إدخال بيانات الشفتات الافتراضية
    INSERT INTO [dbo].[Shifts] ([Name], [StartTime], [EndTime], [Description])
    VALUES 
        (N'صباحي', '08:00:00', '16:00:00', N'الشفت الصباحي من 8 صباحاً إلى 4 مساءً'),
        (N'مسائي', '16:00:00', '00:00:00', N'الشفت المسائي من 4 مساءً إلى 12 منتصف الليل'),
        (N'ليلي', '00:00:00', '08:00:00', N'الشفت الليلي من 12 منتصف الليل إلى 8 صباحاً');
END
GO

-- إنشاء جدول المستخدمين
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [Password] NVARCHAR(255) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [LastLogin] DATETIME NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إدخال بيانات المستخدم الافتراضي (admin/admin123)
    INSERT INTO [dbo].[Users] ([Username], [Password], [FullName], [Email], [Role])
    VALUES (N'admin', N'admin123', N'مدير النظام', N'admin@example.com', N'Admin');
END
GO

-- إنشاء جدول الموظفين
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [EmployeeNumber] NVARCHAR(20) NOT NULL UNIQUE,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(20) NULL,
        [HireDate] DATE NOT NULL,
        [ShiftId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Shifts]([Id]),
        [MonthlySalary] DECIMAL(18, 2) NOT NULL,
        [AvailableVacationDays] INT NOT NULL DEFAULT 21,
        [FaceImagePath] NVARCHAR(255) NULL,
        [FaceImageBase64] NVARCHAR(MAX) NULL,
        [FaceEncodingData] VARBINARY(MAX) NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إنشاء فهرس على رقم الموظف
    CREATE INDEX IX_Employees_EmployeeNumber ON [dbo].[Employees]([EmployeeNumber]);
END
GO

-- إنشاء جدول الحضور
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attendance]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Attendance] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [EmployeeId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Employees]([Id]),
        [Date] DATE NOT NULL,
        [CheckInTime] DATETIME NULL,
        [CheckOutTime] DATETIME NULL,
        [CheckInImagePath] NVARCHAR(255) NULL,
        [CheckOutImagePath] NVARCHAR(255) NULL,
        [IsManualCheckIn] BIT NOT NULL DEFAULT 0,
        [IsManualCheckOut] BIT NOT NULL DEFAULT 0,
        [Notes] NVARCHAR(255) NULL,
        [IsLate] BIT NOT NULL DEFAULT 0,
        [LateMinutes] INT NOT NULL DEFAULT 0,
        [Device] NVARCHAR(100) NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إنشاء فهرس مركب على معرف الموظف والتاريخ
    CREATE UNIQUE INDEX IX_Attendance_EmployeeId_Date ON [dbo].[Attendance]([EmployeeId], [Date]);
END
GO

-- إنشاء جدول الإجازات
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vacations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vacations] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [EmployeeId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Employees]([Id]),
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [Reason] NVARCHAR(255) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT N'Pending',
        [ApprovedBy] INT NULL FOREIGN KEY REFERENCES [dbo].[Users]([Id]),
        [ApprovalDate] DATETIME NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إنشاء فهرس على معرف الموظف
    CREATE INDEX IX_Vacations_EmployeeId ON [dbo].[Vacations]([EmployeeId]);
END
GO

-- إنشاء جدول إعدادات النظام
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Settings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Settings] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Key] NVARCHAR(50) NOT NULL UNIQUE,
        [Value] NVARCHAR(MAX) NULL,
        [Description] NVARCHAR(255) NULL,
        [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    -- إدخال الإعدادات الافتراضية
    INSERT INTO [dbo].[Settings] ([Key], [Value], [Description])
    VALUES 
        (N'CompanyName', N'شركة نظام الحضور', N'اسم الشركة'),
        (N'LateThresholdMinutes', N'15', N'عدد دقائق التأخير المسموح بها قبل اعتبار الموظف متأخراً'),
        (N'WorkHoursPerDay', N'8', N'عدد ساعات العمل اليومية'),
        (N'FaceRecognitionThreshold', N'0.6', N'نسبة التطابق المطلوبة للتعرف على الوجه (0.6 = 60%)'),
        (N'SyncEnabled', N'true', N'تفعيل المزامنة مع الخادم المركزي'),
        (N'SyncInterval', N'15', N'الفاصل الزمني للمزامنة بالدقائق');
END
GO

-- إنشاء جدول سجلات النظام
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SystemLogs] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
        [Level] NVARCHAR(20) NOT NULL,
        [Source] NVARCHAR(50) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [UserId] INT NULL FOREIGN KEY REFERENCES [dbo].[Users]([Id]),
        [Details] NVARCHAR(MAX) NULL
    );
    
    -- إنشاء فهرس على وقت الحدث
    CREATE INDEX IX_SystemLogs_Timestamp ON [dbo].[SystemLogs]([Timestamp]);
END
GO

-- إنشاء إجراء مخزن لتسجيل الحضور
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RecordCheckIn]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_RecordCheckIn]
        @EmployeeId INT,
        @Date DATE,
        @CheckInTime DATETIME,
        @CheckInImagePath NVARCHAR(255) = NULL,
        @IsManualCheckIn BIT = 0,
        @Notes NVARCHAR(255) = NULL,
        @Device NVARCHAR(100) = NULL,
        @AttendanceId INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @ShiftId INT;
        DECLARE @ShiftStartTime TIME;
        DECLARE @LateThresholdMinutes INT;
        DECLARE @IsLate BIT = 0;
        DECLARE @LateMinutes INT = 0;
        
        -- الحصول على معرف الشفت للموظف
        SELECT @ShiftId = ShiftId FROM Employees WHERE Id = @EmployeeId;
        
        -- الحصول على وقت بداية الشفت
        SELECT @ShiftStartTime = StartTime FROM Shifts WHERE Id = @ShiftId;
        
        -- الحصول على عدد دقائق التأخير المسموح بها
        SELECT @LateThresholdMinutes = CONVERT(INT, Value) FROM Settings WHERE [Key] = ''LateThresholdMinutes'';
        
        -- حساب ما إذا كان الموظف متأخراً
        IF CAST(@CheckInTime AS TIME) > DATEADD(MINUTE, @LateThresholdMinutes, @ShiftStartTime)
        BEGIN
            SET @IsLate = 1;
            SET @LateMinutes = DATEDIFF(MINUTE, @ShiftStartTime, CAST(@CheckInTime AS TIME));
        END
        
        -- التحقق من وجود سجل حضور للموظف في نفس اليوم
        IF EXISTS (SELECT 1 FROM Attendance WHERE EmployeeId = @EmployeeId AND [Date] = @Date)
        BEGIN
            -- تحديث سجل الحضور الموجود
            UPDATE Attendance
            SET CheckInTime = @CheckInTime,
                CheckInImagePath = ISNULL(@CheckInImagePath, CheckInImagePath),
                IsManualCheckIn = @IsManualCheckIn,
                Notes = ISNULL(@Notes, Notes),
                IsLate = @IsLate,
                LateMinutes = @LateMinutes,
                Device = ISNULL(@Device, Device),
                LastUpdated = GETDATE()
            WHERE EmployeeId = @EmployeeId AND [Date] = @Date;
            
            SELECT @AttendanceId = Id FROM Attendance WHERE EmployeeId = @EmployeeId AND [Date] = @Date;
        END
        ELSE
        BEGIN
            -- إنشاء سجل حضور جديد
            INSERT INTO Attendance (EmployeeId, [Date], CheckInTime, CheckInImagePath, IsManualCheckIn, Notes, IsLate, LateMinutes, Device, LastUpdated)
            VALUES (@EmployeeId, @Date, @CheckInTime, @CheckInImagePath, @IsManualCheckIn, @Notes, @IsLate, @LateMinutes, @Device, GETDATE());
            
            SET @AttendanceId = SCOPE_IDENTITY();
        END
        
        RETURN 0;
    END
    ');
END
GO

-- إنشاء إجراء مخزن لتسجيل الانصراف
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RecordCheckOut]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_RecordCheckOut]
        @AttendanceId INT,
        @CheckOutTime DATETIME,
        @CheckOutImagePath NVARCHAR(255) = NULL,
        @IsManualCheckOut BIT = 0,
        @Device NVARCHAR(100) = NULL,
        @Success BIT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SET @Success = 0;
        
        -- التحقق من وجود سجل الحضور
        IF EXISTS (SELECT 1 FROM Attendance WHERE Id = @AttendanceId)
        BEGIN
            -- تحديث سجل الحضور
            UPDATE Attendance
            SET CheckOutTime = @CheckOutTime,
                CheckOutImagePath = ISNULL(@CheckOutImagePath, CheckOutImagePath),
                IsManualCheckOut = @IsManualCheckOut,
                Device = ISNULL(@Device, Device),
                LastUpdated = GETDATE()
            WHERE Id = @AttendanceId;
            
            SET @Success = 1;
        END
        
        RETURN 0;
    END
    ');
END
GO

-- إنشاء إجراء مخزن للحصول على سجلات الحضور
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAttendanceRecords]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_GetAttendanceRecords]
        @EmployeeId INT = NULL,
        @StartDate DATE,
        @EndDate DATE
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT a.*, e.Name as EmployeeName, e.EmployeeNumber
        FROM Attendance a
        INNER JOIN Employees e ON a.EmployeeId = e.Id
        WHERE (@EmployeeId IS NULL OR a.EmployeeId = @EmployeeId)
          AND a.[Date] BETWEEN @StartDate AND @EndDate
        ORDER BY a.[Date] DESC, a.CheckInTime DESC;
        
        RETURN 0;
    END
    ');
END
GO

PRINT N'تم إنشاء قاعدة البيانات والجداول بنجاح';
