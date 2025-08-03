using System.ComponentModel.DataAnnotations;

namespace Evacuation.Shared.Validation
{
    public static class ValidationHelper
    {
        // เมธอดนี้ใช้สำหรับตรวจสอบความถูกต้องของ object โดยอิงตาม DataAnnotation ที่กำหนดใน class
        public static Dictionary<string, string[]> ValidateObject(object obj)
        {
            // สร้าง context สำหรับการตรวจสอบ object โดยไม่ใช้ serviceProvider หรือ items เพิ่มเติม
            var context = new ValidationContext(obj, serviceProvider: null, items: null);

            // รายการที่จะเก็บผลลัพธ์ของการ validate
            var results = new List<ValidationResult>();

            // Dictionary สำหรับเก็บ error โดย key คือชื่อ property และ value คือ array ของ error messages
            var errors = new Dictionary<string, string[]>();

            // พยายาม validate object ตาม attribute ต่าง ๆ เช่น [Required], [Range], [StringLength], etc.
            // ตัว true ด้านหลังคือให้ตรวจสอบ property ภายในด้วย (recursive validation)
            if (!Validator.TryValidateObject(obj, context, results, true))
            {
                // ถ้ามี error ใน validation
                foreach (var result in results)
                {
                    // เดินซ้ำแต่ละ property ที่มี error
                    foreach (var memberName in result.MemberNames)
                    {
                        // ถ้ายังไม่มี key นี้ใน dictionary ก็เพิ่มเข้าไป
                        if (!errors.ContainsKey(memberName))
                            errors[memberName] = [];

                        // เพิ่มข้อความ error เข้าไปใน array ของ property นั้น ๆ
                        errors[memberName] = errors[memberName].Append(result.ErrorMessage ?? "").ToArray();
                    }
                }
            }

            // ส่งคืน dictionary ที่ประกอบด้วย property ชื่อ และ error messages ที่เกี่ยวข้อง
            return errors;
        }
    }

}
