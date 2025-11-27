



namespace WEB_353505_Horoshko.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<Category>> CreateCategoryAsync(Category category, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<List<Category>>> GetCategoriesAsync()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Хорроры", NormalizedName = "horror" },
                new Category { Id = 2, Name = "Фантастика", NormalizedName = "sci-fi" },
                new Category { Id = 3, Name = "Фэнтези", NormalizedName = "fantasy" },
                new Category { Id = 4, Name = "Детективы", NormalizedName = "detective" },
                new Category { Id = 5, Name = "Романы", NormalizedName = "novel" },
                new Category { Id = 6, Name = "Приключения", NormalizedName = "adventure" },
                new Category { Id = 6, Name = "Приключения", NormalizedName = "adventure" },
                new Category { Id = 7, Name = "Научная литература", NormalizedName = "science" },
                new Category { Id = 8, Name = "Исторические книги", NormalizedName = "history" }
            };

            var result = ResponseData<List<Category>>.Success(categories);

            return Task.FromResult(result);
        }

        public Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategorytAsync(int id, Category category)
        {
            throw new NotImplementedException();
        }
    }
}
