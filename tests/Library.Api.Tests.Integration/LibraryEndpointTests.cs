using System.Net;
using Bogus;
using FluentAssertions;
using Library.Api.Models;

namespace Library.Api.Tests.Integration;

public class LibraryEndpointTests : IClassFixture<LibraryApiFactory>, IAsyncLifetime
{
	private readonly LibraryApiFactory _factory;
	public LibraryEndpointTests(LibraryApiFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task CreateBook_ResturnsCreated_WhenDataIsCorrect()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();

		// Act
		var result = await client.PostAsJsonAsync("/books", book);
		var createdBook = await result.Content.ReadFromJsonAsync<Book>();

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.Created);
		createdBook.Should().BeEquivalentTo(book);
		result.Headers.Location.Should().Be($"/books/{book.Isbn}");
	}

	[Fact]
	public async Task CreateBook_ReturnsBadRequest_WhenIsbnIsInvalid()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		book.Isbn = "invalid";

		// Act
		var result = await client.PostAsJsonAsync("/books", book);
		var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
		var error = errors!.Single();
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		error.PropertyName.Should().Be("Isbn");
		error.ErrorMessage.Should().Be("Value was not a valid ISBN-13");
	}

	[Fact]
	public async Task CreateBook_ReturnsBadRequest_WhenBookExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();

		// Act
		await client.PostAsJsonAsync("/books", book);
		var result = await client.PostAsJsonAsync("/books", book);
		var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
		var error = errors!.Single();
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		error.PropertyName.Should().Be("Isbn");
		error.ErrorMessage.Should().Be("A book with this ISBN-13 already exists");
	}

	[Fact]
	public async Task GetBook_ReturnsBook_WhenBookExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);

		// Act
		var result = await client.GetAsync($"/books/{book.Isbn}");
		var existingBook = await result.Content.ReadFromJsonAsync<Book>();
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.OK);
		existingBook.Should().BeEquivalentTo(book);
	}

	[Fact]
	public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExists()
	{
		// Arrange
		var client = _factory.CreateClient();

		// Act
		var result = await client.GetAsync($"/books/{Guid.NewGuid()}");
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task GetBooksWithSearchTerm_ReturnsBooks_WhenBooksExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);
		var books = new List<Book> { book };

		// Act
		var result = await client.GetAsync($"/books?searchTerm={book.Title}");
		var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.OK);
		returnedBooks.Should().BeEquivalentTo(books);
	}

	[Fact]
	public async Task GetBooksWithSearchTerm_ReturnsNoBooks_WhenBooksDoNotExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);

		// Act
		var result = await client.GetAsync($"/books?searchTerm={Guid.NewGuid()}");
		var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();
		
		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.OK);
		returnedBooks.Should().BeEmpty();
	}

	[Fact]
	public async Task UpdateBook_UpdatesBook_WhenDataIsCorrect()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);

		// Act
		book.Title = "Updated Title";
		var result = await client.PutAsJsonAsync($"/books/{book.Isbn}", book);
		var updatedBook = await result.Content.ReadFromJsonAsync<Book>();

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.OK);
		updatedBook.Should().BeEquivalentTo(book);
	}

	[Fact]
	public async Task UpdateBook_DoesNotUpdateBook_WhenDataIsIncorrect()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);

		// Act
		book.PageCount = 0;
		var result = await client.PutAsJsonAsync($"/books/{book.Isbn}", book);
		var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
		var error = errors!.Single();

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		error.PropertyName.Should().Be("PageCount");
		error.ErrorMessage.Should().Be("'Page Count' must be greater than '0'.");
	}

	[Fact]
	public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();

		// Act
		var result = await client.PutAsJsonAsync($"/books/{book.Isbn}", book);

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task DeleteBook_ReturnsNotContent_WhenBookExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();
		await client.PostAsJsonAsync("/books", book);

		// Act
		var result = await client.DeleteAsync($"/books/{book.Isbn}");

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExists()
	{
		// Arrange
		var client = _factory.CreateClient();
		var book = GenerateBook();

		// Act
		var result = await client.DeleteAsync($"/books/{book.Isbn}");

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

    private static Book GenerateBook()
    {
		var faker = new Faker();
		return new Book
		{
			Isbn = faker.Random.Long(1000000000000, 9999999999999).ToString(),
			Title = faker.Lorem.Sentence(),
			ShortDescription = faker.Lorem.Paragraph(),
			Author = faker.Person.FullName,
			PageCount = faker.Random.Number(100, 500),
			ReleaseDate = faker.Date.Past()
		};
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}
