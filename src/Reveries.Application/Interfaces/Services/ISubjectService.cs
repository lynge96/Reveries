namespace Reveries.Application.Interfaces.Services;

public interface ISubjectService
{
    Task SaveSubjectAsync(string subject, CancellationToken cancellationToken = default);
}