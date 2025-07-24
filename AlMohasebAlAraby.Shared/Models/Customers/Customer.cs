using System.ComponentModel.DataAnnotations;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Shared.Models.Customers
{
    /// <summary>
    /// العملاء
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// كود العميل
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CustomerCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم العميل
        /// </summary>
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// اسم العميل باللغة الإنجليزية
        /// </summary>
        [StringLength(200)]
        public string? CustomerNameEn { get; set; }

        /// <summary>
        /// نوع العميل (فرد/شركة)
        /// </summary>
        public CustomerType CustomerType { get; set; } = CustomerType.Individual;

        /// <summary>
        /// الرقم القومي أو السجل التجاري
        /// </summary>
        [StringLength(50)]
        public string? NationalId { get; set; }

        /// <summary>
        /// الرقم الضريبي
        /// </summary>
        [StringLength(50)]
        public string? TaxNumber { get; set; }

        /// <summary>
        /// رقم الهاتف الأساسي
        /// </summary>
        [StringLength(20)]
        public string? Phone1 { get; set; }

        /// <summary>
        /// رقم الهاتف الثانوي
        /// </summary>
        [StringLength(20)]
        public string? Phone2 { get; set; }

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// العنوان
        /// </summary>
        [StringLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// المدينة
        /// </summary>
        [StringLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// المحافظة
        /// </summary>
        [StringLength(100)]
        public string? Governorate { get; set; }

        /// <summary>
        /// الرمز البريدي
        /// </summary>
        [StringLength(20)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// الدولة
        /// </summary>
        [StringLength(100)]
        public string? Country { get; set; } = "مصر";

        /// <summary>
        /// حد الائتمان
        /// </summary>
        public decimal CreditLimit { get; set; } = 0;

        /// <summary>
        /// فترة السداد بالأيام
        /// </summary>
        public int PaymentTermDays { get; set; } = 0;

        /// <summary>
        /// طريقة الدفع المفضلة
        /// </summary>
        public PaymentMethod PreferredPaymentMethod { get; set; } = PaymentMethod.Cash;

        /// <summary>
        /// نسبة الخصم الافتراضية
        /// </summary>
        public decimal DefaultDiscountPercentage { get; set; } = 0;

        /// <summary>
        /// الرصيد الحالي
        /// </summary>
        public decimal CurrentBalance { get; set; } = 0;

        /// <summary>
        /// إجمالي المبيعات
        /// </summary>
        public decimal TotalSales { get; set; } = 0;

        /// <summary>
        /// تاريخ آخر عملية شراء
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// معرف الحساب المحاسبي
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// الحساب المحاسبي
        /// </summary>
        public virtual Accounting.ChartOfAccounts? Account { get; set; }

        /// <summary>
        /// معرف مندوب المبيعات
        /// </summary>
        public int? SalesRepresentativeId { get; set; }

        /// <summary>
        /// مندوب المبيعات
        /// </summary>
        public virtual HR.Employee? SalesRepresentative { get; set; }

        /// <summary>
        /// معرف مجموعة العملاء
        /// </summary>
        public int? CustomerGroupId { get; set; }

        /// <summary>
        /// مجموعة العملاء
        /// </summary>
        public virtual CustomerGroup? CustomerGroup { get; set; }

        /// <summary>
        /// فواتير المبيعات
        /// </summary>
        public virtual ICollection<Sales.SalesInvoice> SalesInvoices { get; set; } = new List<Sales.SalesInvoice>();

        /// <summary>
        /// عقود التقسيط
        /// </summary>
        public virtual ICollection<Installments.InstallmentContract> InstallmentContracts { get; set; } = new List<Installments.InstallmentContract>();

        /// <summary>
        /// جهات الاتصال
        /// </summary>
        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();

        /// <summary>
        /// التحقق من توفر حد الائتمان
        /// </summary>
        public bool HasAvailableCredit(decimal amount)
        {
            return (CurrentBalance + amount) <= CreditLimit;
        }

        /// <summary>
        /// حساب الرصيد المتاح
        /// </summary>
        public decimal GetAvailableCredit()
        {
            return Math.Max(0, CreditLimit - CurrentBalance);
        }

        /// <summary>
        /// التحقق من تجاوز فترة السداد
        /// </summary>
        public bool IsOverdue()
        {
            return LastPurchaseDate.HasValue && 
                   DateTime.Now.Subtract(LastPurchaseDate.Value).Days > PaymentTermDays &&
                   CurrentBalance > 0;
        }
    }

    /// <summary>
    /// أنواع العملاء
    /// </summary>
    public enum CustomerType
    {
        /// <summary>
        /// فرد
        /// </summary>
        Individual = 1,

        /// <summary>
        /// شركة
        /// </summary>
        Company = 2
    }

    /// <summary>
    /// مجموعات العملاء
    /// </summary>
    public class CustomerGroup : BaseEntity
    {
        /// <summary>
        /// كود المجموعة
        /// </summary>
        [Required]
        [StringLength(20)]
        public string GroupCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم المجموعة
        /// </summary>
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// وصف المجموعة
        /// </summary>
        [StringLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// نسبة الخصم الافتراضية للمجموعة
        /// </summary>
        public decimal DefaultDiscountPercentage { get; set; } = 0;

        /// <summary>
        /// حد الائتمان الافتراضي للمجموعة
        /// </summary>
        public decimal DefaultCreditLimit { get; set; } = 0;

        /// <summary>
        /// فترة السداد الافتراضية بالأيام
        /// </summary>
        public int DefaultPaymentTermDays { get; set; } = 0;

        /// <summary>
        /// العملاء في هذه المجموعة
        /// </summary>
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }

    /// <summary>
    /// جهات اتصال العملاء
    /// </summary>
    public class CustomerContact : BaseEntity
    {
        /// <summary>
        /// معرف العميل
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// العميل
        /// </summary>
        public virtual Customer Customer { get; set; } = null!;

        /// <summary>
        /// اسم جهة الاتصال
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ContactName { get; set; } = string.Empty;

        /// <summary>
        /// المنصب
        /// </summary>
        [StringLength(100)]
        public string? Position { get; set; }

        /// <summary>
        /// رقم الهاتف
        /// </summary>
        [StringLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// هل هو جهة الاتصال الأساسية
        /// </summary>
        public bool IsPrimary { get; set; } = false;
    }
}
