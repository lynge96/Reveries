namespace Reveries.Domain.Common;

public abstract class Entity<TId> where TId : struct
{
    public TId Id { get; protected init; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }
}