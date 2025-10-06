using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface ISubjectRepository
{
    Task<Subject?> GetSubjectByNameAsync(string genre);
    Task<int> CreateSubjectAsync(Subject subject);
}
