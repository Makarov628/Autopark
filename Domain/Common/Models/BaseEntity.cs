namespace Autopark.Domain.Common.Models;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        SetCreatedDate();
        RenewUpdateDate();
    }

    public void SetCreatedDate()
    {
        CreatedAt = DateTime.Now;
    }

    public void RenewUpdateDate()
    {
        UpdatedAt = DateTime.Now;
    }
}