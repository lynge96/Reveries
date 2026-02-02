using Reveries.Core.Models;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Interfaces.Persistence.Repositories;

public interface ISubjectRepository
{
    Task<Genre?> GetSubjectByNameAsync(string genre);
    Task<Genre> CreateSubjectAsync(Genre genre);
}
