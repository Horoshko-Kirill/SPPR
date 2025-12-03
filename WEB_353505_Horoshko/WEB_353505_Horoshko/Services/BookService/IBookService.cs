namespace WEB_353505_Horoshko.Services.BookService
{
    public interface IBookService
    {
        public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1);
        public Task<ResponseData<List<Book>>> GetAllBookListAsync(string? categoryNormalizedName = null);

        public Task<ResponseData<Book>> GetBookByIdAsync(int id);
        public Task<ResponseData<Book>> UpdateBookAsync(int id, IFormFile? file, Book product);
        public Task<ResponseData<bool>> DeleteBookAsync(int id);
        public Task<ResponseData<Book>> CreateBookAsync(Book product, IFormFile? file);
    }
}
