-- إدخال بيانات تجريبية لنظام حضور الموظفين
USE EmployeeAttendance;
GO

-- إدخال موظفين تجريبيين
INSERT INTO [dbo].[Employees] ([EmployeeNumber], [Name], [Email], [Phone], [HireDate], [ShiftId], [MonthlySalary], [AvailableVacationDays])
VALUES 
    (N'EMP001', N'أحمد محمد علي', N'ahmed.mohamed@company.com', N'01234567890', '2022-01-15', 1, 5000.00, 21),
    (N'EMP002', N'فاطمة أحمد حسن', N'fatma.ahmed@company.com', N'01234567891', '2022-02-01', 1, 4500.00, 21),
    (N'EMP003', N'محمد عبدالله سالم', N'mohamed.abdullah@company.com', N'01234567892', '2022-03-10', 2, 5500.00, 21),
    (N'EMP004', N'نورا حسام الدين', N'nora.hossam@company.com', N'01234567893', '2022-04-05', 1, 4800.00, 21),
    (N'EMP005', N'عمر خالد محمود', N'omar.khaled@company.com', N'01234567894', '2022-05-20', 3, 6000.00, 21),
    (N'EMP006', N'سارة عبدالرحمن', N'sara.abdelrahman@company.com', N'01234567895', '2022-06-15', 1, 4700.00, 21),
    (N'EMP007', N'يوسف إبراهيم أحمد', N'youssef.ibrahim@company.com', N'01234567896', '2022-07-01', 2, 5200.00, 21),
    (N'EMP008', N'مريم محمد حسين', N'mariam.mohamed@company.com', N'01234567897', '2022-08-10', 1, 4600.00, 21),
    (N'EMP009', N'حسام علي عبدالله', N'hossam.ali@company.com', N'01234567898', '2022-09-05', 3, 5800.00, 21),
    (N'EMP010', N'دينا أحمد محمد', N'dina.ahmed@company.com', N'01234567899', '2022-10-20', 1, 4900.00, 21);

-- إدخال سجلات حضور تجريبية للأسبوع الماضي
DECLARE @StartDate DATE = DATEADD(DAY, -7, GETDATE());
DECLARE @EndDate DATE = GETDATE();
DECLARE @CurrentDate DATE = @StartDate;
DECLARE @EmployeeId INT;
DECLARE @CheckInTime DATETIME;
DECLARE @CheckOutTime DATETIME;
DECLARE @IsLate BIT;
DECLARE @LateMinutes INT;

-- إدخال سجلات حضور لكل موظف لكل يوم عمل في الأسبوع الماضي
WHILE @CurrentDate <= @EndDate
BEGIN
    -- تجاهل عطلات نهاية الأسبوع (الجمعة والسبت)
    IF DATEPART(WEEKDAY, @CurrentDate) NOT IN (6, 7) -- الجمعة = 6, السبت = 7
    BEGIN
        -- إدخال سجلات لكل موظف
        DECLARE employee_cursor CURSOR FOR 
        SELECT Id FROM Employees;
        
        OPEN employee_cursor;
        FETCH NEXT FROM employee_cursor INTO @EmployeeId;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- محاكاة أوقات حضور متنوعة
            SET @CheckInTime = DATEADD(MINUTE, 
                CASE 
                    WHEN @EmployeeId % 4 = 0 THEN ABS(CHECKSUM(NEWID()) % 30) -- متأخر أحياناً
                    ELSE ABS(CHECKSUM(NEWID()) % 15) -- في الوقت عادة
                END, 
                CAST(@CurrentDate AS DATETIME) + CAST('08:00:00' AS TIME));
            
            -- تحديد ما إذا كان متأخراً
            SET @IsLate = CASE WHEN @CheckInTime > CAST(@CurrentDate AS DATETIME) + CAST('08:15:00' AS TIME) THEN 1 ELSE 0 END;
            SET @LateMinutes = CASE WHEN @IsLate = 1 THEN DATEDIFF(MINUTE, CAST(@CurrentDate AS DATETIME) + CAST('08:00:00' AS TIME), @CheckInTime) ELSE 0 END;
            
            -- محاكاة وقت الانصراف (8 ساعات عمل + استراحة)
            SET @CheckOutTime = DATEADD(HOUR, 8, @CheckInTime);
            SET @CheckOutTime = DATEADD(MINUTE, ABS(CHECKSUM(NEWID()) % 30), @CheckOutTime); -- تنويع وقت الانصراف
            
            -- إدخال سجل الحضور
            INSERT INTO [dbo].[Attendance] 
            ([EmployeeId], [Date], [CheckInTime], [CheckOutTime], [IsManualCheckIn], [IsManualCheckOut], 
             [IsLate], [LateMinutes], [Device], [Notes])
            VALUES 
            (@EmployeeId, @CurrentDate, @CheckInTime, @CheckOutTime, 0, 0, 
             @IsLate, @LateMinutes, N'تطبيق الموبايل', 
             CASE WHEN @IsLate = 1 THEN N'تأخير في الحضور' ELSE N'حضور منتظم' END);
            
            FETCH NEXT FROM employee_cursor INTO @EmployeeId;
        END
        
        CLOSE employee_cursor;
        DEALLOCATE employee_cursor;
    END
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

-- إدخال بعض سجلات الإجازات التجريبية
INSERT INTO [dbo].[Vacations] ([EmployeeId], [StartDate], [EndDate], [Type], [Reason], [Status])
VALUES 
    (1, '2024-01-15', '2024-01-17', N'إجازة سنوية', N'إجازة شخصية', N'Approved'),
    (2, '2024-01-20', '2024-01-22', N'إجازة مرضية', N'إجازة مرضية', N'Approved'),
    (3, '2024-02-01', '2024-02-03', N'إجازة سنوية', N'سفر عائلي', N'Pending'),
    (4, '2024-02-10', '2024-02-12', N'إجازة طارئة', N'ظروف عائلية', N'Approved'),
    (5, '2024-02-15', '2024-02-17', N'إجازة سنوية', N'راحة', N'Pending');

-- إدخال بعض سجلات النظام
INSERT INTO [dbo].[SystemLogs] ([Level], [Source], [Message], [UserId])
VALUES 
    (N'Info', N'Authentication', N'تم تسجيل دخول المستخدم بنجاح', 1),
    (N'Info', N'Attendance', N'تم تسجيل حضور الموظف EMP001', 1),
    (N'Warning', N'Attendance', N'تأخير في الحضور للموظف EMP003', 1),
    (N'Info', N'System', N'تم بدء تشغيل النظام', 1),
    (N'Info', N'Sync', N'تمت مزامنة البيانات بنجاح', 1);

-- عرض ملخص البيانات المدخلة
SELECT 'الموظفين' AS [النوع], COUNT(*) AS [العدد] FROM Employees
UNION ALL
SELECT 'سجلات الحضور' AS [النوع], COUNT(*) AS [العدد] FROM Attendance
UNION ALL
SELECT 'الإجازات' AS [النوع], COUNT(*) AS [العدد] FROM Vacations
UNION ALL
SELECT 'سجلات النظام' AS [النوع], COUNT(*) AS [العدد] FROM SystemLogs;

PRINT N'تم إدخال البيانات التجريبية بنجاح!';
PRINT N'يمكنك الآن تسجيل الدخول باستخدام:';
PRINT N'رقم الموظف: EMP001';
PRINT N'كلمة المرور: 123456';

GO
