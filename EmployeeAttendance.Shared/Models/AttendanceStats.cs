namespace EmployeeAttendance.Shared.Models
{
    /// <summary>
    /// نموذج بيانات إحصائيات الحضور
    /// </summary>
    public class AttendanceStats
    {
        /// <summary>
        /// معرف الموظف
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// إجمالي أيام العمل
        /// </summary>
        public int TotalWorkingDays { get; set; }

        /// <summary>
        /// عدد أيام الحضور
        /// </summary>
        public int PresentDays { get; set; }

        /// <summary>
        /// عدد أيام الغياب
        /// </summary>
        public int AbsentDays { get; set; }

        /// <summary>
        /// عدد أيام التأخير
        /// </summary>
        public int LateDays { get; set; }

        /// <summary>
        /// إجمالي ساعات العمل
        /// </summary>
        public double TotalWorkingHours { get; set; }

        /// <summary>
        /// متوسط ساعات العمل اليومية
        /// </summary>
        public double AverageWorkingHours { get; set; }

        /// <summary>
        /// نسبة الحضور
        /// </summary>
        public double AttendanceRate 
        { 
            get 
            { 
                if (TotalWorkingDays == 0) return 0;
                return (double)PresentDays / TotalWorkingDays * 100;
            } 
        }

        /// <summary>
        /// نسبة التأخير
        /// </summary>
        public double LateRate 
        { 
            get 
            { 
                if (PresentDays == 0) return 0;
                return (double)LateDays / PresentDays * 100;
            } 
        }
    }
}
