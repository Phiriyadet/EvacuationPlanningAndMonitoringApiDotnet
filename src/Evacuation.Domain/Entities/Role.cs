namespace Evacuation.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;

        protected Role() { }

        public Role(string name, string description = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));
            Name = name;
            Description = description;
        }

        public void Update(string name, string description = "")
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            Description = description;
            SetUpdateAt();
        }
    }
}
