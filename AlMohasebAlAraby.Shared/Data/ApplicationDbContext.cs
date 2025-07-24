using Microsoft.EntityFrameworkCore;
using AlMohasebAlAraby.Shared.Models;
using AlMohasebAlAraby.Shared.Models.Accounting;
using AlMohasebAlAraby.Shared.Models.Inventory;
using AlMohasebAlAraby.Shared.Models.Customers;
using AlMohasebAlAraby.Shared.Models.Suppliers;
using AlMohasebAlAraby.Shared.Models.Sales;
using AlMohasebAlAraby.Shared.Models.Purchase;
using AlMohasebAlAraby.Shared.Models.Installments;
using AlMohasebAlAraby.Shared.Models.Pharmacy;
using AlMohasebAlAraby.Shared.Models.HR;
using AlMohasebAlAraby.Shared.Models.Settings;

namespace AlMohasebAlAraby.Shared.Data
{
    /// <summary>
    /// سياق قاعدة البيانات الرئيسي للتطبيق
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region Accounting Tables
        /// <summary>
        /// دليل الحسابات
        /// </summary>
        public DbSet<ChartOfAccounts> ChartOfAccounts { get; set; }

        /// <summary>
        /// قيود اليومية
        /// </summary>
        public DbSet<JournalEntry> JournalEntries { get; set; }

        /// <summary>
        /// تفاصيل قيود اليومية
        /// </summary>
        public DbSet<JournalEntryDetail> JournalEntryDetails { get; set; }

        /// <summary>
        /// مرفقات قيود اليومية
        /// </summary>
        public DbSet<JournalEntryAttachment> JournalEntryAttachments { get; set; }
        #endregion

        #region Inventory Tables
        /// <summary>
        /// تصنيفات الأصناف
        /// </summary>
        public DbSet<ProductCategory> ProductCategories { get; set; }

