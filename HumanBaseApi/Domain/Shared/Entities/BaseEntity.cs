namespace Domain.Shared.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public bool IsActive { get; private set; } = true;
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public void InitializeAdd(bool isActive = true)
        {
            IsActive = isActive;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            UpdateTimestamp();
        }
        public void SetIsDeleted()
        {
            IsDeleted = true;
            IsActive = false;
            UpdateTimestamp();
        }
    }
}