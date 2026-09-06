namespace Reveries.Persistence.Exceptions;

public sealed class DatabaseMigrationException : PersistenceException
{
    public DatabaseMigrationException(Exception? innerException)
        : base("Database migration failed.", innerException)
    {
    }
}