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
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;

        public IndexModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IList<Book> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var result = await _bookService.GetAllBookListAsync();

            Book = result.Data;
        }
    }
}
