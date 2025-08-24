namespace Evacuation.Shared.GenerateId
{
    public static class ConvertType
    {
        public static int ConvertToIntPart(string id, string prefix)
        {
            if (string.IsNullOrWhiteSpace(id)) return 0;

            var parts = id.Split('-');
            if (parts.Length != 2 || !parts[0].Equals(prefix.TrimEnd('-'), StringComparison.OrdinalIgnoreCase))
                return 0;

            return int.TryParse(parts[1], out int number) ? number : 0;
        }

    }
}
