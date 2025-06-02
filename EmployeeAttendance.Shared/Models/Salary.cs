using System;
using System.Collections.Generic;

namespace EmployeeAttendance.Shared.Models
{
    /// <summary>
    /// نموذج بيانات المرتبات
    /// </summary>
    public class Salary
    {
        /// <summary>
        /// معرف المرتب
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// مرجع للموظف
        /// </summary>
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// الشهر (1-12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// السنة
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// الراتب الأساسي
        /// </summary>
        public decimal BasicSalary { get; set; }

        /// <summary>
        /// بدل السكن
        /// </summary>
        public decimal HousingAllowance { get; set; }

        /// <summary>
        /// بدل المواصلات
        /// </summary>
        public decimal TransportationAllowance { get; set; }

        /// <summary>
        /// بدلات أخرى
        /// </summary>
        public decimal OtherAllowances { get; set; }

        /// <summary>
        /// المكافآت
        /// </summary>
        public decimal Bonuses { get; set; }

        /// <summary>
        /// خصم الغياب
        /// </summary>
        public decimal AbsenceDeduction { get; set; }

        /// <summary>
        /// خصم التأخير
        /// </summary>
        public decimal LateDeduction { get; set; }

        /// <summary>
        /// خصومات أخرى
        /// </summary>
        public decimal OtherDeductions { get; set; }

        /// <summary>
        /// إجمالي المرتب قبل الخصومات
        /// </summary>
        public decimal GrossSalary => BasicSalary + HousingAllowance + TransportationAllowance + OtherAllowances + Bonuses;

        /// <summary>
        /// إجمالي الخصومات
        /// </summary>
        public decimal TotalDeductions => AbsenceDeduction + LateDeduction + OtherDeductions;

        /// <summary>
        /// صافي المرتب
        /// </summary>
        public decimal NetSalary => GrossSalary - TotalDeductions;

        /// <summary>
        /// تاريخ الدفع
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// حالة الدفع
        /// </summary>
        public SalaryStatus Status { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ آخر تحديث للبيانات
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// قائمة تفاصيل المرتب
        /// </summary>
        public virtual ICollection<SalaryDetail> Details { get; set; }

        public Salary()
        {
            Status = SalaryStatus.Pending;
            LastUpdated = DateTime.Now;
            Details = new List<SalaryDetail>();
        }
    }

    /// <summary>
    /// حالة المرتب
    /// </summary>
    public enum SalaryStatus
    {
        /// <summary>
        /// قيد الانتظار
        /// </summary>
        Pending = 0,

        /// <summary>
        /// تمت الموافقة
        /// </summary>
        Approved = 1,

        /// <summary>
        /// تم الدفع
        /// </summary>
        Paid = 2,

        /// <summary>
        /// ملغي
        /// </summary>
        Cancelled = 3
    }

    /// <summary>
    /// نموذج تفاصيل المرتب
    /// </summary>
    public class SalaryDetail
    {
        /// <summary>
        /// معرف التفصيل
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// معرف المرتب
        /// </summary>
        public int SalaryId { get; set; }

        /// <summary>
        /// مرجع للمرتب
        /// </summary>
        public virtual Salary? Salary { get; set; }

        /// <summary>
        /// نوع التفصيل
        /// </summary>
        public SalaryDetailType Type { get; set; }

        /// <summary>
        /// وصف التفصيل
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// المبلغ
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// تاريخ التفصيل
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        public SalaryDetail()
        {
            Date = DateTime.Now;
        }
    }

    /// <summary>
    /// نوع تفصيل المرتب
    /// </summary>
    public enum SalaryDetailType
    {
        /// <summary>
        /// راتب أساسي
        /// </summary>
        BasicSalary = 0,

        /// <summary>
        /// بدل
        /// </summary>
        Allowance = 1,

        /// <summary>
        /// مكافأة
        /// </summary>
        Bonus = 2,

        /// <summary>
        /// خصم
        /// </summary>
        Deduction = 3,

        /// <summary>
        /// أخرى
        /// </summary>
        Other = 4
    }
}
