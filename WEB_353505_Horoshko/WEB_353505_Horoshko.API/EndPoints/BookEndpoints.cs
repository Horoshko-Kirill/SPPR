using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WEB_353505_Horoshko.API.Data;
using WEB_353505_Horoshko.API.Use_Cases;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Domain.Models;
namespace WEB_353505_Horoshko.API.EndPoints;


public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book")
            .WithTags(nameof(Book))
            .DisableAntiforgery()
            .RequireAuthorization("admin");

        group.MapGet("/{id:int}", async Task<Results<Ok<ResponseData<Book>>, NotFound>> (int id, AppDbContext db) =>
        {
            var book = await db.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(new ResponseData<Book>
            {
                Data = book,
                Successfull = true
            });
        })
        .WithName("GetBookById")
        .WithOpenApi();

        group.MapGet("/category/{category:regex(^[a-zA-Z0-9-]*$)?}",
            async (IMediator mediator, string? category, int pageNo = 1, int pageSize = 3) =>
        {
            var data = await mediator.Send(new GetListOfBooks(category, pageNo, pageSize));
            return TypedResults.Ok(data);
        })
        .WithName("GetBooks")
        .WithOpenApi()
        .AllowAnonymous();

        group.MapGet("/all/{category:regex(^[a-zA-Z0-9-]*$)?}",
            async (IMediator mediator, string? category) =>
            {
                var data = await mediator.Send(new GetAllBooks(category));
                return TypedResults.Ok(data);
            })
        .WithName("GetAllBooks")
        .WithOpenApi()
        .AllowAnonymous();


        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, [FromForm] string bookJson, [FromForm] IFormFile? file, AppDbContext db, IMediator mediator) =>
        {

            var book = JsonSerializer.Deserialize<Book>(bookJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var oldBook = await db.Books
           .AsNoTracking()
           .FirstOrDefaultAsync(b => b.Id == id);

            if (oldBook == null)
                return TypedResults.NotFound();

            string? oldImage = oldBook.Image;

            if (file != null && file.Length > 0)
            {
                var newImageUrl = await mediator.Send(new SaveImage(file));
                book.Image = newImageUrl;

                if (!string.IsNullOrEmpty(oldImage))
                {
                    await mediator.Send(new DeleteImage(oldImage));
                }
            }
            else
            {
                book.Image = oldImage;
            }

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

        group.MapPost("/", async ([FromForm] string bookJson, [FromForm] IFormFile? file, AppDbContext db, IMediator mediator) =>
        {

            var book = JsonSerializer.Deserialize<Book>(bookJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (file != null && file.Length > 0)
            {
                var imageUrl = await mediator.Send(new SaveImage(file));
                book.Image = imageUrl;
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{book.Id}",book);
        })
        .WithName("CreateBook")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db, IMediator mediator) =>
        {

            var book = await db.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) 
                return TypedResults.NotFound();

            if (!string.IsNullOrEmpty(book.Image))
            {
                await mediator.Send(new DeleteImage(book.Image));
            }

            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBook")
        .WithOpenApi();
    }
}