        /// <summary>
        /// وحدات القياس
        /// </summary>
        public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }

        /// <summary>
        /// الأصناف
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// تحويلات وحدات القياس
        /// </summary>
        public DbSet<ProductUnitConversion> ProductUnitConversions { get; set; }
        #endregion

        #region Customer & Supplier Tables
        /// <summary>
        /// مجموعات العملاء
        /// </summary>
        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        /// <summary>
        /// العملاء
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// جهات اتصال العملاء
        /// </summary>
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        #endregion

        #region Installment Tables
        /// <summary>
        /// عقود التقسيط
        /// </summary>
        public DbSet<InstallmentContract> InstallmentContracts { get; set; }
        #endregion

        #region Pharmacy Tables
        /// <summary>
        /// الأدوية
        /// </summary>
        public DbSet<Medicine> Medicines { get; set; }
        #endregion

        #region System Tables
        /// <summary>
        /// المستخدمين
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// إعدادات الشركة
        /// </summary>
        public DbSet<CompanySettings> CompanySettings { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تكوين العلاقات والقيود
            ConfigureAccountingEntities(modelBuilder);
            ConfigureInventoryEntities(modelBuilder);
            ConfigureCustomerEntities(modelBuilder);
            ConfigureInstallmentEntities(modelBuilder);
            ConfigurePharmacyEntities(modelBuilder);
            ConfigureSystemEntities(modelBuilder);

            // إضافة البيانات الأولية
            SeedInitialData(modelBuilder);
        }

        private void ConfigureAccountingEntities(ModelBuilder modelBuilder)
        {
            // تكوين دليل الحسابات
            modelBuilder.Entity<ChartOfAccounts>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.AccountNameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.AccountNameEn).HasMaxLength(200);
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18,2)");

                // العلاقة الذاتية للحسابات الفرعية
                entity.HasOne(e => e.ParentAccount)
                      .WithMany(e => e.SubAccounts)
                      .HasForeignKey(e => e.ParentAccountId)
                      .OnDelete(DeleteBehavior.Restrict);

                // فهرس فريد على كود الحساب
                entity.HasIndex(e => e.AccountCode).IsUnique();
            });

            // تكوين قيود اليومية
            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TotalDebitAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCreditAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18,4)");

                // فهرس فريد على رقم القيد
                entity.HasIndex(e => e.EntryNumber).IsUnique();
            });

            // تكوين تفاصيل قيود اليومية
            modelBuilder.Entity<JournalEntryDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DebitAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreditAmount).HasColumnType("decimal(18,2)");

                // العلاقة مع قيد اليومية
                entity.HasOne(e => e.JournalEntry)
                      .WithMany(e => e.Details)
                      .HasForeignKey(e => e.JournalEntryId)
                      .OnDelete(DeleteBehavior.Cascade);

                // العلاقة مع الحساب
                entity.HasOne(e => e.Account)
                      .WithMany(e => e.JournalEntryDetails)
                      .HasForeignKey(e => e.AccountId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureInventoryEntities(ModelBuilder modelBuilder)
        {
            // تكوين تصنيفات الأصناف
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CategoryCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CategoryNameAr).IsRequired().HasMaxLength(100);

                // العلاقة الذاتية للتصنيفات الفرعية
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // فهرس فريد على كود التصنيف
                entity.HasIndex(e => e.CategoryCode).IsUnique();
            });

            // تكوين الأصناف
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProductNameAr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SellingPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CurrentStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MinimumStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MaximumStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReorderPoint).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReservedStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.TaxRate).HasColumnType("decimal(5,2)");

                // العلاقة مع التصنيف
                entity.HasOne(e => e.Category)
                      .WithMany(e => e.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // العلاقة مع وحدة القياس
                entity.HasOne(e => e.UnitOfMeasure)
                      .WithMany(e => e.Products)
                      .HasForeignKey(e => e.UnitOfMeasureId)
                      .OnDelete(DeleteBehavior.Restrict);

                // فهرس فريد على كود الصنف
                entity.HasIndex(e => e.ProductCode).IsUnique();
            });
        }

        private void ConfigureCustomerEntities(ModelBuilder modelBuilder)
        {
            // تكوين العملاء
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalSales).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DefaultDiscountPercentage).HasColumnType("decimal(5,2)");

                // العلاقة مع الحساب المحاسبي
                entity.HasOne(e => e.Account)
                      .WithMany()
                      .HasForeignKey(e => e.AccountId)
                      .OnDelete(DeleteBehavior.SetNull);

                // العلاقة مع مجموعة العملاء
                entity.HasOne(e => e.CustomerGroup)
                      .WithMany(e => e.Customers)
                      .HasForeignKey(e => e.CustomerGroupId)
                      .OnDelete(DeleteBehavior.SetNull);

                // فهرس فريد على كود العميل
                entity.HasIndex(e => e.CustomerCode).IsUnique();
            });
        }

        private void ConfigureInstallmentEntities(ModelBuilder modelBuilder)
        {
            // تكوين عقود التقسيط
            modelBuilder.Entity<InstallmentContract>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DownPayment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");
                entity.Property(e => e.MonthlyInstallmentAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPaidAmount).HasColumnType("decimal(18,2)");

                // العلاقة مع العميل
                entity.HasOne(e => e.Customer)
                      .WithMany(e => e.InstallmentContracts)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // فهرس فريد على رقم العقد
                entity.HasIndex(e => e.ContractNumber).IsUnique();
            });
        }

        private void ConfigurePharmacyEntities(ModelBuilder modelBuilder)
        {
            // تكوين الأدوية
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MedicineCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TradeName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ScientificName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Manufacturer).IsRequired().HasMaxLength(200);
                entity.Property(e => e.OfficialPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PublicPrice).HasColumnType("decimal(18,2)");

                // فهرس فريد على كود الدواء
                entity.HasIndex(e => e.MedicineCode).IsUnique();
            });
        }

        private void ConfigureSystemEntities(ModelBuilder modelBuilder)
        {
            // تكوين المستخدمين
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);

                // فهرس فريد على اسم المستخدم
                entity.HasIndex(e => e.Username).IsUnique();
            });
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // إضافة المستخدم الافتراضي
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "مدير النظام",
                    Email = "admin@almohaseb.com",
                    Role = Enums.UserRole.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                }
            );

            // إضافة وحدات القياس الأساسية
            modelBuilder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure { Id = 1, UnitCode = "قطعة", UnitNameAr = "قطعة", UnitNameEn = "Piece", IsBaseUnit = true, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new UnitOfMeasure { Id = 2, UnitCode = "كيلو", UnitNameAr = "كيلوجرام", UnitNameEn = "Kilogram", IsBaseUnit = true, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new UnitOfMeasure { Id = 3, UnitCode = "لتر", UnitNameAr = "لتر", UnitNameEn = "Liter", IsBaseUnit = true, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new UnitOfMeasure { Id = 4, UnitCode = "متر", UnitNameAr = "متر", UnitNameEn = "Meter", IsBaseUnit = true, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new UnitOfMeasure { Id = 5, UnitCode = "علبة", UnitNameAr = "علبة", UnitNameEn = "Box", IsBaseUnit = true, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now }
            );
        }
    }
}
