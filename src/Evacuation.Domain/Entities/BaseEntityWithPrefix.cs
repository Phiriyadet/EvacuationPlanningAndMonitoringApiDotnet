namespace Evacuation.Domain.Entities
{
    public abstract class BaseEntityWithPrefix : BaseEntity
    {
        public abstract string Prefix { get; } // กำหนด prefix ใน entity ลูก

        // BusinessId สร้างจาก prefix + Id เช่น Z-5, V-10
        public string BusinessId => $"{Prefix}-{Id}";
    }


}
