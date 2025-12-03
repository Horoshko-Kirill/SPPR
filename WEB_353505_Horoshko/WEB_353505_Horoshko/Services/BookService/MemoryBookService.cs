
using Microsoft.AspNetCore.Mvc;

namespace WEB_353505_Horoshko.Services.BookService
{
    public class MemoryBookService : IBookService
    {
        private List<Book> _books;
        private List<Category> _categories;
        private IConfiguration _config;

        public MemoryBookService([FromServices] IConfiguration config, ICategoryService categoryService)
        {
            _categories = categoryService.GetCategoriesAsync().Result.Data;

            _config = config;

            SetupData();
        }
        private void SetupData()
        {
            _books = new List<Book>
            {
                new Book {Id = 1, Name = "Обманный бросок", Description = "Исайя влюбился в Кеннеди с первого взгляда, но и представить не мог, что после пьяной ночи проснется с ней, врачом своей команды, в одной постели, подписав брачный договор. Такое утро могло быть самым счастливым в его жизни, если бы не одно но: контракт с бейсбольным клубом, запрещающий случайные связи и все, что может подмочить репутацию команды. Теперь обоим грозит увольнение… если только их чувство не окажется настоящим, а брак – подлинным. Хотя бы до конца сезона.\r\n\r\nСумеет ли Кеннеди полюбить Исайю, или все вернется на круги своя?",
                Category = _categories.Find(c => c.NormalizedName.Equals("novel")), CategoryId = 5, Image = "../images/ОбманныйБросок.jpg"},
                new Book {Id = 2, Name = "Как поймать монстра. Круг третий. Книга 1", Description = "Городское фэнтези, в котором есть таинственные артефакты, кровавые пентаграммы, двустволка с соляными пулями, железная кочерга, шесть ящиков виски и долбаная Ирландия.",
                Category = _categories.Find(c => c.NormalizedName.Equals("fantasy")), CategoryId = 3, Image = "../images/КакПойматьМонстра.jpg"},
                new Book {Id = 3, Name = "Ужасно катастрофический поход в зоопарк", Description = "Катастрофа никогда не приходит одна. Ужасно катастрофическому походу школьников в зоопарк предшествовала целая серия катастроф, и первая из них – потоп в маленькой школе для особенных детей. Кто-то залепил пластилином сливные отверстия в раковинах и открыл краны на все выходные. Школу закрывают, детей переводят в другую, соседнюю, но они не могут с этим смириться и тайно начинают собственное расследование. Они уверены, что потоп был устроен кем-то из взрослых с преступной целью.",
                Category = _categories.Find(c => c.NormalizedName.Equals("detective")), CategoryId = 4, Image = "../images/УжасноКатастрофическийПоходВЗоопарк.jpg"},
                new Book {Id = 4, Name = "Хороших девочек не убивают", Description = "Пять лет назад популярную школьную красавицу Энди Белл убил ее парень Сэл Сингх, а сознавшись в содеянном, покончил с собой. Дело закрыто. Полиция знает, что это сделал Сэл. Весь город знает...\r\n\r\nНо ученица местной школы Пиппа Фитц-Амоби с ними не согласна. Она выбирает эту жуткую трагедию в качестве темы для своего выпускного проекта. Вместе с братом Сэла, вооружившись ноутбуком, диктофоном и большим желанием докопаться до истины, Пиппа начинает свое расследование.",
                Category = _categories.Find(c => c.NormalizedName.Equals("horror")), CategoryId = 1, Image = "../images/ХорошихДевочекНеУбивают.jpg"},
                new Book {Id = 5, Name = "Рассвет Жатвы", Description = "С приближением Пятидесятых Голодных игр страх охватывает все дистрикты Панема. В этом году состоится вторая Квартальная Бойня, и каждый дистрикт обязан отправить в два раза больше трибутов. Хеймитч Эбернети старается не думать об этом, ему хочется, чтобы Жатва поскорее закончилась, тогда он сможет провести время с любимой девушкой и семьей.\r\n\r\nКогда называют имя Хеймитча, его привычный мир рушится. Он отправляется в Капитолий вместе с тремя другими трибутами из Дистрикта-12: девочкой, которая для него как сестра, парнем, зарабатывающим на ставках, и самой заносчивой девушкой в городе. Хеймитч знает, что живым не вернется, и все же продолжает борьбу, понимая, что главные его соперники находятся за пределами смертельной арены…",
                Category = _categories.Find(c => c.NormalizedName.Equals("sci-fi")), CategoryId = 2, Image = "../images/РасветЖатвы.jpg"}
            };
        }
        public Task<ResponseData<Book>> CreateProductAsync(Book product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            int pageSize = _config.GetValue<int>("ItemsPerPage");

            List<Book> books = new List<Book>();
            if (categoryNormalizedName == null)
            {
                books = _books;
            }
            else
            {
                books = _books.FindAll(b => b.Category.NormalizedName.Equals(categoryNormalizedName));
            }

            var totalCount = books.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (pageNo < 1) pageNo = 1;
            if (pageNo > totalPages) pageNo = totalPages;

            var itemsForCurrentPage = books
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var booksListModel = new ListModel<Book>
            {
                Items = itemsForCurrentPage,  
                CurrentPage = pageNo,         
                TotalPages = totalPages,      
            };

            var result = ResponseData<ListModel<Book>>.Success(booksListModel);
            return Task.FromResult(result);
        }

        public Task<ResponseData<Book>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Book product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> GetBookByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> UpdateBookAsync(int id, Book product)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<bool>> DeleteBookAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> CreateBookAsync(Book product)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<List<Book>>> GetAllBookListAsync(string? categoryNormalizedName = null)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> UpdateBookAsync(int id, IFormFile? file, Book product)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> CreateBookAsync(Book product, IFormFile? file)
        {
            throw new NotImplementedException();
        }
    }
}
