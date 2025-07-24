using AlMohasebAlAraby.Shared.Models.Accounting;

namespace AlMohasebAlAraby.Desktop.Services
{
    /// <summary>
    /// واجهة خدمة المحاسبة
    /// </summary>
    public interface IAccountingService
    {
        #region Chart of Accounts
        /// <summary>
        /// الحصول على جميع الحسابات
        /// </summary>
        Task<List<ChartOfAccounts>> GetAllAccountsAsync();

        /// <summary>
        /// الحصول على حساب بالمعرف
        /// </summary>
        Task<ChartOfAccounts?> GetAccountByIdAsync(int id);

        /// <summary>
        /// الحصول على حساب بالكود
        /// </summary>
        Task<ChartOfAccounts?> GetAccountByCodeAsync(string code);

        /// <summary>
        /// إضافة حساب جديد
        /// </summary>
        Task<int> AddAccountAsync(ChartOfAccounts account);

        /// <summary>
        /// تحديث حساب
        /// </summary>
        Task<bool> UpdateAccountAsync(ChartOfAccounts account);

        /// <summary>
        /// حذف حساب
        /// </summary>
        Task<bool> DeleteAccountAsync(int id);

        /// <summary>
        /// الحصول على الحسابات الفرعية
        /// </summary>
        Task<List<ChartOfAccounts>> GetSubAccountsAsync(int parentId);

        /// <summary>
        /// الحصول على الحسابات القابلة للترحيل
        /// </summary>
        Task<List<ChartOfAccounts>> GetPostableAccountsAsync();
        #endregion

        #region Journal Entries
        /// <summary>
        /// الحصول على جميع قيود اليومية
        /// </summary>
        Task<List<JournalEntry>> GetAllJournalEntriesAsync();

        /// <summary>
        /// الحصول على قيد يومية بالمعرف
        /// </summary>
        Task<JournalEntry?> GetJournalEntryByIdAsync(int id);

        /// <summary>
        /// الحصول على قيود اليومية بالفترة
        /// </summary>
        Task<List<JournalEntry>> GetJournalEntriesByPeriodAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// إضافة قيد يومية جديد
        /// </summary>
        Task<int> AddJournalEntryAsync(JournalEntry entry);

        /// <summary>
        /// تحديث قيد يومية
        /// </summary>
        Task<bool> UpdateJournalEntryAsync(JournalEntry entry);

        /// <summary>
        /// حذف قيد يومية
        /// </summary>
        Task<bool> DeleteJournalEntryAsync(int id);

        /// <summary>
        /// اعتماد قيد يومية
        /// </summary>
        Task<bool> ApproveJournalEntryAsync(int id, int approvedBy);

        /// <summary>
        /// ترحيل قيد يومية
        /// </summary>
        Task<bool> PostJournalEntryAsync(int id, int postedBy);

        /// <summary>
        /// إلغاء قيد يومية
        /// </summary>
        Task<bool> CancelJournalEntryAsync(int id);

        /// <summary>
        /// الحصول على رقم القيد التالي
        /// </summary>
        Task<string> GetNextJournalEntryNumberAsync();
        #endregion

        #region Reports
        /// <summary>
        /// تقرير ميزان المراجعة
        /// </summary>
        Task<List<TrialBalanceItem>> GetTrialBalanceAsync(DateTime asOfDate);

        /// <summary>
        /// تقرير دفتر الأستاذ العام
        /// </summary>
        Task<List<GeneralLedgerItem>> GetGeneralLedgerAsync(int accountId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// تقرير الميزانية العمومية
        /// </summary>
        Task<BalanceSheetReport> GetBalanceSheetAsync(DateTime asOfDate);

        /// <summary>
        /// تقرير قائمة الدخل
        /// </summary>
        Task<IncomeStatementReport> GetIncomeStatementAsync(DateTime fromDate, DateTime toDate);
        #endregion

        #region Account Balances
        /// <summary>
        /// حساب رصيد حساب
        /// </summary>
        Task<decimal> CalculateAccountBalanceAsync(int accountId, DateTime? asOfDate = null);

        /// <summary>
        /// تحديث أرصدة الحسابات
        /// </summary>
        Task UpdateAccountBalancesAsync();

        /// <summary>
        /// الحصول على حركة حساب
        /// </summary>
        Task<List<AccountMovement>> GetAccountMovementsAsync(int accountId, DateTime fromDate, DateTime toDate);
        #endregion
    }

    /// <summary>
    /// عنصر ميزان المراجعة
    /// </summary>
    public class TrialBalanceItem
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
    }

    /// <summary>
    /// عنصر دفتر الأستاذ العام
    /// </summary>
    public class GeneralLedgerItem
    {
        public DateTime Date { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// تقرير الميزانية العمومية
    /// </summary>
    public class BalanceSheetReport
    {
        public DateTime AsOfDate { get; set; }
        public List<BalanceSheetItem> Assets { get; set; } = new();
        public List<BalanceSheetItem> Liabilities { get; set; } = new();
        public List<BalanceSheetItem> Equity { get; set; } = new();
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
    }

    /// <summary>
    /// عنصر الميزانية العمومية
    /// </summary>
    public class BalanceSheetItem
    {
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// تقرير قائمة الدخل
    /// </summary>
    public class IncomeStatementReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<IncomeStatementItem> Revenues { get; set; } = new();
        public List<IncomeStatementItem> Expenses { get; set; } = new();
        public decimal TotalRevenues { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
    }

    /// <summary>
    /// عنصر قائمة الدخل
    /// </summary>
    public class IncomeStatementItem
    {
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// حركة حساب
    /// </summary>
    public class AccountMovement
    {
        public DateTime Date { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
