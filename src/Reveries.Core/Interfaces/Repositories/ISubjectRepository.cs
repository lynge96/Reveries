using Reveries.Core.Entities;

namespace Reveries.Core.Interfaces.Repositories;

public interface ISubjectRepository
{
    Task<Subject?> GetSubjectByNameAsync(string name);
    Task<int> CreateSubjectAsync(Subject subject);
}
