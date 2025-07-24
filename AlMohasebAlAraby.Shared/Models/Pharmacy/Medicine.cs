using System.ComponentModel.DataAnnotations;

namespace AlMohasebAlAraby.Shared.Models.Pharmacy
{
    /// <summary>
    /// الأدوية - قاعدة بيانات الأدوية المصرية
    /// </summary>
    public class Medicine : BaseEntity
    {
        /// <summary>
        /// كود الدواء
        /// </summary>
        [Required]
        [StringLength(50)]
        public string MedicineCode { get; set; } = string.Empty;

        /// <summary>
        /// الاسم التجاري للدواء
        /// </summary>
        [Required]
        [StringLength(200)]
        public string TradeName { get; set; } = string.Empty;

        /// <summary>
        /// الاسم العلمي (المادة الفعالة)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ScientificName { get; set; } = string.Empty;

        /// <summary>
        /// التركيز
        /// </summary>
        [StringLength(100)]
        public string? Concentration { get; set; }

        /// <summary>
        /// الشكل الصيدلاني
        /// </summary>
        public PharmaceuticalForm PharmaceuticalForm { get; set; }

        /// <summary>
        /// الشركة المصنعة
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// الشركة المستوردة/الموزعة
        /// </summary>
        [StringLength(200)]
        public string? Distributor { get; set; }

        /// <summary>
        /// بلد المنشأ
        /// </summary>
        [StringLength(100)]
        public string? CountryOfOrigin { get; set; }

        /// <summary>
        /// الباركود
        /// </summary>
        [StringLength(100)]
        public string? Barcode { get; set; }

        /// <summary>
        /// رقم التسجيل في وزارة الصحة
        /// </summary>
        [StringLength(100)]
        public string? RegistrationNumber { get; set; }

        /// <summary>
        /// المجموعة الدوائية
        /// </summary>
        public int? PharmacologicalGroupId { get; set; }

        /// <summary>
        /// المجموعة الدوائية
        /// </summary>
        public virtual PharmacologicalGroup? PharmacologicalGroup { get; set; }

        /// <summary>
        /// التصنيف العلاجي
        /// </summary>
        public int? TherapeuticClassificationId { get; set; }

        /// <summary>
        /// التصنيف العلاجي
        /// </summary>
        public virtual TherapeuticClassification? TherapeuticClassification { get; set; }

        /// <summary>
        /// طريقة الإعطاء
        /// </summary>
        public RouteOfAdministration RouteOfAdministration { get; set; }

        /// <summary>
        /// هل يتطلب وصفة طبية
        /// </summary>
        public bool RequiresPrescription { get; set; } = true;

        /// <summary>
        /// هل هو دواء مخدر أو مؤثر عقلي
        /// </summary>
        public bool IsControlledSubstance { get; set; } = false;

        /// <summary>
        /// هل هو دواء بارد (يحتاج تبريد)
        /// </summary>
        public bool RequiresRefrigeration { get; set; } = false;

        /// <summary>
        /// درجة حرارة التخزين المثلى
        /// </summary>
        [StringLength(50)]
        public string? StorageTemperature { get; set; }

        /// <summary>
        /// شروط التخزين
        /// </summary>
        [StringLength(500)]
        public string? StorageConditions { get; set; }

        /// <summary>
        /// مدة الصلاحية بالشهور
        /// </summary>
        public int? ShelfLifeMonths { get; set; }

        /// <summary>
        /// الجرعة المعتادة
        /// </summary>
        [StringLength(200)]
        public string? UsualDosage { get; set; }

        /// <summary>
        /// دواعي الاستعمال
        /// </summary>
        [StringLength(1000)]
        public string? Indications { get; set; }

        /// <summary>
        /// موانع الاستعمال
        /// </summary>
        [StringLength(1000)]
        public string? Contraindications { get; set; }

        /// <summary>
        /// الآثار الجانبية
        /// </summary>
        [StringLength(1000)]
        public string? SideEffects { get; set; }

        /// <summary>
        /// التفاعلات الدوائية
        /// </summary>
        [StringLength(1000)]
        public string? DrugInteractions { get; set; }

        /// <summary>
        /// تحذيرات خاصة
        /// </summary>
        [StringLength(1000)]
        public string? Warnings { get; set; }

        /// <summary>
        /// السعر الرسمي
        /// </summary>
        public decimal? OfficialPrice { get; set; }

