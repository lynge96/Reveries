using Reveries.Application.Interfaces.Services;

namespace Reveries.Application.Services;

public class SubjectService : ISubjectService
{
    public Task SaveSubjectAsync(string subject, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}