using System.ComponentModel.DataAnnotations;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Shared.Models.Accounting
{
    /// <summary>
    /// دليل الحسابات
    /// </summary>
    public class ChartOfAccounts : BaseEntity
    {
        /// <summary>
        /// كود الحساب
        /// </summary>
        [Required]
        [StringLength(20)]
        public string AccountCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم الحساب باللغة العربية
        /// </summary>
        [Required]
        [StringLength(200)]
        public string AccountNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الحساب باللغة الإنجليزية
        /// </summary>
        [StringLength(200)]
        public string? AccountNameEn { get; set; }

        /// <summary>
        /// نوع الحساب
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// طبيعة الحساب (مدين/دائن)
        /// </summary>
        public AccountNature AccountNature { get; set; }

        /// <summary>
        /// مستوى الحساب في الشجرة
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// معرف الحساب الأب
        /// </summary>
        public int? ParentAccountId { get; set; }

        /// <summary>
        /// الحساب الأب
        /// </summary>
        public virtual ChartOfAccounts? ParentAccount { get; set; }

        /// <summary>
        /// الحسابات الفرعية
        /// </summary>
        public virtual ICollection<ChartOfAccounts> SubAccounts { get; set; } = new List<ChartOfAccounts>();

        /// <summary>
        /// هل الحساب قابل للترحيل (حساب تفصيلي)
        /// </summary>
        public bool IsPostable { get; set; } = true;

        /// <summary>
        /// هل الحساب مخصص للعملاء
        /// </summary>
        public bool IsCustomerAccount { get; set; } = false;

        /// <summary>
        /// هل الحساب مخصص للموردين
        /// </summary>
        public bool IsSupplierAccount { get; set; } = false;

        /// <summary>
        /// هل الحساب مخصص للبنوك
        /// </summary>
        public bool IsBankAccount { get; set; } = false;

        /// <summary>
        /// هل الحساب مخصص للصندوق
        /// </summary>
        public bool IsCashAccount { get; set; } = false;

        /// <summary>
        /// العملة الافتراضية للحساب
        /// </summary>
        public Currency DefaultCurrency { get; set; } = Currency.EGP;

        /// <summary>
        /// الرصيد الافتتاحي
        /// </summary>
        public decimal OpeningBalance { get; set; } = 0;

        /// <summary>
        /// الرصيد الحالي
        /// </summary>
        public decimal CurrentBalance { get; set; } = 0;

        /// <summary>
        /// تاريخ آخر حركة
        /// </summary>
        public DateTime? LastTransactionDate { get; set; }

        /// <summary>
        /// وصف الحساب
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// قيود اليومية المرتبطة بهذا الحساب
        /// </summary>
        public virtual ICollection<JournalEntryDetail> JournalEntryDetails { get; set; } = new List<JournalEntryDetail>();

        /// <summary>
        /// الحصول على المسار الكامل للحساب
        /// </summary>
        public string GetFullPath()
        {
            if (ParentAccount == null)
                return AccountNameAr;
            
            return $"{ParentAccount.GetFullPath()} > {AccountNameAr}";
        }

        /// <summary>
        /// الحصول على الكود الكامل للحساب
        /// </summary>
        public string GetFullCode()
        {
            if (ParentAccount == null)
                return AccountCode;
            
            return $"{ParentAccount.GetFullCode()}.{AccountCode}";
        }

        /// <summary>
        /// حساب الرصيد الحالي
        /// </summary>
        public decimal CalculateCurrentBalance()
        {
            decimal balance = OpeningBalance;
            
            foreach (var detail in JournalEntryDetails.Where(d => d.JournalEntry.Status == JournalEntryStatus.Posted))
            {
                if (AccountNature == AccountNature.Debit)
                {
                    balance += detail.DebitAmount - detail.CreditAmount;
                }
                else
                {
                    balance += detail.CreditAmount - detail.DebitAmount;
                }
            }
            
            return balance;
        }
    }
}
