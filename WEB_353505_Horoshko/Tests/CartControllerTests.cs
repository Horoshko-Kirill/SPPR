using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WEB_353505_Horoshko.Domain.Help;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Services.BookService;
using WEB_353505_Horoshko.Domain.Models;

public class CartControllerTests
{
    private readonly IBookService _bookService;
    private readonly Cart _cart;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _bookService = Substitute.For<IBookService>();
        _cart = Substitute.For<Cart>(); 
        _controller = new CartController(_bookService, _cart);
    }


    [Fact]
    public void Index_ReturnsView_WithCartItemsList()
    {
        var items = new Dictionary<int, CartItem>
        {
            { 1, new CartItem { Item = new Book { Id = 1, Name="A"}, Quantity = 2 } }
        };

        _cart.CartItems.Returns(items);

        var result = _controller.Index() as ViewResult;

        Assert.NotNull(result);
        Assert.IsType<List<CartItem>>(result.Model);

        var model = result.Model as List<CartItem>;
        Assert.Single(model);
        Assert.Equal(1, model[0].Item.Id);
    }

    [Fact]
    public async Task Add_WhenProductFound_AddsToCart_AndRedirects()
    {
        var book = new Book { Id = 10, Name = "Test" };
        _bookService.GetBookByIdAsync(10)
            .Returns(new ResponseData<Book> { Successfull = true, Data = book });

        var result = await _controller.Add(10, "/return/url");

        await _bookService.Received().GetBookByIdAsync(10);
        _cart.Received().AddToCart(book);
        Assert.IsType<RedirectResult>(result);
        Assert.Equal("/return/url", (result as RedirectResult).Url);
    }

    [Fact]
    public void Remove_RemovesItem_AndRedirectsToIndex()
    {
        var result = _controller.Remove(5) as RedirectToActionResult;

        _cart.Received().RemoveItems(5);
        Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public void Clear_ClearsCart_AndRedirectsToIndex()
    {
        var result = _controller.Clear() as RedirectToActionResult;

        _cart.Received().ClearAll();
        Assert.Equal("Index", result.ActionName);
    }
}
