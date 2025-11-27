namespace WEB_353505_Horoshko.Services.BookService
{
    public interface IBookService
    {
        public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1);
        public Task<ResponseData<Book>> GetProductByIdAsync(int id);
        public Task UpdateProductAsync(int id, Book product, IFormFile? formFile);
        public Task DeleteProductAsync(int id);
        public Task<ResponseData<Book>> CreateProductAsync(Book product, IFormFile? formFile);
    }
}
