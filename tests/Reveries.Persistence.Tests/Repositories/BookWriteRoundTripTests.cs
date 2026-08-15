using Reveries.Domain.Books;
using Reveries.Domain.Enums;
using Reveries.Persistence.Context;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Writes a book aggregate through a real transaction (the same repository
/// sequence the application's SaveBookAsync uses) and reads it back through the
/// view, proving the write path and its transaction against real Postgres. Each
/// test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class BookWriteRoundTripTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public BookWriteRoundTripTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Saving_a_book_through_a_transaction_round_trips_through_the_view()
    {
        // Arrange
        var book = NewBook();
        await using var writeContext = _fixture.NewDbContext();

        // Act
        await PersistAsync(writeContext, book, CancellationToken.None);

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
    private static async Task PersistAsync(PostgresDbContext db, Book book, CancellationToken ct)
    {
        var transactionManager = new TransactionManager(db);
        var books = new BookRepository(db);
        var publishers = new PublisherRepository(db);
        var series = new SeriesRepository(db);
        var authors = new AuthorRepository(db);
        var bookAuthors = new BookAuthorsRepository(db);
        var genres = new GenreRepository(db);
        var bookGenres = new BookGenresRepository(db);
        var deweyDecimals = new DeweyDecimalsRepository(db);
        var bookDeweyDecimals = new BookDeweyDecimalsRepository(db);

        await using var transaction = await transactionManager.BeginTransactionAsync(ct);

        var publisher = await publishers.GetOrCreateAsync(book.Publisher, ct);
        book.SetPublisher(publisher);

        var createdSeries = await series.GetOrCreateAsync(book.Series, ct);
        book.SetSeries(createdSeries);

        await books.InsertBookAsync(book, ct);

        var authorIds = await authors.GetOrCreateAuthorsAsync(book.Authors, ct);
        await bookAuthors.InsertBookAuthorsAsync(book.Id.Value, authorIds, ct);

        var genreIds = await genres.GetOrCreateGenresAsync(book.Genres, ct);
        await bookGenres.InsertBookGenresAsync(book.Id.Value, genreIds, ct);

        var deweyIds = await deweyDecimals.GetOrCreateDeweyDecimalsAsync(book.DeweyDecimals, ct);
        await bookDeweyDecimals.InsertBookDeweyDecimalsAsync(book.Id.Value, deweyIds, ct);

        await transaction.CommitAsync(ct);
    }

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
