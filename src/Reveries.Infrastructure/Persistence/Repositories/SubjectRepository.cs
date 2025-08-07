using Reveries.Core.Interfaces;
using Reveries.Infrastructure.Persistence.Context;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public SubjectRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
}