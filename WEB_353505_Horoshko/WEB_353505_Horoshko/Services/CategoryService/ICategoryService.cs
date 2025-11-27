namespace WEB_353505_Horoshko.Services.CategoryService
{
    public interface ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoriesAsync();
        public Task<ResponseData<Category>> GetCategoryByIdAsync(int id);
        public Task UpdateCategorytAsync(int id, Category category);
        public Task DeleteCategoryAsync(int id);
        public Task<ResponseData<Category>> CreateCategoryAsync(Category category, IFormFile? formFile);
    }
}
