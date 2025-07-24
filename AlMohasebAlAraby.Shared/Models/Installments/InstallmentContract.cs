using System.ComponentModel.DataAnnotations;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Shared.Models.Installments
{
    /// <summary>
    /// عقد التقسيط
    /// </summary>
    public class InstallmentContract : BaseEntity
    {
        /// <summary>
        /// رقم العقد
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ العقد
        /// </summary>
        public DateTime ContractDate { get; set; } = DateTime.Now;

        /// <summary>
        /// معرف العميل
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// العميل
        /// </summary>
        public virtual Customers.Customer Customer { get; set; } = null!;

        /// <summary>
        /// إجمالي قيمة العقد
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// المقدم المدفوع
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// المبلغ المتبقي للتقسيط
        /// </summary>
        public decimal FinancedAmount => TotalAmount - DownPayment;

        /// <summary>
        /// معدل الفائدة السنوي
        /// </summary>
        public decimal InterestRate { get; set; } = 0;

        /// <summary>
        /// عدد الأقساط
        /// </summary>
        public int NumberOfInstallments { get; set; }

        /// <summary>
        /// قيمة القسط الشهري
        /// </summary>
        public decimal MonthlyInstallmentAmount { get; set; }

        /// <summary>
        /// تاريخ بداية الأقساط
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ انتهاء الأقساط
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// حالة العقد
        /// </summary>
        public InstallmentContractStatus Status { get; set; } = InstallmentContractStatus.Active;

        /// <summary>
        /// إجمالي المبلغ المدفوع
        /// </summary>
        public decimal TotalPaidAmount { get; set; } = 0;

        /// <summary>
        /// المبلغ المتبقي
        /// </summary>
        public decimal RemainingAmount => TotalAmount - TotalPaidAmount;

        /// <summary>
        /// عدد الأقساط المدفوعة
        /// </summary>
        public int PaidInstallments { get; set; } = 0;

        /// <summary>
        /// عدد الأقساط المتبقية
        /// </summary>
        public int RemainingInstallments => NumberOfInstallments - PaidInstallments;

        /// <summary>
        /// تاريخ آخر دفعة
        /// </summary>
        public DateTime? LastPaymentDate { get; set; }

        /// <summary>
        /// معرف فاتورة المبيعات المرتبطة
        /// </summary>
        public int? SalesInvoiceId { get; set; }

        /// <summary>
        /// فاتورة المبيعات المرتبطة
        /// </summary>
        public virtual Sales.SalesInvoice? SalesInvoice { get; set; }

        /// <summary>
        /// الضامن الأول
        /// </summary>
        [StringLength(200)]
        public string? Guarantor1Name { get; set; }

        /// <summary>
        /// هاتف الضامن الأول
        /// </summary>
        [StringLength(20)]
        public string? Guarantor1Phone { get; set; }

        /// <summary>
        /// عنوان الضامن الأول
        /// </summary>
        [StringLength(300)]
        public string? Guarantor1Address { get; set; }

        /// <summary>
        /// الضامن الثاني
        /// </summary>
        [StringLength(200)]
        public string? Guarantor2Name { get; set; }

        /// <summary>
        /// هاتف الضامن الثاني
        /// </summary>
        [StringLength(20)]
        public string? Guarantor2Phone { get; set; }

        /// <summary>
        /// عنوان الضامن الثاني
        /// </summary>
        [StringLength(300)]
        public string? Guarantor2Address { get; set; }

        /// <summary>
        /// شروط العقد
        /// </summary>
        [StringLength(2000)]
        public string? Terms { get; set; }

        /// <summary>
        /// جدول الأقساط
        /// </summary>
        public virtual ICollection<InstallmentSchedule> InstallmentSchedules { get; set; } = new List<InstallmentSchedule>();

        /// <summary>
        /// مدفوعات الأقساط
        /// </summary>
        public virtual ICollection<InstallmentPayment> InstallmentPayments { get; set; } = new List<InstallmentPayment>();

        /// <summary>
        /// أصناف العقد
        /// </summary>
        public virtual ICollection<InstallmentContractItem> ContractItems { get; set; } = new List<InstallmentContractItem>();

        /// <summary>
        /// حساب قيمة القسط الشهري
        /// </summary>
        public decimal CalculateMonthlyInstallment()
        {
            if (NumberOfInstallments == 0) return 0;

            decimal principal = FinancedAmount;
            if (InterestRate == 0)
            {
                return principal / NumberOfInstallments;
            }

            decimal monthlyRate = InterestRate / 100 / 12;
            decimal factor = (decimal)Math.Pow((double)(1 + monthlyRate), NumberOfInstallments);
            return principal * monthlyRate * factor / (factor - 1);
        }

        /// <summary>
        /// إنشاء جدول الأقساط
        /// </summary>
        public void GenerateInstallmentSchedule()
        {
            InstallmentSchedules.Clear();
            
            DateTime currentDate = StartDate;
            decimal remainingBalance = FinancedAmount;
            decimal monthlyRate = InterestRate / 100 / 12;

            for (int i = 1; i <= NumberOfInstallments; i++)
            {
                decimal interestAmount = remainingBalance * monthlyRate;
                decimal principalAmount = MonthlyInstallmentAmount - interestAmount;
                
                var schedule = new InstallmentSchedule
                {
                    InstallmentContractId = Id,
                    InstallmentNumber = i,
                    DueDate = currentDate,
                    InstallmentAmount = MonthlyInstallmentAmount,
                    PrincipalAmount = principalAmount,
                    InterestAmount = interestAmount,
                    RemainingBalance = remainingBalance - principalAmount,
                    Status = InstallmentStatus.Pending
                };

                InstallmentSchedules.Add(schedule);
                
                remainingBalance -= principalAmount;
                currentDate = currentDate.AddMonths(1);
            }
        }

        /// <summary>
        /// التحقق من وجود أقساط متأخرة
        /// </summary>
        public bool HasOverdueInstallments()
        {
            return InstallmentSchedules.Any(s => s.Status == InstallmentStatus.Overdue);
        }

        /// <summary>
        /// الحصول على الأقساط المتأخرة
        /// </summary>
        public IEnumerable<InstallmentSchedule> GetOverdueInstallments()
        {
            return InstallmentSchedules.Where(s => s.Status == InstallmentStatus.Overdue);
        }

        /// <summary>
        /// الحصول على القسط التالي المستحق
        /// </summary>
        public InstallmentSchedule? GetNextDueInstallment()
        {
            return InstallmentSchedules
                .Where(s => s.Status == InstallmentStatus.Pending)
                .OrderBy(s => s.DueDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// حساب إجمالي المبلغ المتأخر
        /// </summary>
        public decimal GetTotalOverdueAmount()
        {
            return GetOverdueInstallments().Sum(s => s.RemainingAmount);
        }
    }

    /// <summary>
    /// حالة عقد التقسيط
    /// </summary>
    public enum InstallmentContractStatus
    {
        /// <summary>
        /// نشط
        /// </summary>
        Active = 1,

        /// <summary>
        /// مكتمل
        /// </summary>
        Completed = 2,

        /// <summary>
        /// ملغي
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// متأخر
        /// </summary>
        Overdue = 4,

        /// <summary>
        /// معلق
        /// </summary>
        Suspended = 5
    }
}
