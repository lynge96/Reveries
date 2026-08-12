using Dapper;
using Reveries.Domain.Models;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Covers GetAllBooksAsync against real Postgres: the empty case, multiple rows,
/// and that each book hydrates its own relations without leaking across rows.
/// Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class GetAllBooksTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public GetAllBooksTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Returns_an_empty_list_when_there_are_no_books()
    {
        // Act
        var books = await GetAllAsync();

        // Assert
        Assert.Empty(books);
    }

    [Fact]
    public async Task Returns_every_book_with_its_own_relations()
    {
        // Arrange — one book with a publisher, one without; distinct authors each
        var orwellBookId = Guid.NewGuid();
        var huxleyBookId = Guid.NewGuid();

        await SeedBookAsync(orwellBookId, "Nineteen Eighty-Four",
            publisher: (Guid.NewGuid(), "Signet Classics"),
            author: (Guid.NewGuid(), "George", "Orwell"));

        await SeedBookAsync(huxleyBookId, "Brave New World",
            publisher: null,
            author: (Guid.NewGuid(), "Aldous", "Huxley"));

        // Act
        var books = await GetAllAsync();

        // Assert
        Assert.Equal(2, books.Count);

        var orwellBook = books.Single(b => b.Id.Value == orwellBookId);
        Assert.Equal("Signet Classics", orwellBook.Publisher!.Name);
        Assert.Equal("george orwell", Assert.Single(orwellBook.Authors).NormalizedName);

        var huxleyBook = books.Single(b => b.Id.Value == huxleyBookId);
        Assert.Null(huxleyBook.Publisher);
        Assert.Null(huxleyBook.Series);
        Assert.Equal("aldous huxley", Assert.Single(huxleyBook.Authors).NormalizedName);
    }

    private async Task<List<Book>> GetAllAsync()
    {
        await using var dbContext = _fixture.NewDbContext();
        return await new BookRepository(dbContext).GetAllBooksAsync(CancellationToken.None);
    }

    private async Task SeedBookAsync(
        Guid bookId,
        string title,
        (Guid Id, string Name)? publisher = null,
        (Guid Id, string First, string Last)? author = null)
    {
        await using var connection = await _fixture.DataSource.OpenConnectionAsync();

        Guid? publisherId = null;
        if (publisher is { } p)
        {
            publisherId = p.Id;
            await connection.ExecuteAsync(
                "INSERT INTO library.publishers (id, name) VALUES (@Id, @Name)",
                new { p.Id, p.Name });
        }

        await connection.ExecuteAsync(
            "INSERT INTO library.books (id, title, publisher_id) VALUES (@Id, @Title, @PublisherId)",
            new { Id = bookId, Title = title, PublisherId = publisherId });

        if (author is { } a)
        {
            await connection.ExecuteAsync(
                "INSERT INTO library.authors (id, normalized_name, first_name, last_name) VALUES (@Id, @Norm, @First, @Last)",
                new { a.Id, Norm = $"{a.First} {a.Last}".ToLowerInvariant(), a.First, a.Last });

            await connection.ExecuteAsync(
                "INSERT INTO library.books_authors (book_id, author_id) VALUES (@BookId, @AuthorId)",
                new { BookId = bookId, AuthorId = a.Id });
        }
    }
}