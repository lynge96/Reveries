using Reveries.Domain.Enums;
using Reveries.Domain.Models;
using Reveries.Persistence.Context;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Writes a book aggregate through a real UnitOfWork transaction (the same
/// repository sequence the application's SaveBookAsync uses) and reads it back
/// through the view, proving the write path and its transaction against real
/// Postgres. Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class BookWriteRoundTripTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public BookWriteRoundTripTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Saving_a_book_through_the_unit_of_work_round_trips_through_the_view()
    {
        // Arrange
        var book = NewBook();
        await using var writeContext = _fixture.NewDbContext();
        var unitOfWork = NewUnitOfWork(writeContext);

        // Act
        await PersistAsync(unitOfWork, book, CancellationToken.None);

        // Assert — read back over a fresh connection to prove it committed
        await using var readContext = _fixture.NewDbContext();
        var persisted = await new BookRepository(readContext)
            .GetBookByIdAsync(book.Id.Value, CancellationToken.None);

        Assert.NotNull(persisted);
        Assert.Equal(book.Title.Value, persisted!.Title.Value);
        Assert.Equal(book.Isbn13!.Value, persisted.Isbn13!.Value);
        Assert.Equal(book.Isbn10!.Value, persisted.Isbn10!.Value);
        Assert.Equal(book.Pages, persisted.Pages);
        Assert.Equal(book.Language, persisted.Language);
        Assert.Equal(book.PublicationDate, persisted.PublicationDate);
        Assert.False(persisted.IsRead);
        Assert.Equal(book.Publisher!.Name, persisted.Publisher!.Name);
        Assert.Null(persisted.Series);

        Assert.Equal(
            book.Authors.Select(a => a.NormalizedName).OrderBy(n => n),
            persisted.Authors.Select(a => a.NormalizedName).OrderBy(n => n));
        Assert.Equal(
            book.Genres.Select(g => g.Value).OrderBy(v => v),
            persisted.Genres.Select(g => g.Value).OrderBy(v => v));
        Assert.Equal(
            book.DeweyDecimals.Select(d => d.Code).OrderBy(c => c),
            persisted.DeweyDecimals.Select(d => d.Code).OrderBy(c => c));
    }

    /// <summary>
    /// Mirrors the repository calls of BookPersistenceService.SaveBookAsync — the
    /// reference entities are resolved via GetOrCreate, the book is inserted, then
    /// the join rows — all inside one committed transaction.
    /// </summary>
    private static async Task PersistAsync(UnitOfWork unitOfWork, Book book, CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var publisher = await unitOfWork.Publishers.GetOrCreateAsync(book.Publisher, ct);
        book.SetPublisher(publisher);

        var series = await unitOfWork.Series.GetOrCreateAsync(book.Series, ct);
        book.SetSeries(series);

        await unitOfWork.Books.InsertBookAsync(book, ct);

        var authorIds = await unitOfWork.Authors.GetOrCreateAuthorsAsync(book.Authors, ct);
        await unitOfWork.BookAuthors.InsertBookAuthorsAsync(book.Id.Value, authorIds, ct);

        var genreIds = await unitOfWork.Genres.GetOrCreateGenresAsync(book.Genres, ct);
        await unitOfWork.BookGenres.InsertBookGenresAsync(book.Id.Value, genreIds, ct);

        var deweyIds = await unitOfWork.DeweyDecimals.GetOrCreateDeweyDecimalsAsync(book.DeweyDecimals, ct);
        await unitOfWork.BookDeweyDecimals.InsertBookDeweyDecimalsAsync(book.Id.Value, deweyIds, ct);

        await transaction.CommitAsync(ct);
    }

    private static UnitOfWork NewUnitOfWork(PostgresDbContext db) =>
        new(
            db,
            new BookRepository(db),
            new AuthorRepository(db),
            new SeriesRepository(db),
            new PublisherRepository(db),
            new BookAuthorsRepository(db),
            new BookGenresRepository(db),
            new DeweyDecimalsRepository(db),
            new GenreRepository(db),
            new BookDeweyDecimalsRepository(db));

    private static Book NewBook() => Book.Create(
        isbn13: "9780451524935",
        isbn10: "0451524934",
        title: "Nineteen Eighty-Four",
        authors: ["George Orwell", "Aldous Huxley"],
        pages: 328,
        publishDate: "1949",
        publisher: "Signet Classics",
        languageIso639: "en",
        synopsis: "A dystopian novel.",
        imageThumbnail: null,
        imageUrl: null,
        msrp: null,
        binding: null,
        edition: null,
        weight: null,
        thickness: null,
        height: null,
        width: null,
        subjects: ["Dystopia", "Fantasy"],
        deweyDecimals: ["823"],
        dataSource: DataSource.Database);
}
