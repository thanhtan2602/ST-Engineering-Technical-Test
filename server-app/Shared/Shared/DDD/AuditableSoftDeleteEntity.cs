namespace Shared.DDD
{
    public abstract class AuditableSoftDeleteEntity<T> : Entity<T>
    {
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
