using System.ComponentModel.DataAnnotations;

namespace AlMohasebAlAraby.Shared.Models
{
    /// <summary>
    /// الكلاس الأساسي لجميع الكيانات في النظام
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للكيان
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// تاريخ آخر تحديث
        /// </summary>
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// معرف المستخدم الذي أنشأ السجل
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// معرف المستخدم الذي عدل السجل آخر مرة
        /// </summary>
        public int? LastModifiedBy { get; set; }

        /// <summary>
        /// حالة السجل (نشط/غير نشط)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// هل السجل محذوف (حذف منطقي)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// ملاحظات إضافية
        /// </summary>
        public string? Notes { get; set; }
    }
}
