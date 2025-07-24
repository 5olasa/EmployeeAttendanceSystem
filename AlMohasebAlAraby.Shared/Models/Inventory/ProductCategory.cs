using System.ComponentModel.DataAnnotations;

namespace AlMohasebAlAraby.Shared.Models.Inventory
{
    /// <summary>
    /// تصنيفات الأصناف
    /// </summary>
    public class ProductCategory : BaseEntity
    {
        /// <summary>
        /// كود التصنيف
        /// </summary>
        [Required]
        [StringLength(20)]
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم التصنيف باللغة العربية
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CategoryNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم التصنيف باللغة الإنجليزية
        /// </summary>
        [StringLength(100)]
        public string? CategoryNameEn { get; set; }

        /// <summary>
        /// معرف التصنيف الأب
        /// </summary>
        public int? ParentCategoryId { get; set; }

        /// <summary>
        /// التصنيف الأب
        /// </summary>
        public virtual ProductCategory? ParentCategory { get; set; }

        /// <summary>
        /// التصنيفات الفرعية
        /// </summary>
        public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        /// <summary>
        /// مستوى التصنيف في الشجرة
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// وصف التصنيف
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// صورة التصنيف
        /// </summary>
        [StringLength(500)]
        public string? ImagePath { get; set; }

        /// <summary>
        /// ترتيب العرض
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// الأصناف في هذا التصنيف
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        /// <summary>
        /// الحصول على المسار الكامل للتصنيف
        /// </summary>
        public string GetFullPath()
        {
            if (ParentCategory == null)
                return CategoryNameAr;
            
            return $"{ParentCategory.GetFullPath()} > {CategoryNameAr}";
        }

        /// <summary>
        /// الحصول على الكود الكامل للتصنيف
        /// </summary>
        public string GetFullCode()
        {
            if (ParentCategory == null)
                return CategoryCode;
            
            return $"{ParentCategory.GetFullCode()}.{CategoryCode}";
        }

        /// <summary>
        /// عدد الأصناف في هذا التصنيف وتصنيفاته الفرعية
        /// </summary>
        public int GetTotalProductsCount()
        {
            int count = Products.Count;
            foreach (var subCategory in SubCategories)
            {
                count += subCategory.GetTotalProductsCount();
            }
            return count;
        }
    }

    /// <summary>
    /// وحدات القياس
    /// </summary>
    public class UnitOfMeasure : BaseEntity
    {
        /// <summary>
        /// كود وحدة القياس
        /// </summary>
        [Required]
        [StringLength(10)]
        public string UnitCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم وحدة القياس باللغة العربية
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UnitNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم وحدة القياس باللغة الإنجليزية
        /// </summary>
        [StringLength(50)]
        public string? UnitNameEn { get; set; }

        /// <summary>
        /// وصف وحدة القياس
        /// </summary>
        [StringLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// هل هي وحدة أساسية أم فرعية
        /// </summary>
        public bool IsBaseUnit { get; set; } = true;

        /// <summary>
        /// الأصناف التي تستخدم هذه الوحدة
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        /// <summary>
        /// تحويلات الوحدة
        /// </summary>
        public virtual ICollection<ProductUnitConversion> UnitConversions { get; set; } = new List<ProductUnitConversion>();
    }

    /// <summary>
    /// تحويلات وحدات القياس للمنتجات
    /// </summary>
    public class ProductUnitConversion : BaseEntity
    {
        /// <summary>
        /// معرف المنتج
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// المنتج
        /// </summary>
        public virtual Product Product { get; set; } = null!;

        /// <summary>
        /// معرف الوحدة الأساسية
        /// </summary>
        public int BaseUnitId { get; set; }

        /// <summary>
        /// الوحدة الأساسية
        /// </summary>
        public virtual UnitOfMeasure BaseUnit { get; set; } = null!;

        /// <summary>
        /// معرف الوحدة البديلة
        /// </summary>
        public int AlternativeUnitId { get; set; }

        /// <summary>
        /// الوحدة البديلة
        /// </summary>
        public virtual UnitOfMeasure AlternativeUnit { get; set; } = null!;

        /// <summary>
        /// معامل التحويل (كم من الوحدة الأساسية = 1 من الوحدة البديلة)
        /// </summary>
        public decimal ConversionFactor { get; set; }

        /// <summary>
        /// سعر البيع بالوحدة البديلة
        /// </summary>
        public decimal? AlternativeSellingPrice { get; set; }

        /// <summary>
        /// تحويل الكمية من الوحدة البديلة إلى الوحدة الأساسية
        /// </summary>
        public decimal ConvertToBaseUnit(decimal alternativeQuantity)
        {
            return alternativeQuantity * ConversionFactor;
        }

        /// <summary>
        /// تحويل الكمية من الوحدة الأساسية إلى الوحدة البديلة
        /// </summary>
        public decimal ConvertFromBaseUnit(decimal baseQuantity)
        {
            return baseQuantity / ConversionFactor;
        }
    }
}
