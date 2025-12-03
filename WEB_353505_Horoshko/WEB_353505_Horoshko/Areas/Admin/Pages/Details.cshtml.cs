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
    public class DetailsModel : PageModel
    {
        private readonly IBookService _bookService;

        public DetailsModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Console.WriteLine(id);

            var book = await _bookService.GetBookByIdAsync(id.Value);

            Console.WriteLine(book.Data.Id);

            if (book.Data is not null)
            {
                Book = book.Data;

                return Page();
            }

            return NotFound();
        }
    }
}
