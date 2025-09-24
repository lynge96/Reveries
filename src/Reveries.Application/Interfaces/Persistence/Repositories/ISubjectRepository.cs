using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Persistence.Repositories;

public interface ISubjectRepository
{
    Task<Subject?> GetSubjectByNameAsync(string genre);
    Task<int> CreateSubjectAsync(Subject subject);
}
