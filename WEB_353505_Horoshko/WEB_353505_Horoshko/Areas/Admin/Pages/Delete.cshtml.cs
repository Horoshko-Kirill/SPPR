using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353505_Horoshko.Context;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.Areas.Admin.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService _bookService;

        public DeleteModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookService.GetBookByIdAsync((int)id);

            if (book.Data is not null)
            {
                Book = book.Data;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = _bookService.DeleteBookAsync((int)id);

            if (!result.IsCompletedSuccessfully)
            {
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
