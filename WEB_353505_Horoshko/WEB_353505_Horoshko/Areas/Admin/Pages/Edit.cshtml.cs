using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_353505_Horoshko.Context;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.Areas.Admin.Pages
{
    public class EditModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;

        public EditModel(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Console.WriteLine("1");
                return NotFound();
            }

            var book = await _bookService.GetBookByIdAsync((int)id);
            Console.WriteLine(book.Data); 
            if (book == null || book.Data == null)
            {
                Console.WriteLine("2");
                return NotFound();
            }

            Book = book.Data;

            var categories = await _categoryService.GetCategoriesAsync();

            ViewData["CategoryId"] = new SelectList(categories.Data, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetCategoriesAsync();
                ViewData["CategoryId"] = new SelectList(categories.Data, "Id", "Name");
                return Page();
            }

            var result = await _bookService.UpdateBookAsync(Book.Id, ImageFile, Book);

            if (!BookExists(Book.Id).Result)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> BookExists(int id)
        {
            var book = await _bookService.GetBookByIdAsync((int)id);

            return !(book == null && book.Data == null);
        }
    }
}
