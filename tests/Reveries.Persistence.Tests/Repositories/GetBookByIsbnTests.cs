using Dapper;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Covers the ISBN lookup — the hot scan path — against real Postgres, including
/// the cross-matching where a scanned ISBN-10/13 can match either stored column.
/// Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class GetBookByIsbnTests : IAsyncLifetime
{
    private const string Isbn13 = "9780451524935";
    private const string Isbn10 = "0451524934";
    private const string OtherIsbn13 = "9780261102217";

    private readonly PostgresContainerFixture _fixture;

    public GetBookByIsbnTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Finds_the_book_when_the_isbn13_matches_directly()
    {
        // Arrange
        var id = Guid.NewGuid();
        await SeedBookAsync(id, "Nineteen Eighty-Four", Isbn13, Isbn10);

        // Act
        var book = await LookupAsync(Isbn.Create(Isbn13), null);

        // Assert
        Assert.NotNull(book);
        Assert.Equal(id, book!.Id.Value);
    }

    [Fact]
    public async Task Finds_the_book_when_a_scanned_isbn_matches_the_stored_isbn10()
    {
        // Arrange — book stored under its ISBN-10 only
        var id = Guid.NewGuid();
        await SeedBookAsync(id, "Nineteen Eighty-Four", isbn13: null, isbn10: Isbn10);

        // Act — the scanned code arrives in the isbn13 parameter but matches isbn10
        var book = await LookupAsync(Isbn.Create(Isbn10), null);

        // Assert
        Assert.NotNull(book);
        Assert.Equal(id, book!.Id.Value);
    }

    [Fact]
    public async Task Finds_the_book_through_the_isbn10_parameter_against_a_stored_isbn13()
    {
        // Arrange — book stored under its ISBN-13 only
        var id = Guid.NewGuid();
        await SeedBookAsync(id, "Nineteen Eighty-Four", isbn13: Isbn13, isbn10: null);

        // Act — value supplied via the isbn10 parameter still matches the stored isbn13
        var book = await LookupAsync(null, Isbn.Create(Isbn13));

        // Assert
        Assert.NotNull(book);
        Assert.Equal(id, book!.Id.Value);
    }

    [Fact]
    public async Task Returns_null_when_no_stored_isbn_matches()
    {
        // Arrange
        await SeedBookAsync(Guid.NewGuid(), "Nineteen Eighty-Four", Isbn13, Isbn10);

        // Act
        var book = await LookupAsync(Isbn.Create(OtherIsbn13), null);

        // Assert
        Assert.Null(book);
    }

    [Fact]
    public async Task Maps_snake_case_columns_to_the_domain_book()
    {
        // Arrange
        var id = Guid.NewGuid();
        await SeedBookWithDetailsAsync(id);

        // Act
        var book = await LookupAsync(Isbn.Create(Isbn13), null);

        // Assert — these come from snake_case columns via the underscore convention
        Assert.NotNull(book);
        Assert.Equal(328, book!.Pages);
        Assert.True(book.IsRead);
        Assert.Equal("http://img/cover.jpg", book.CoverImageUrl);
        Assert.Equal("http://img/thumb.jpg", book.ImageThumbnailUrl);
    }

    private async Task<Book?> LookupAsync(Isbn? isbn13, Isbn? isbn10)
    {
        await using var dbContext = _fixture.NewDbContext();
        return await new BookRepository(dbContext).GetBookByIsbnAsync(isbn13, isbn10, CancellationToken.None);
    }

    private async Task SeedBookAsync(Guid id, string title, string? isbn13, string? isbn10)
    {
        await using var connection = await _fixture.DataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            "INSERT INTO library.books (id, title, isbn13, isbn10) VALUES (@Id, @Title, @Isbn13, @Isbn10)",
            new { Id = id, Title = title, Isbn13 = isbn13, Isbn10 = isbn10 });
    }

    private async Task SeedBookWithDetailsAsync(Guid id)
    {
        await using var connection = await _fixture.DataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            """
            INSERT INTO library.books (id, title, isbn13, isbn10, page_count, is_read, image_url, image_thumbnail)
            VALUES (@Id, @Title, @Isbn13, @Isbn10, @PageCount, @IsRead, @ImageUrl, @ImageThumbnail)
            """,
            new
            {
                Id = id,
                Title = "Nineteen Eighty-Four",
                Isbn13,
                Isbn10,
                PageCount = 328,
                IsRead = true,
                ImageUrl = "http://img/cover.jpg",
                ImageThumbnail = "http://img/thumb.jpg"
            });
    }
}