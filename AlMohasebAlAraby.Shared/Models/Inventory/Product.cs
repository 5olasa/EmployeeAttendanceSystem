using System.ComponentModel.DataAnnotations;
using AlMohasebAlAraby.Shared.Enums;

namespace AlMohasebAlAraby.Shared.Models.Inventory
{
    /// <summary>
    /// الأصناف/المنتجات
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// كود الصنف
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم الصنف باللغة العربية
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ProductNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الصنف باللغة الإنجليزية
        /// </summary>
        [StringLength(200)]
        public string? ProductNameEn { get; set; }

        /// <summary>
        /// الباركود
        /// </summary>
        [StringLength(100)]
        public string? Barcode { get; set; }

        /// <summary>
        /// معرف التصنيف
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// التصنيف
        /// </summary>
        public virtual ProductCategory Category { get; set; } = null!;

        /// <summary>
        /// معرف وحدة القياس الأساسية
        /// </summary>
        public int UnitOfMeasureId { get; set; }

        /// <summary>
        /// وحدة القياس الأساسية
        /// </summary>
        public virtual UnitOfMeasure UnitOfMeasure { get; set; } = null!;

        /// <summary>
        /// سعر التكلفة
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// سعر البيع
        /// </summary>
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// الحد الأدنى للمخزون
        /// </summary>
        public decimal MinimumStock { get; set; }

        /// <summary>
        /// الحد الأقصى للمخزون
        /// </summary>
        public decimal MaximumStock { get; set; }

        /// <summary>
        /// نقطة إعادة الطلب
        /// </summary>
        public decimal ReorderPoint { get; set; }

        /// <summary>
        /// الكمية الحالية في المخزون
        /// </summary>
        public decimal CurrentStock { get; set; }

        /// <summary>
        /// الكمية المحجوزة
        /// </summary>
        public decimal ReservedStock { get; set; }

        /// <summary>
        /// الكمية المتاحة للبيع
        /// </summary>
        public decimal AvailableStock => CurrentStock - ReservedStock;

        /// <summary>
        /// هل الصنف يتطلب تتبع الدفعات
        /// </summary>
        public bool RequiresBatchTracking { get; set; } = false;

        /// <summary>
        /// هل الصنف يتطلب تتبع الأرقام التسلسلية
        /// </summary>
        public bool RequiresSerialNumbers { get; set; } = false;

        /// <summary>
        /// هل الصنف له تاريخ انتهاء صلاحية
        /// </summary>
        public bool HasExpiryDate { get; set; } = false;

        /// <summary>
        /// مدة الصلاحية بالأيام
        /// </summary>
        public int? ShelfLifeDays { get; set; }

        /// <summary>
        /// نسبة الضريبة
        /// </summary>
        public decimal TaxRate { get; set; } = 14.0m;

        /// <summary>
        /// هل الصنف خاضع للضريبة
        /// </summary>
        public bool IsTaxable { get; set; } = true;

        /// <summary>
        /// وصف الصنف
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// صورة الصنف
        /// </summary>
        [StringLength(500)]
        public string? ImagePath { get; set; }

        /// <summary>
        /// الوزن
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// الأبعاد (الطول × العرض × الارتفاع)
        /// </summary>
        [StringLength(100)]
        public string? Dimensions { get; set; }

        /// <summary>
        /// اللون
        /// </summary>
        [StringLength(50)]
        public string? Color { get; set; }

        /// <summary>
        /// الحجم
        /// </summary>
        [StringLength(50)]
        public string? Size { get; set; }

        /// <summary>
        /// الموديل
        /// </summary>
        [StringLength(100)]
        public string? Model { get; set; }

        /// <summary>
        /// الماركة
        /// </summary>
        [StringLength(100)]
        public string? Brand { get; set; }

        /// <summary>
        /// بلد المنشأ
        /// </summary>
        [StringLength(100)]
        public string? CountryOfOrigin { get; set; }

        /// <summary>
        /// حركات المخزون
        /// </summary>
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        /// <summary>
        /// دفعات المنتج
        /// </summary>
        public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();

        /// <summary>
        /// وحدات القياس البديلة
        /// </summary>
        public virtual ICollection<ProductUnitConversion> UnitConversions { get; set; } = new List<ProductUnitConversion>();

        /// <summary>
        /// أسعار المنتج حسب العميل أو الكمية
        /// </summary>
        public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

        /// <summary>
        /// التحقق من توفر الكمية المطلوبة
        /// </summary>
        public bool IsQuantityAvailable(decimal requiredQuantity)
        {
            return AvailableStock >= requiredQuantity;
        }

        /// <summary>
        /// التحقق من الحاجة لإعادة الطلب
        /// </summary>
        public bool NeedsReorder()
        {
            return CurrentStock <= ReorderPoint;
        }

        /// <summary>
        /// التحقق من انخفاض المخزون
        /// </summary>
        public bool IsLowStock()
        {
            return CurrentStock <= MinimumStock;
        }

        /// <summary>
        /// حساب قيمة المخزون الحالي
        /// </summary>
        public decimal CalculateStockValue()
        {
            return CurrentStock * CostPrice;
        }
    }
}
