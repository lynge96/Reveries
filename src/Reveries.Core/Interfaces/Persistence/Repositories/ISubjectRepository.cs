using Reveries.Core.Models;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface ISubjectRepository
{
    Task<Subject?> GetSubjectByNameAsync(string genre);
    Task<Subject> CreateSubjectAsync(Subject subject);
}
