using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Books.Queries.FindBookByIsbn;
using Reveries.Application.Books.Queries.GetBookById;
using Reveries.Application.Common.Abstractions;
using Reveries.Domain.Authors;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Tests.Books;

public class BookLifecycleCharacterizationTests
{
    private const string TestIsbn = "9780132350884";

    [Fact]
    public async Task ScanEnrichPersistReadBack_MergesSources_PersistsMergedWork_AndReadsItBack()
    {
        var harness = new Harness();
        harness.IsbndbReturns(new BookCandidate
        {
            Isbn = Isbn.Create(TestIsbn),
            Title = "ISBNDB Title",
            Publisher = "ISBNDB Publisher",
            Pages = 464
        });
        harness.GoogleReturns(new BookCandidate
        {
            Isbn = Isbn.Create(TestIsbn),
            Title = "Google Title",
            Authors = ["George Orwell"],
            PrimaryGenres = ["Fiction"],
            Synopsis = "Short teaser.",
            Description = "Full description text."
        });

        using var scope = harness.BuildScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var found = await mediator.Send(new FindBookByIsbnQuery(TestIsbn));

        Assert.Equal("Google Title", found.Title);
        Assert.Equal("ISBNDB Publisher", found.Publisher);
        Assert.Equal(464, found.Pages);
        Assert.Equal("George Orwell", Assert.Single(found.Authors));

        var editionId = await mediator.Send(new CreateBookCommand
        {
            Isbn = found.Isbn,
            Title = found.Title,
            Authors = found.Authors.ToList(),
            Publisher = found.Publisher,
            PrimaryGenres = found.PrimaryGenres.ToList(),
            Synopsis = found.Synopsis,
            Description = found.Description,
            Pages = found.Pages
        });

        Assert.NotNull(harness.InsertedWork);
        Assert.Equal("Google Title", harness.InsertedWork!.Title.ToString());
        Assert.NotNull(harness.InsertedEdition);
        Assert.Equal(harness.InsertedEdition!.Id, editionId);
        await harness.TransactionManager.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await harness.Works.Received(1).InsertWorkAsync(Arg.Any<Work>(), Arg.Any<WorkRelations>(), Arg.Any<CancellationToken>());
        await harness.Editions.Received(1).InsertEditionAsync(Arg.Any<Edition>(), Arg.Any<CancellationToken>());
        await harness.Transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        var storedId = Guid.NewGuid();
        harness.Queries.GetBookByIdAsync(storedId, Arg.Any<CancellationToken>()).Returns(new BookDetails
        {
            BookId = storedId,
            Title = harness.InsertedWork.Title.ToString(),
            Publisher = found.Publisher
        });

        var readBack = await mediator.Send(new GetBookByIdQuery(storedId));

        Assert.Equal("Google Title", readBack.Title);
        Assert.Equal("ISBNDB Publisher", readBack.Publisher);
    }

    [Fact]
    public async Task CreateBook_WithSeries_ResolvesAndPersistsSeriesOnTheWork()
    {
        var harness = new Harness();

        using var scope = harness.BuildScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new CreateBookCommand
        {
            Isbn = Isbn.Create(TestIsbn),
            Title = "Guards! Guards!",
            Series = "Discworld",
            NumberInSeries = 8
        });

        Assert.NotNull(harness.InsertedWork);
        Assert.NotNull(harness.InsertedWork!.SeriesId);
        Assert.Equal(8, harness.InsertedWork.NumberInSeries);
        await harness.Series.Received(1).AddAsync(
            Arg.Is<Reveries.Domain.BookSeries.Series>(s => s.Name == "Discworld"),
            Arg.Any<CancellationToken>());
    }

    private sealed class Harness
    {
        public IBookSearch Isbndb { get; } = Substitute.For<IBookSearch>();
        public IBookSearch Google { get; } = Substitute.For<IBookSearch>();
        public IEditionRepository Editions { get; } = Substitute.For<IEditionRepository>();
        public IWorkRepository Works { get; } = Substitute.For<IWorkRepository>();
        public IAuthorRepository Authors { get; } = Substitute.For<IAuthorRepository>();
        public IGenreRepository Genres { get; } = Substitute.For<IGenreRepository>();
        public IDeweyDecimalsRepository DeweyDecimals { get; } = Substitute.For<IDeweyDecimalsRepository>();
        public IPublisherRepository Publishers { get; } = Substitute.For<IPublisherRepository>();
        public ISeriesRepository Series { get; } = Substitute.For<ISeriesRepository>();
        public ITransactionManager TransactionManager { get; } = Substitute.For<ITransactionManager>();
        public ITransaction Transaction { get; } = Substitute.For<ITransaction>();
        public IBookQueryRepository Queries { get; } = Substitute.For<IBookQueryRepository>();

        public Work? InsertedWork { get; private set; }
        public Edition? InsertedEdition { get; private set; }

        public Harness()
        {
            Isbndb.Source.Returns(BookSource.Isbndb);
            Google.Source.Returns(BookSource.GoogleBooks);
            IsbndbReturns();
            GoogleReturns();

            Editions.EditionExistsAsync(Arg.Any<Isbn>(), Arg.Any<CancellationToken>()).Returns(false);
            Authors.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>()).Returns([]);
            Publishers.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Reveries.Domain.Publishers.Publisher?)null);
            Series.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Reveries.Domain.BookSeries.Series?)null);

            Genres.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
            Genres.AddRangeAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(ci => AssignIds(ci.Arg<IReadOnlyList<string>>()));
            DeweyDecimals.GetByCodesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
            DeweyDecimals.AddRangeAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(ci => AssignIds(ci.Arg<IReadOnlyList<string>>()));

            TransactionManager.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(Transaction);

            Works.WhenForAnyArgs(w => w.InsertWorkAsync(default!, default!, default))
                .Do(ci => InsertedWork = ci.Arg<Work>());
            Editions.WhenForAnyArgs(e => e.InsertEditionAsync(default!, default))
                .Do(ci => InsertedEdition = ci.Arg<Edition>());
        }

        public void IsbndbReturns(params BookCandidate[] candidates) => Configure(Isbndb, candidates);
        public void GoogleReturns(params BookCandidate[] candidates) => Configure(Google, candidates);

        public IServiceScope BuildScope()
        {
            var services = new ServiceCollection();
            services.AddApplication();
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

            services.AddSingleton(Isbndb);
            services.AddSingleton(Google);
            services.AddSingleton(Editions);
            services.AddSingleton(Works);
            services.AddSingleton(Authors);
            services.AddSingleton(Genres);
            services.AddSingleton(DeweyDecimals);
            services.AddSingleton(Publishers);
            services.AddSingleton(Series);
            services.AddSingleton(TransactionManager);
            services.AddSingleton(Queries);

            return services.BuildServiceProvider().CreateScope();
        }

        private static void Configure(IBookSearch source, BookCandidate[] candidates)
        {
            source.GetBooksByIsbnsAsync(Arg.Any<IReadOnlyList<Isbn>>(), Arg.Any<CancellationToken>())
                .Returns((IReadOnlyList<BookCandidate>?)candidates);
        }

        private static Dictionary<string, int> AssignIds(IReadOnlyList<string> keys)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var next = 1;
            foreach (var key in keys)
                map[key] = next++;

            return map;
        }
    }
}