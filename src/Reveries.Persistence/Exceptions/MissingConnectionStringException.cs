namespace Reveries.Persistence.Exceptions;

public sealed class MissingConnectionStringException : PersistenceException
{
    public string Name { get; }

    public MissingConnectionStringException(string name)
        : base(
            $"Missing connection string 'ConnectionStrings:{name}'. Set it via user-secrets (dev) " +
            $"or the ConnectionStrings__{name} environment variable (prod).")
    {
        Name = name;
    }
}