        /// <summary>
        /// سعر البيع للجمهور
        /// </summary>
        public decimal? PublicPrice { get; set; }

        /// <summary>
        /// هل متوفر في السوق
        /// </summary>
        public bool IsAvailableInMarket { get; set; } = true;

        /// <summary>
        /// تاريخ آخر تحديث للسعر
        /// </summary>
        public DateTime? LastPriceUpdate { get; set; }

        /// <summary>
        /// دفعات الدواء في المخزون
        /// </summary>
        public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

        /// <summary>
        /// البدائل المتاحة
        /// </summary>
        public virtual ICollection<MedicineAlternative> Alternatives { get; set; } = new List<MedicineAlternative>();

        /// <summary>
        /// الحصول على الكمية المتاحة الإجمالية
        /// </summary>
        public decimal GetTotalAvailableQuantity()
        {
            return MedicineBatches
                .Where(b => b.ExpiryDate > DateTime.Now && b.AvailableQuantity > 0)
                .Sum(b => b.AvailableQuantity);
        }

        /// <summary>
        /// الحصول على أقرب دفعة للانتهاء
        /// </summary>
        public MedicineBatch? GetNearestExpiryBatch()
        {
            return MedicineBatches
                .Where(b => b.ExpiryDate > DateTime.Now && b.AvailableQuantity > 0)
                .OrderBy(b => b.ExpiryDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// التحقق من وجود دفعات منتهية الصلاحية
        /// </summary>
        public bool HasExpiredBatches()
        {
            return MedicineBatches.Any(b => b.ExpiryDate <= DateTime.Now && b.AvailableQuantity > 0);
        }

        /// <summary>
        /// التحقق من وجود دفعات قريبة الانتهاء
        /// </summary>
        public bool HasNearExpiryBatches(int warningDays = 90)
        {
            var warningDate = DateTime.Now.AddDays(warningDays);
            return MedicineBatches.Any(b => b.ExpiryDate <= warningDate && b.ExpiryDate > DateTime.Now && b.AvailableQuantity > 0);
        }
    }

    /// <summary>
    /// الأشكال الصيدلانية
    /// </summary>
    public enum PharmaceuticalForm
    {
        /// <summary>
        /// أقراص
        /// </summary>
        Tablets = 1,

        /// <summary>
        /// كبسولات
        /// </summary>
        Capsules = 2,

        /// <summary>
        /// شراب
        /// </summary>
        Syrup = 3,

        /// <summary>
        /// حقن
        /// </summary>
        Injection = 4,

        /// <summary>
        /// مرهم
        /// </summary>
        Ointment = 5,

        /// <summary>
        /// كريم
        /// </summary>
        Cream = 6,

        /// <summary>
        /// قطرة
        /// </summary>
        Drops = 7,

        /// <summary>
        /// بخاخ
        /// </summary>
        Spray = 8,

        /// <summary>
        /// لبوس
        /// </summary>
        Suppository = 9,

        /// <summary>
        /// محلول
        /// </summary>
        Solution = 10,

        /// <summary>
        /// مسحوق
        /// </summary>
        Powder = 11,

        /// <summary>
        /// جل
        /// </summary>
        Gel = 12
    }

    /// <summary>
    /// طرق الإعطاء
    /// </summary>
    public enum RouteOfAdministration
    {
        /// <summary>
        /// عن طريق الفم
        /// </summary>
        Oral = 1,

        /// <summary>
        /// حقن عضلي
        /// </summary>
        IntramuscularInjection = 2,

        /// <summary>
        /// حقن وريدي
        /// </summary>
        IntravenousInjection = 3,

        /// <summary>
        /// حقن تحت الجلد
        /// </summary>
        SubcutaneousInjection = 4,

        /// <summary>
        /// موضعي
        /// </summary>
        Topical = 5,

        /// <summary>
        /// عن طريق العين
        /// </summary>
        Ophthalmic = 6,

        /// <summary>
        /// عن طريق الأذن
        /// </summary>
        Otic = 7,

        /// <summary>
        /// عن طريق الأنف
        /// </summary>
        Nasal = 8,

        /// <summary>
        /// عن طريق الاستنشاق
        /// </summary>
        Inhalation = 9,

        /// <summary>
        /// شرجي
        /// </summary>
        Rectal = 10,

        /// <summary>
        /// مهبلي
        /// </summary>
        Vaginal = 11
    }
}
