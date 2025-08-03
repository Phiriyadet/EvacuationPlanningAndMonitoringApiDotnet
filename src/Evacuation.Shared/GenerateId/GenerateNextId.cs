namespace Evacuation.Shared.GenerateId
{
    public class GenerateNextId
    {
        public static string GenerateNextIdWithPrefix(string prefix, string? lastId)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));
            }

            // กรณีที่ยังไม่มี ID เก่าเลย ให้เริ่มที่ 1
            if (string.IsNullOrWhiteSpace(lastId))
            {
                return $"{prefix}-1";
            }

            // แยกส่วน Prefix และ Number ออกจากกัน
            var parts = lastId.Trim().Split('-');

            // ตรวจสอบความถูกต้องของ Format ID
            if (parts.Length != 2 || !string.Equals(parts[0], prefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Last ID '{lastId}' format is invalid or does not match the prefix '{prefix}'.", nameof(lastId));
            }

            // แปลงส่วน Number เป็น Integer และบวกเพิ่ม
            if (!int.TryParse(parts[1], out int lastNumber))
            {
                throw new ArgumentException($"Last ID number part '{parts[1]}' is not a valid integer.", nameof(lastId));
            }

            return $"{prefix}-{lastNumber + 1}";
        }

    }
}
