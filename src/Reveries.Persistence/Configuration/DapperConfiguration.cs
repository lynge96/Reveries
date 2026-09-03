using Dapper;

namespace Reveries.Persistence.Configuration;

public static class DapperConfiguration
{
    public static void Configure() =>
        DefaultTypeMap.MatchNamesWithUnderscores = true;
}
