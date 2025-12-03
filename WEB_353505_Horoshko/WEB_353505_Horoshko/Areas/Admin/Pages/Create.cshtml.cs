using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_353505_Horoshko.Context;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.Areas.Admin.Pages
{
    public class CreateModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        public CreateModel(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var responseCategories = await _categoryService.GetCategoriesAsync();

            ViewData["CategoryId"] = new SelectList(responseCategories.Data, "Id", "Name");
                return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var responseCategories = await _categoryService.GetCategoriesAsync();
                ViewData["CategoryId"] = new SelectList(responseCategories.Data, "Id", "Name");

                return Page();
            }

            var response = await _bookService.CreateBookAsync(Book, ImageFile);
            
            if (response.Successfull)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
