using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Domain.Models;
using WEB_353505_Horoshko.Services.Authentication;
using WEB_353505_Horoshko.UI.Services;
using Xunit;

public class ApiBookServiceTests
{
    private ApiBookService CreateService(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://test")
        };

        var inMemorySettings = new Dictionary<string, string> { { "ItemsPerPage", "3" } };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var loggerMock = new Mock<ILogger<ApiBookService>>();
        var tokenMock = new Mock<ITokenAccessor>();
        tokenMock.Setup(t => t.SetAuthorizationHeaderAsync(It.IsAny<HttpClient>(), false))
                 .Returns(Task.CompletedTask);

        return new ApiBookService(httpClient, configuration, loggerMock.Object, tokenMock.Object);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenHttpOk()
    {
        var expectedBook = new Book { Id = 1, Name = "TestBook" };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ResponseData<Book>
            {
                Successfull = true,
                Data = expectedBook
            })
        };

        var service = CreateService(responseMessage);
        var result = await service.GetBookByIdAsync(1);

        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(expectedBook.Id, result.Data.Id);
        Assert.Equal(expectedBook.Name, result.Data.Name);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsError_WhenHttpNotFound()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
        var service = CreateService(responseMessage);

        var result = await service.GetBookByIdAsync(1);

        Assert.False(result.Successfull);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetBookListAsync_ReturnsList_WhenHttpOk()
    {
        var books = new List<Book>
        {
            new Book { Id = 1, Name = "Book1" },
            new Book { Id = 2, Name = "Book2" },
        };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ResponseData<ListModel<Book>>
            {
                Successfull = true,
                Data = new ListModel<Book>
                {
                    Items = books,
                    CurrentPage = 1,
                    TotalPages = 1
                }
            })
        };

        var service = CreateService(responseMessage);
        var result = await service.GetBookListAsync(null);

        Assert.True(result.Successfull);
        Assert.Equal(books.Count, result.Data.Items.Count);
        Assert.Equal(1, result.Data.CurrentPage);
    }

    [Fact]
    public async Task GetBookListAsync_ReturnsError_WhenHttpFails()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var service = CreateService(responseMessage);

        var result = await service.GetBookListAsync(null);

        Assert.False(result.Successfull);
        Assert.Null(result.Data);
    }
}
