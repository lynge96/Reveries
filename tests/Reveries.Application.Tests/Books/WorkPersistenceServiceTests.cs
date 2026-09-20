using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Services;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Domain.Authors;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Tests.Books;

public class WorkPersistenceServiceTests
{
    private const string TestIsbn = "9780132350884";

    [Fact]
    public async Task Reuses_existing_work_when_title_and_author_match()
    {
        var fixture = new Fixture();
        var existingWorkId = WorkId.New();
        fixture.ResolvesAuthors(AuthorId.New());
        fixture.Works.FindWorkIdByTitleAndAuthorsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<AuthorId>>(), Arg.Any<CancellationToken>())
            .Returns(existingWorkId);

        await fixture.Sut().SaveBookAsync(Candidate(["George Orwell"]), null, null, CancellationToken.None);

        await fixture.Works.DidNotReceiveWithAnyArgs().InsertWorkAsync(default!, default!, default);
        Assert.NotNull(fixture.InsertedEdition);
        Assert.Equal(existingWorkId, fixture.InsertedEdition!.WorkId);
    }

    [Fact]
    public async Task Creates_a_new_work_when_no_title_and_author_match_exists()
    {
        var fixture = new Fixture();
        fixture.ResolvesAuthors(AuthorId.New());
        fixture.Works.FindWorkIdByTitleAndAuthorsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<AuthorId>>(), Arg.Any<CancellationToken>())
            .Returns((WorkId?)null);

        await fixture.Sut().SaveBookAsync(Candidate(["George Orwell"]), null, null, CancellationToken.None);

        Assert.NotNull(fixture.InsertedWork);
        Assert.Equal(fixture.InsertedWork!.Id, fixture.InsertedEdition!.WorkId);
    }

    [Fact]
    public async Task Skips_de_duplication_when_the_book_has_no_authors()
    {
        var fixture = new Fixture();
        fixture.ResolvesAuthors();

        await fixture.Sut().SaveBookAsync(Candidate([]), null, null, CancellationToken.None);

        await fixture.Works.DidNotReceiveWithAnyArgs()
            .FindWorkIdByTitleAndAuthorsAsync(default!, default!, default);
        Assert.NotNull(fixture.InsertedWork);
    }

    private static BookCandidate Candidate(IReadOnlyList<string> authors) => new()
    {
        Isbn = Isbn.Create(TestIsbn),
        Title = "Clean Code",
        Authors = authors
    };

    private sealed class Fixture
    {
        public IWorkRepository Works { get; } = Substitute.For<IWorkRepository>();
        public IEditionRepository Editions { get; } = Substitute.For<IEditionRepository>();
        public IAuthorResolver AuthorResolver { get; } = Substitute.For<IAuthorResolver>();
        public IPublisherResolver PublisherResolver { get; } = Substitute.For<IPublisherResolver>();
        public IGenreResolver GenreResolver { get; } = Substitute.For<IGenreResolver>();
        public IDeweyResolver DeweyResolver { get; } = Substitute.For<IDeweyResolver>();
        public ISeriesResolver SeriesResolver { get; } = Substitute.For<ISeriesResolver>();
        public ITransactionManager TransactionManager { get; } = Substitute.For<ITransactionManager>();
        public ITransaction Transaction { get; } = Substitute.For<ITransaction>();

        public Work? InsertedWork { get; private set; }
        public Edition? InsertedEdition { get; private set; }

        public Fixture()
        {
            TransactionManager.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Transaction);
            Editions.EditionExistsAsync(Arg.Any<Isbn>(), Arg.Any<CancellationToken>()).Returns(false);
            GenreResolver.ResolveIdsAsync(Arg.Any<IReadOnlyList<Genre>>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<string, int>());
            DeweyResolver.ResolveIdsAsync(Arg.Any<IReadOnlyList<DeweyDecimal>>(), Arg.Any<CancellationToken>())
                .Returns([]);

            Works.WhenForAnyArgs(w => w.InsertWorkAsync(default!, default!, default))
                .Do(ci => InsertedWork = ci.Arg<Work>());
            Editions.WhenForAnyArgs(e => e.InsertEditionAsync(default!, default))
                .Do(ci => InsertedEdition = ci.Arg<Edition>());
        }

        public void ResolvesAuthors(params AuthorId[] ids) =>
            AuthorResolver.ResolveIdsAsync(Arg.Any<IReadOnlyList<Author>>(), Arg.Any<CancellationToken>())
                .Returns([.. ids]);

        public WorkPersistenceService Sut() => new(
            TransactionManager,
            NullLogger<WorkPersistenceService>.Instance,
            Works,
            Editions,
            AuthorResolver,
            PublisherResolver,
            GenreResolver,
            DeweyResolver,
            SeriesResolver);
    }
}