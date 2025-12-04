using Microsoft.EntityFrameworkCore;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.API.Data
{
    public class DbInitializer
    {


        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Categories.AnyAsync() || await context.Books.AnyAsync())
            {
                return;
            }

            var baseUrl = app.Configuration["AppSettings:BaseUrl"];

            var categories = new List<Category>
            {
                new Category { Name = "Хорроры", NormalizedName = "horror" },
                new Category { Name = "Фантастика", NormalizedName = "sci-fi" },
                new Category { Name = "Фэнтези", NormalizedName = "fantasy" },
                new Category { Name = "Детективы", NormalizedName = "detective" },
                new Category { Name = "Романы", NormalizedName = "novel" },
                new Category { Name = "Приключения", NormalizedName = "adventure" },
                new Category { Name = "Научная литература", NormalizedName = "science" },
                new Category { Name = "Исторические книги", NormalizedName = "history" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var horrorCategory = await context.Categories.FirstAsync(c => c.NormalizedName == "horror");
            var scifiCategory = await context.Categories.FirstAsync(c => c.NormalizedName == "sci-fi");
            var fantasyCategory = await context.Categories.FirstAsync(c => c.NormalizedName == "fantasy");
            var detectiveCategory = await context.Categories.FirstAsync(c => c.NormalizedName == "detective");
            var novelCategory = await context.Categories.FirstAsync(c => c.NormalizedName == "novel");

            var books = new List<Book>
            {
                new Book
                {
                    Name = "Обманный бросок",
                    Description = "Исайя влюбился в Кеннеди с первого взгляда, но и представить не мог, что после пьяной ночи проснется с ней, врачом своей команды, в одной постели, подписав брачный договор. Такое утро могло быть самым счастливым в его жизни, если бы не одно но: контракт с бейсбольным клубом, запрещающий случайные связи и все, что может подмочить репутацию команды. Теперь обоим грозит увольнение… если только их чувство не окажется настоящим, а брак – подлинным. Хотя бы до конца сезона.\r\n\r\nСумеет ли Кеннеди полюбить Исайю, или все вернется на круги своя?",
                    CategoryId = novelCategory.Id,
                    Image = $"{baseUrl}/Images/ОбманныйБросок.jpg",
                    Price = 10
                },
                new Book
                {
                    Name = "Как поймать монстра. Круг третий. Книга 1",
                    Description = "Городское фэнтези, в котором есть таинственные артефакты, кровавые пентаграммы, двустволка с соляными пулями, железная кочерга, шесть ящиков виски и долбаная Ирландия.",
                    CategoryId = fantasyCategory.Id,
                    Image = $"{baseUrl}/Images/КакПойматьМонстра.jpg",
                    Price = 20
                },
                new Book
                {
                    Name = "Ужасно катастрофический поход в зоопарк",
                    Description = "Катастрофа никогда не приходит одна. Ужасно катастрофическому походу школьников в зоопарку предшествовала целая серия катастроф, и первая из них – потоп в маленькой школе для особенных детей. Кто-то залепил пластилином сливные отверстия в раковинах и открыл краны на все выходные. Школу закрывают, детей переводят в другую, соседнюю, но они не могут с этим смириться и тайно начинают собственное расследование. Они уверены, что потоп был устроен кем-то из взрослых с преступной целью.",
                    CategoryId = detectiveCategory.Id,
                    Image = $"{baseUrl}/Images/УжасноКатастрофическийПоходВЗоопарк.jpg",
                    Price = 30
                },
                new Book
                {
                    Name = "Хороших девочек не убивают",
                    Description = "Пять лет назад популярную школьную красавицу Энди Белл убил ее парень Сэл Сингх, а сознавшись в содеянном, покончил с собой. Дело закрыто. Полиция знает, что это сделал Сэл. Весь город знает...\r\n\r\nНо ученица местной школы Пиппа Фитц-Амоби с ними не согласна. Она выбирает эту жуткую трагедию в качестве темы для своего выпускного проекта. Вместе с братом Сэла, вооружившись ноутбуком, диктофоном и большим желанием докопаться до истины, Пиппа начинает свое расследование.",
                    CategoryId = horrorCategory.Id,
                    Image = $"{baseUrl}/Images/ХорошихДевочекНеУбивают.jpg",
                    Price = 40
                },
                new Book
                {
                    Name = "Рассвет Жатвы",
                    Description = "С приближением Пятидесятых Голодных игр страх охватывает все дистрикты Панема. В этом году состоится вторая Квартальная Бойня, и каждый дистрикт обязан отправить в два раза больше трибутов. Хеймитч Эбернети старается не думать об этом, ему хочется, чтобы Жатва поскорее закончилась, тогда он сможет провести время с любимой девушкой и семьей.\r\n\r\nКогда называют имя Хеймитча, его привычный мир рушится. Он отправляется в Капитолий вместе с тремя другими трибутами из Дистрикта-12: девочкой, которая для него как сестра, парнем, зарабатывающим на ставках, и самой заносчивой девушкой в городе. Хеймитч знает, что живым не вернется, и все же продолжает борьбу, понимая, что главные его соперники находятся за пределами смертельной арены…",
                    CategoryId = scifiCategory.Id,
                    Image = $"{baseUrl}/Images/РасветЖатвы.jpg",
                    Price = 50
                }
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }
    }
}
