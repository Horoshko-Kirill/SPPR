using Microsoft.AspNetCore.Mvc;
using WEB_353505_Horoshko.Domain.Help;


public class CartController : Controller
{
    private readonly IBookService _bookService;
    private readonly Cart _cart;

    public CartController(IBookService bookService, Cart cart)
    {
        _bookService = bookService;
        _cart = cart;
    }

    public async Task<IActionResult> Add(int id, string returnUrl)
    {
        var data = await _bookService.GetBookByIdAsync(id);
        if (data.Successfull)
        {
            _cart.AddToCart(data.Data); 
        }
        return Redirect(returnUrl);
    }

    public IActionResult Remove(int id)
    {
        _cart.RemoveItems(id);
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        _cart.ClearAll();
        return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
        var cartItems = _cart.CartItems.Values.ToList(); 
        return View(cartItems);
    }
 
}
