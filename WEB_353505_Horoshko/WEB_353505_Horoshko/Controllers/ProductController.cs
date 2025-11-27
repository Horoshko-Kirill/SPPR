using Microsoft.AspNetCore.Mvc;

namespace WEB_353505_Horoshko.Controllers
{
    public class ProductController : Controller
    {
        private IBookService _bookService;
        private ICategoryService _categoryService;

        public ProductController(ICategoryService categoryService, IBookService bookService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            var bookResponse = await _bookService.GetBookListAsync(category, pageNo);
            if (!bookResponse.Successfull)
            {
                return NotFound(bookResponse.ErrorMessage);
            }

            var categoriesResponse = await _categoryService.GetCategoriesAsync();
            var categories = categoriesResponse.Successfull ? categoriesResponse.Data : new List<Category>();

            ViewData["categories"] = categories;

            string currentCategoryName = "Все";
            if (!string.IsNullOrEmpty(category))
            {
                var currentCategory = categories.FirstOrDefault(c => c.NormalizedName == category);
                currentCategoryName = currentCategory?.Name ?? "Все";
            }

            ViewData["currentCategory"] = currentCategoryName;

            return View(bookResponse.Data);
        }
    }
}
