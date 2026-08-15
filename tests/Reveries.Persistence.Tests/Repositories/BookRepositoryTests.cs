using Dapper;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Exercises the handwritten Dapper SQL and the books_view hydration against a
/// real Postgres. Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class BookRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public BookRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetBookByIdAsync_hydrates_the_full_aggregate_from_the_view()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        await SeedFullBookAsync(bookId);

        await using var dbContext = _fixture.NewDbContext();
        var repository = new BookRepository(dbContext);

        // Act
        var book = await repository.GetBookByIdAsync(bookId, CancellationToken.None);

        // Assert
        Assert.NotNull(book);
        Assert.Equal(bookId, book.Id.Value);
        Assert.Equal("Nineteen Eighty-Four", book.Title.Value);
        Assert.Equal("9780451524935", book.Isbn13!.Value);
        Assert.Equal("0451524934", book.Isbn10!.Value);
        Assert.Equal(328, book.Pages);
        Assert.Equal("English", book.Language);
        Assert.Equal("1949", book.PublicationDate);

        Assert.Equal("Signet Classics", book.Publisher!.Name);
        Assert.Equal("Modern Classics", book.Series!.Name);
        Assert.Equal(1, book.SeriesNumber);

        Assert.Equal(2, book.Authors.Count);
        Assert.Contains(book.Authors, a => a.NormalizedName == "george orwell");
        Assert.Contains(book.Authors, a => a.NormalizedName == "aldous huxley");

        Assert.Equal(2, book.Genres.Count);
        Assert.Contains(book.Genres, g => g.Value == "Dystopia");
        Assert.Contains(book.Genres, g => g.Value == "Fantasy");

        var dewey = Assert.Single(book.DeweyDecimals);
        Assert.Equal("823", dewey.Code);
    }

    [Fact]
    public async Task GetBookByIdAsync_returns_null_when_the_book_does_not_exist()
    {
        // Arrange
        await using var dbContext = _fixture.NewDbContext();
        var repository = new BookRepository(dbContext);

        // Act
        var book = await repository.GetBookByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(book);
    }

    private async Task SeedFullBookAsync(Guid bookId)
    {
        var publisherId = Guid.NewGuid();
        var seriesId = Guid.NewGuid();
        var orwellId = Guid.NewGuid();
        var huxleyId = Guid.NewGuid();

        await using var connection = await _fixture.DataSource.OpenConnectionAsync();

        await connection.ExecuteAsync(
            "INSERT INTO library.publishers (id, name) VALUES (@Id, @Name)",
            new { Id = publisherId, Name = "Signet Classics" });

        await connection.ExecuteAsync(
            "INSERT INTO library.series (id, name) VALUES (@Id, @Name)",
            new { Id = seriesId, Name = "Modern Classics" });

        await connection.ExecuteAsync(
            "INSERT INTO library.authors (id, normalized_name, first_name, last_name) VALUES (@Id, @Norm, @First, @Last)",
            new[]
            {
                new { Id = orwellId, Norm = "george orwell", First = "George", Last = "Orwell" },
                new { Id = huxleyId, Norm = "aldous huxley", First = "Aldous", Last = "Huxley" }
            });

        await connection.ExecuteAsync(
            "INSERT INTO library.genres (id, name) VALUES (@Id, @Name)",
            new[]
            {
                new { Id = 1, Name = "Dystopia" },
                new { Id = 2, Name = "Fantasy" }
            });

        await connection.ExecuteAsync(
            "INSERT INTO library.dewey_decimals (id, code) VALUES (@Id, @Code)",
            new { Id = 1, Code = "823" });

        await connection.ExecuteAsync(
            """
            INSERT INTO library.books
                (id, title, isbn13, isbn10, series_number, publication_date, page_count,
                 language, publisher_id, series_id)
            VALUES
                (@Id, @Title, @Isbn13, @Isbn10, @SeriesNumber, @PublicationDate, @PageCount,
                 @Language, @PublisherId, @SeriesId)
            """,
            new
            {
                Id = bookId,
                Title = "Nineteen Eighty-Four",
                Isbn13 = "9780451524935",
                Isbn10 = "0451524934",
                SeriesNumber = 1,
                PublicationDate = "1949",
                PageCount = 328,
                Language = "English",
                PublisherId = publisherId,
                SeriesId = seriesId
            });

        await connection.ExecuteAsync(
            "INSERT INTO library.books_authors (book_id, author_id) VALUES (@BookId, @AuthorId)",
            new[]
            {
                new { BookId = bookId, AuthorId = orwellId },
                new { BookId = bookId, AuthorId = huxleyId }
            });

        await connection.ExecuteAsync(
            "INSERT INTO library.books_genres (book_id, genre_id) VALUES (@BookId, @GenreId)",
            new[]
            {
                new { BookId = bookId, GenreId = 1 },
                new { BookId = bookId, GenreId = 2 }
            });

        await connection.ExecuteAsync(
            "INSERT INTO library.books_dewey_decimals (book_id, dewey_decimal_id) VALUES (@BookId, @DeweyId)",
            new { BookId = bookId, DeweyId = 1 });
    }
}