using System.Linq.Expressions;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IGenericIncludeRepository<T, TKey> : IGenericRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        Task<Dictionary<TKey, string>> GetIdMapAsync();
        /// <summary>
        /// ดึงข้อมูลทั้งหมดของ Entity <typeparamref name="T"/> 
        /// พร้อมกับ Navigation Properties ที่ต้องการ include
        /// </summary>
        /// <param name="includes">
        /// รายการ Navigation Properties ที่ต้องการ include
        /// - ใช้ lambda expression ในรูปแบบ <c>x => x.Property</c>
        /// - ส่งได้หลายค่าโดยคั่นด้วยเครื่องหมายจุลภาค (comma)
        /// - ตัวอย่างการเรียกใช้:
        ///   <code>
        ///   var plans = await repo.GetAllWithIncludeAsync(
        ///       p => p.Zone,
        ///       p => p.Vehicle
        ///   );
        ///   </code>
        /// </param>
        /// <returns>
        /// ส่งกลับ <see cref="IEnumerable{T}"/> ที่รวมข้อมูล Entity หลัก 
        /// และ Navigation Properties ที่ include มาด้วย
        /// </returns>
        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        
        /// <summary>
        /// ดึงข้อมูลของ Entity <typeparamref name="T"/> ตามค่า <paramref name="id"/> 
        /// พร้อมกับ Navigation Properties ที่ต้องการ include
        /// </summary>
        /// <param name="id">
        /// ค่าคีย์หลัก (Primary Key) ของ Entity ที่ต้องการค้นหา
        /// </param>
        /// <param name="includes">
        /// รายการ Navigation Properties ที่ต้องการ include
        /// - ใช้ lambda expression ในรูปแบบ <c>x => x.Property</c>
        /// - สามารถส่งได้หลายค่า เช่น <c>x => x.Zone</c>, <c>x => x.Vehicle</c>
        /// - ตัวอย่างการเรียกใช้:
        ///   <code>
        ///   var plan = await repo.GetByIdWithIncludeAsync(
        ///       1,
        ///       p => p.Zone,
        ///       p => p.Vehicle
        ///   );
        ///   </code>
        /// </param>
        /// <returns>
        /// ส่งกลับ Entity ที่พบ (ชนิด <typeparamref name="T"/>) 
        /// โดยรวมข้อมูลของ Navigation Properties ที่ include มาด้วย 
        /// หรือ <c>null</c> ถ้าไม่พบข้อมูล
        /// </returns>
        Task<T?> GetByIdWithIncludeAsync(TKey key, params Expression<Func<T, object>>[] includes);
    }
}
