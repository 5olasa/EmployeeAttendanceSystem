using System.ComponentModel.DataAnnotations;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Shared.Models.Accounting
{
    /// <summary>
    /// قيد اليومية
    /// </summary>
    public class JournalEntry : BaseEntity
    {
        /// <summary>
        /// رقم القيد
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EntryNumber { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ القيد
        /// </summary>
        public DateTime EntryDate { get; set; } = DateTime.Now;

        /// <summary>
        /// نوع المستند
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// رقم المستند المرجعي
        /// </summary>
        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// وصف القيد
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// حالة القيد
        /// </summary>
        public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;

        /// <summary>
        /// إجمالي المبلغ المدين
        /// </summary>
        public decimal TotalDebitAmount { get; set; }

        /// <summary>
        /// إجمالي المبلغ الدائن
        /// </summary>
        public decimal TotalCreditAmount { get; set; }

        /// <summary>
        /// العملة
        /// </summary>
        public Currency Currency { get; set; } = Currency.EGP;

        /// <summary>
        /// سعر الصرف (إذا كانت العملة غير الجنيه المصري)
        /// </summary>
        public decimal ExchangeRate { get; set; } = 1;

        /// <summary>
        /// تاريخ الاعتماد
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// معرف المستخدم الذي اعتمد القيد
        /// </summary>
        public int? ApprovedBy { get; set; }

        /// <summary>
        /// تاريخ الترحيل
        /// </summary>
        public DateTime? PostedDate { get; set; }

        /// <summary>
        /// معرف المستخدم الذي رحل القيد
        /// </summary>
        public int? PostedBy { get; set; }

        /// <summary>
        /// تفاصيل القيد
        /// </summary>
        public virtual ICollection<JournalEntryDetail> Details { get; set; } = new List<JournalEntryDetail>();

        /// <summary>
        /// المرفقات
        /// </summary>
        public virtual ICollection<JournalEntryAttachment> Attachments { get; set; } = new List<JournalEntryAttachment>();

        /// <summary>
        /// التحقق من توازن القيد
        /// </summary>
        public bool IsBalanced()
        {
            return Math.Abs(TotalDebitAmount - TotalCreditAmount) < 0.01m;
        }

        /// <summary>
        /// حساب إجماليات القيد
        /// </summary>
        public void CalculateTotals()
        {
            TotalDebitAmount = Details.Sum(d => d.DebitAmount);
            TotalCreditAmount = Details.Sum(d => d.CreditAmount);
        }

        /// <summary>
        /// التحقق من صحة القيد
        /// </summary>
        public List<string> ValidateEntry()
        {
            var errors = new List<string>();

            if (Details.Count < 2)
            {
                errors.Add("يجب أن يحتوي القيد على حسابين على الأقل");
            }

            if (!IsBalanced())
            {
                errors.Add("القيد غير متوازن - إجمالي المدين يجب أن يساوي إجمالي الدائن");
            }

            if (Details.Any(d => d.DebitAmount == 0 && d.CreditAmount == 0))
            {
                errors.Add("لا يمكن أن يكون المبلغ صفر في أي من تفاصيل القيد");
            }

            if (Details.Any(d => d.DebitAmount > 0 && d.CreditAmount > 0))
            {
                errors.Add("لا يمكن أن يكون الحساب مدين ودائن في نفس الوقت");
            }

            return errors;
        }
    }

    /// <summary>
    /// تفاصيل قيد اليومية
    /// </summary>
    public class JournalEntryDetail : BaseEntity
    {
        /// <summary>
        /// معرف قيد اليومية
        /// </summary>
        public int JournalEntryId { get; set; }

        /// <summary>
        /// قيد اليومية
        /// </summary>
        public virtual JournalEntry JournalEntry { get; set; } = null!;

        /// <summary>
        /// معرف الحساب
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// الحساب
        /// </summary>
        public virtual ChartOfAccounts Account { get; set; } = null!;

        /// <summary>
        /// المبلغ المدين
        /// </summary>
        public decimal DebitAmount { get; set; }

        /// <summary>
        /// المبلغ الدائن
        /// </summary>
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// وصف التفصيل
        /// </summary>
        [StringLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// رقم الشيك (إن وجد)
        /// </summary>
        [StringLength(50)]
        public string? CheckNumber { get; set; }

        /// <summary>
        /// تاريخ الشيك (إن وجد)
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// اسم البنك (إن وجد)
        /// </summary>
        [StringLength(100)]
        public string? BankName { get; set; }

        /// <summary>
        /// ترتيب التفصيل في القيد
        /// </summary>
        public int LineNumber { get; set; }
    }

    /// <summary>
    /// مرفقات قيد اليومية
    /// </summary>
    public class JournalEntryAttachment : BaseEntity
    {
        /// <summary>
        /// معرف قيد اليومية
        /// </summary>
        public int JournalEntryId { get; set; }

        /// <summary>
        /// قيد اليومية
        /// </summary>
        public virtual JournalEntry JournalEntry { get; set; } = null!;

        /// <summary>
        /// اسم الملف
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// مسار الملف
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// نوع الملف
        /// </summary>
        [StringLength(50)]
        public string? FileType { get; set; }

        /// <summary>
        /// حجم الملف بالبايت
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// وصف المرفق
        /// </summary>
        [StringLength(300)]
        public string? Description { get; set; }
    }
}
