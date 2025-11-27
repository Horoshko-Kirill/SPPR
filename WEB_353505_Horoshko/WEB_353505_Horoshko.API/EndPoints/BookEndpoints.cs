using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WEB_353505_Horoshko.API.Data;
using WEB_353505_Horoshko.Domain.Entities;
using MediatR;
using WEB_353505_Horoshko.API.Use_Cases;
namespace WEB_353505_Horoshko.API.EndPoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book").WithTags(nameof(Book));

        group.MapGet("/{category:regex(^[a-zA-Z0-9-]*$)?}",
            async (IMediator mediator, string? category, int pageNo = 1, int pageSize = 3) =>
        {
            var data = await mediator.Send(new GetListOfBooks(category, pageNo, pageSize));
            return TypedResults.Ok(data);
        })
        .WithName("GetBooks")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Book>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Books.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Book model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBookById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Book book, AppDbContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, book.Id)
                    .SetProperty(m => m.Name, book.Name)
                    .SetProperty(m => m.Description, book.Description)
                    .SetProperty(m => m.CategoryId, book.CategoryId)
                    .SetProperty(m => m.Image, book.Image)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBook")
        .WithOpenApi();

        group.MapPost("/", async (Book book, AppDbContext db) =>
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{book.Id}",book);
        })
        .WithName("CreateBook")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBook")
        .WithOpenApi();
    }
}
