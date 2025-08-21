namespace Evacuation.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }  // PK, EF Core ใช้ได้
        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset UpdatedAt { get; protected set; }

        protected BaseEntity()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        // อัปเดตเวลาแก้ไข
        public void SetUpdateAt() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}
