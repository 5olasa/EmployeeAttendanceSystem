using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AlMohasebAlAraby.Shared.Data;
using AlMohasebAlAraby.Shared.Models.Accounting;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Desktop.Services
{
    /// <summary>
    /// خدمة المحاسبة
    /// </summary>
    public class AccountingService : IAccountingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountingService> _logger;

        public AccountingService(ApplicationDbContext context, ILogger<AccountingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Chart of Accounts
        public async Task<List<ChartOfAccounts>> GetAllAccountsAsync()
        {
            try
            {
                return await _context.ChartOfAccounts
                    .Include(a => a.ParentAccount)
                    .Include(a => a.SubAccounts)
                    .Where(a => !a.IsDeleted)
                    .OrderBy(a => a.AccountCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على جميع الحسابات");
                throw;
            }
        }

        public async Task<ChartOfAccounts?> GetAccountByIdAsync(int id)
        {
            try
            {
                return await _context.ChartOfAccounts
                    .Include(a => a.ParentAccount)
                    .Include(a => a.SubAccounts)
                    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على الحساب بالمعرف {Id}", id);
                throw;
            }
        }

        public async Task<ChartOfAccounts?> GetAccountByCodeAsync(string code)
        {
            try
            {
                return await _context.ChartOfAccounts
                    .Include(a => a.ParentAccount)
                    .Include(a => a.SubAccounts)
                    .FirstOrDefaultAsync(a => a.AccountCode == code && !a.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على الحساب بالكود {Code}", code);
                throw;
            }
        }

        public async Task<int> AddAccountAsync(ChartOfAccounts account)
        {
            try
            {
                // التحقق من عدم تكرار الكود
                var existingAccount = await GetAccountByCodeAsync(account.AccountCode);
                if (existingAccount != null)
                {
                    throw new InvalidOperationException($"كود الحساب {account.AccountCode} موجود مسبقاً");
                }

                account.CreatedDate = DateTime.Now;
                account.LastModifiedDate = DateTime.Now;

                _context.ChartOfAccounts.Add(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم إضافة حساب جديد: {AccountName} - {AccountCode}", account.AccountNameAr, account.AccountCode);
                return account.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة الحساب {AccountName}", account.AccountNameAr);
                throw;
            }
        }

        public async Task<bool> UpdateAccountAsync(ChartOfAccounts account)
        {
            try
            {
                var existingAccount = await GetAccountByIdAsync(account.Id);
                if (existingAccount == null)
                {
                    return false;
                }

                // التحقق من عدم تكرار الكود مع حساب آخر
                var duplicateAccount = await _context.ChartOfAccounts
                    .FirstOrDefaultAsync(a => a.AccountCode == account.AccountCode && a.Id != account.Id && !a.IsDeleted);
                if (duplicateAccount != null)
                {
                    throw new InvalidOperationException($"كود الحساب {account.AccountCode} موجود مسبقاً");
                }

                existingAccount.AccountCode = account.AccountCode;
                existingAccount.AccountNameAr = account.AccountNameAr;
                existingAccount.AccountNameEn = account.AccountNameEn;
                existingAccount.AccountType = account.AccountType;
                existingAccount.AccountNature = account.AccountNature;
                existingAccount.ParentAccountId = account.ParentAccountId;
                existingAccount.IsPostable = account.IsPostable;
                existingAccount.IsCustomerAccount = account.IsCustomerAccount;
                existingAccount.IsSupplierAccount = account.IsSupplierAccount;
                existingAccount.IsBankAccount = account.IsBankAccount;
                existingAccount.IsCashAccount = account.IsCashAccount;
                existingAccount.DefaultCurrency = account.DefaultCurrency;
                existingAccount.Description = account.Description;
                existingAccount.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث الحساب: {AccountName} - {AccountCode}", account.AccountNameAr, account.AccountCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث الحساب {AccountId}", account.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await GetAccountByIdAsync(id);
                if (account == null)
                {
                    return false;
                }

                // التحقق من عدم وجود حسابات فرعية
                var hasSubAccounts = await _context.ChartOfAccounts
                    .AnyAsync(a => a.ParentAccountId == id && !a.IsDeleted);
                if (hasSubAccounts)
                {
                    throw new InvalidOperationException("لا يمكن حذف الحساب لوجود حسابات فرعية");
                }

                // التحقق من عدم وجود حركات على الحساب
                var hasMovements = await _context.JournalEntryDetails
                    .AnyAsync(d => d.AccountId == id);
                if (hasMovements)
                {
                    throw new InvalidOperationException("لا يمكن حذف الحساب لوجود حركات عليه");
                }

                account.IsDeleted = true;
                account.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف الحساب: {AccountName} - {AccountCode}", account.AccountNameAr, account.AccountCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف الحساب {AccountId}", id);
                throw;
            }
        }

        public async Task<List<ChartOfAccounts>> GetSubAccountsAsync(int parentId)
        {
            try
            {
                return await _context.ChartOfAccounts
                    .Where(a => a.ParentAccountId == parentId && !a.IsDeleted)
                    .OrderBy(a => a.AccountCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على الحسابات الفرعية للحساب {ParentId}", parentId);
                throw;
            }
        }

        public async Task<List<ChartOfAccounts>> GetPostableAccountsAsync()
        {
            try
            {
                return await _context.ChartOfAccounts
                    .Where(a => a.IsPostable && !a.IsDeleted)
                    .OrderBy(a => a.AccountCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على الحسابات القابلة للترحيل");
                throw;
            }
        }
        #endregion

        #region Journal Entries
        public async Task<List<JournalEntry>> GetAllJournalEntriesAsync()
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.Details)
                        .ThenInclude(d => d.Account)
                    .Where(e => !e.IsDeleted)
                    .OrderByDescending(e => e.EntryDate)
                    .ThenByDescending(e => e.EntryNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على جميع قيود اليومية");
                throw;
            }
        }

        public async Task<JournalEntry?> GetJournalEntryByIdAsync(int id)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.Details)
                        .ThenInclude(d => d.Account)
                    .Include(e => e.Attachments)
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على قيد اليومية بالمعرف {Id}", id);
                throw;
            }
        }

        public async Task<List<JournalEntry>> GetJournalEntriesByPeriodAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await _context.JournalEntries
                    .Include(e => e.Details)
                        .ThenInclude(d => d.Account)
                    .Where(e => e.EntryDate >= fromDate && e.EntryDate <= toDate && !e.IsDeleted)
                    .OrderBy(e => e.EntryDate)
                    .ThenBy(e => e.EntryNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على قيود اليومية للفترة من {FromDate} إلى {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<int> AddJournalEntryAsync(JournalEntry entry)
        {
            try
            {
                // التحقق من صحة القيد
                var validationErrors = entry.ValidateEntry();
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"القيد غير صحيح: {string.Join(", ", validationErrors)}");
                }

                // إنشاء رقم القيد إذا لم يكن موجوداً
                if (string.IsNullOrEmpty(entry.EntryNumber))
                {
                    entry.EntryNumber = await GetNextJournalEntryNumberAsync();
                }

                entry.CreatedDate = DateTime.Now;
                entry.LastModifiedDate = DateTime.Now;

                // حساب الإجماليات
                entry.CalculateTotals();

                _context.JournalEntries.Add(entry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("تم إضافة قيد يومية جديد: {EntryNumber}", entry.EntryNumber);
                return entry.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة قيد اليومية {EntryNumber}", entry.EntryNumber);
                throw;
            }
        }

        public async Task<bool> UpdateJournalEntryAsync(JournalEntry entry)
        {
            try
            {
                var existingEntry = await GetJournalEntryByIdAsync(entry.Id);
                if (existingEntry == null)
                {
                    return false;
                }

                // التحقق من إمكانية التعديل
                if (existingEntry.Status == JournalEntryStatus.Posted)
                {
                    throw new InvalidOperationException("لا يمكن تعديل قيد مرحل");
                }

                // التحقق من صحة القيد
                var validationErrors = entry.ValidateEntry();
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"القيد غير صحيح: {string.Join(", ", validationErrors)}");
                }

                // تحديث البيانات
                existingEntry.EntryDate = entry.EntryDate;
                existingEntry.Description = entry.Description;
                existingEntry.ReferenceNumber = entry.ReferenceNumber;
                existingEntry.Currency = entry.Currency;
                existingEntry.ExchangeRate = entry.ExchangeRate;
                existingEntry.LastModifiedDate = DateTime.Now;

                // حذف التفاصيل القديمة وإضافة الجديدة
                _context.JournalEntryDetails.RemoveRange(existingEntry.Details);
                existingEntry.Details = entry.Details;

                // حساب الإجماليات
                existingEntry.CalculateTotals();

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم تحديث قيد اليومية: {EntryNumber}", entry.EntryNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث قيد اليومية {EntryId}", entry.Id);
                throw;
            }
        }

        public async Task<bool> DeleteJournalEntryAsync(int id)
        {
            try
            {
                var entry = await GetJournalEntryByIdAsync(id);
                if (entry == null)
                {
                    return false;
                }

                // التحقق من إمكانية الحذف
                if (entry.Status == JournalEntryStatus.Posted)
                {
                    throw new InvalidOperationException("لا يمكن حذف قيد مرحل");
                }

                entry.IsDeleted = true;
                entry.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم حذف قيد اليومية: {EntryNumber}", entry.EntryNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في حذف قيد اليومية {EntryId}", id);
                throw;
            }
        }

        public async Task<bool> ApproveJournalEntryAsync(int id, int approvedBy)
        {
            try
            {
                var entry = await GetJournalEntryByIdAsync(id);
                if (entry == null)
                {
                    return false;
                }

                if (entry.Status != JournalEntryStatus.Draft)
                {
                    throw new InvalidOperationException("يمكن اعتماد المسودات فقط");
                }

                entry.Status = JournalEntryStatus.Approved;
                entry.ApprovedDate = DateTime.Now;
                entry.ApprovedBy = approvedBy;
                entry.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم اعتماد قيد اليومية: {EntryNumber}", entry.EntryNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في اعتماد قيد اليومية {EntryId}", id);
                throw;
            }
        }

        public async Task<bool> PostJournalEntryAsync(int id, int postedBy)
        {
            try
            {
                var entry = await GetJournalEntryByIdAsync(id);
                if (entry == null)
                {
                    return false;
                }

                if (entry.Status != JournalEntryStatus.Approved)
                {
                    throw new InvalidOperationException("يجب اعتماد القيد قبل الترحيل");
                }

                entry.Status = JournalEntryStatus.Posted;
                entry.PostedDate = DateTime.Now;
                entry.PostedBy = postedBy;
                entry.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // تحديث أرصدة الحسابات
                await UpdateAccountBalancesForEntryAsync(entry);

                _logger.LogInformation("تم ترحيل قيد اليومية: {EntryNumber}", entry.EntryNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في ترحيل قيد اليومية {EntryId}", id);
                throw;
            }
        }

        public async Task<bool> CancelJournalEntryAsync(int id)
        {
            try
            {
                var entry = await GetJournalEntryByIdAsync(id);
                if (entry == null)
                {
                    return false;
                }

                if (entry.Status == JournalEntryStatus.Posted)
                {
                    throw new InvalidOperationException("لا يمكن إلغاء قيد مرحل");
                }

                entry.Status = JournalEntryStatus.Cancelled;
                entry.LastModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("تم إلغاء قيد اليومية: {EntryNumber}", entry.EntryNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إلغاء قيد اليومية {EntryId}", id);
                throw;
            }
        }

        public async Task<string> GetNextJournalEntryNumberAsync()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var lastEntry = await _context.JournalEntries
                    .Where(e => e.EntryDate.Year == currentYear)
                    .OrderByDescending(e => e.EntryNumber)
                    .FirstOrDefaultAsync();

                if (lastEntry == null)
                {
                    return $"JE{currentYear}0001";
                }

                // استخراج الرقم من آخر قيد
                var lastNumber = lastEntry.EntryNumber.Substring(6); // إزالة "JE" والسنة
                if (int.TryParse(lastNumber, out int number))
                {
                    return $"JE{currentYear}{(number + 1):D4}";
                }

                return $"JE{currentYear}0001";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في الحصول على رقم القيد التالي");
                throw;
            }
        }
        #endregion

        #region Private Methods
        private async Task UpdateAccountBalancesForEntryAsync(JournalEntry entry)
        {
            try
            {
                foreach (var detail in entry.Details)
                {
                    var account = await GetAccountByIdAsync(detail.AccountId);
                    if (account != null)
                    {
                        if (account.AccountNature == AccountNature.Debit)
                        {
                            account.CurrentBalance += detail.DebitAmount - detail.CreditAmount;
                        }
                        else
                        {
                            account.CurrentBalance += detail.CreditAmount - detail.DebitAmount;
                        }

                        account.LastTransactionDate = entry.EntryDate;
                        account.LastModifiedDate = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحديث أرصدة الحسابات للقيد {EntryId}", entry.Id);
                throw;
            }
        }
        #endregion

        #region Reports - سيتم إكمالها في الجزء التالي
        public async Task<List<TrialBalanceItem>> GetTrialBalanceAsync(DateTime asOfDate)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return new List<TrialBalanceItem>();
        }

        public async Task<List<GeneralLedgerItem>> GetGeneralLedgerAsync(int accountId, DateTime fromDate, DateTime toDate)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return new List<GeneralLedgerItem>();
        }

        public async Task<BalanceSheetReport> GetBalanceSheetAsync(DateTime asOfDate)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return new BalanceSheetReport();
        }

        public async Task<IncomeStatementReport> GetIncomeStatementAsync(DateTime fromDate, DateTime toDate)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return new IncomeStatementReport();
        }

        public async Task<decimal> CalculateAccountBalanceAsync(int accountId, DateTime? asOfDate = null)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return 0;
        }

        public async Task UpdateAccountBalancesAsync()
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
        }

        public async Task<List<AccountMovement>> GetAccountMovementsAsync(int accountId, DateTime fromDate, DateTime toDate)
        {
            // سيتم تنفيذها لاحقاً
            await Task.CompletedTask;
            return new List<AccountMovement>();
        }
        #endregion
    }
}
