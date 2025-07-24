namespace AlMohasebAlAraby.Shared.Enums
{
    /// <summary>
    /// أنواع الحسابات في دليل الحسابات
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// الأصول
        /// </summary>
        Assets = 1,

        /// <summary>
        /// الخصوم
        /// </summary>
        Liabilities = 2,

        /// <summary>
        /// حقوق الملكية
        /// </summary>
        Equity = 3,

        /// <summary>
        /// الإيرادات
        /// </summary>
        Revenue = 4,

        /// <summary>
        /// المصروفات
        /// </summary>
        Expenses = 5
    }

    /// <summary>
    /// طبيعة الحساب (مدين أو دائن)
    /// </summary>
    public enum AccountNature
    {
        /// <summary>
        /// مدين
        /// </summary>
        Debit = 1,

        /// <summary>
        /// دائن
        /// </summary>
        Credit = 2
    }

    /// <summary>
    /// حالة القيد المحاسبي
    /// </summary>
    public enum JournalEntryStatus
    {
        /// <summary>
        /// مسودة
        /// </summary>
        Draft = 1,

        /// <summary>
        /// معتمد
        /// </summary>
        Approved = 2,

        /// <summary>
        /// مرحل
        /// </summary>
        Posted = 3,

        /// <summary>
        /// ملغي
        /// </summary>
        Cancelled = 4
    }

    /// <summary>
    /// نوع المستند
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// قيد يومية عام
        /// </summary>
        GeneralJournal = 1,

        /// <summary>
        /// فاتورة مبيعات
        /// </summary>
        SalesInvoice = 2,

        /// <summary>
        /// فاتورة مشتريات
        /// </summary>
        PurchaseInvoice = 3,

        /// <summary>
        /// سند قبض
        /// </summary>
        Receipt = 4,

        /// <summary>
        /// سند صرف
        /// </summary>
        Payment = 5,

        /// <summary>
        /// قيد افتتاحي
        /// </summary>
        OpeningEntry = 6,

        /// <summary>
        /// قيد إقفال
        /// </summary>
        ClosingEntry = 7
    }

    /// <summary>
    /// العملات المدعومة
    /// </summary>
    public enum Currency
    {
        /// <summary>
        /// الجنيه المصري
        /// </summary>
        EGP = 1,

        /// <summary>
        /// الدولار الأمريكي
        /// </summary>
        USD = 2,

        /// <summary>
        /// اليورو
        /// </summary>
        EUR = 3,

        /// <summary>
        /// الريال السعودي
        /// </summary>
        SAR = 4,

        /// <summary>
        /// الدرهم الإماراتي
        /// </summary>
        AED = 5
    }

    /// <summary>
    /// أنواع التقارير المالية
    /// </summary>
    public enum FinancialReportType
    {
        /// <summary>
        /// الميزانية العمومية
        /// </summary>
        BalanceSheet = 1,

        /// <summary>
        /// قائمة الدخل
        /// </summary>
        IncomeStatement = 2,

        /// <summary>
        /// قائمة التدفقات النقدية
        /// </summary>
        CashFlowStatement = 3,

        /// <summary>
        /// ميزان المراجعة
        /// </summary>
        TrialBalance = 4,

        /// <summary>
        /// دفتر الأستاذ العام
        /// </summary>
        GeneralLedger = 5,

        /// <summary>
        /// كشف حساب
        /// </summary>
        AccountStatement = 6
    }

    /// <summary>
    /// أدوار المستخدمين
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// مدير النظام
        /// </summary>
        Admin = 1,

        /// <summary>
        /// محاسب
        /// </summary>
        Accountant = 2,

        /// <summary>
        /// كاشير
        /// </summary>
        Cashier = 3,

        /// <summary>
        /// مشرف
        /// </summary>
        Supervisor = 4,

        /// <summary>
        /// مستخدم عادي
        /// </summary>
        User = 5
    }

    /// <summary>
    /// حالة الفاتورة
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// مسودة
        /// </summary>
        Draft = 1,

        /// <summary>
        /// معتمدة
        /// </summary>
        Approved = 2,

        /// <summary>
        /// مدفوعة جزئياً
        /// </summary>
        PartiallyPaid = 3,

        /// <summary>
        /// مدفوعة بالكامل
        /// </summary>
        FullyPaid = 4,

        /// <summary>
        /// ملغاة
        /// </summary>
        Cancelled = 5,

        /// <summary>
        /// مرتجعة
        /// </summary>
        Returned = 6
    }

    /// <summary>
    /// طرق الدفع
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// نقدي
        /// </summary>
        Cash = 1,

        /// <summary>
        /// شيك
        /// </summary>
        Check = 2,

        /// <summary>
        /// تحويل بنكي
        /// </summary>
        BankTransfer = 3,

        /// <summary>
        /// بطاقة ائتمان
        /// </summary>
        CreditCard = 4,

        /// <summary>
        /// محفظة إلكترونية
        /// </summary>
        EWallet = 5,

        /// <summary>
        /// آجل
        /// </summary>
        Credit = 6
    }
}
