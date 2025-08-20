namespace Evacuation.Domain.Entities
{
    public abstract class BaseEntityWithPrefix
    {
        public int Id { get; protected set; }  // PK, EF Core ใช้ได้

        public abstract string Prefix { get; } // กำหนด prefix ใน entity ลูก

        // BusinessId สร้างจาก prefix + Id เช่น Z-5, V-10
        public string BusinessId => $"{Prefix}-{Id}";

        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset UpdatedAt { get; protected set; }

        protected BaseEntityWithPrefix()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        // อัปเดตเวลาแก้ไข
        public void SetUpdateAt() => UpdatedAt = DateTimeOffset.UtcNow;
    }


}
