using MediatR;
using Microsoft.EntityFrameworkCore;
using WEB_353505_Horoshko.API.Data;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Domain.Models;

namespace WEB_353505_Horoshko.API.Use_Cases
{
    public sealed record GetListOfBooks(
        string? CategoryNormalizedName,
        int PageNo = 1,
        int PageSize = 3
    ) : IRequest<ResponseData<ListModel<Book>>>;

    public sealed record GetAllBooks(
    string? CategoryNormalizedName
    ) : IRequest<ResponseData<List<Book>>>;

    public class GetListOfBooksHandler : IRequestHandler<GetListOfBooks, ResponseData<ListModel<Book>>>
    {
        private readonly AppDbContext _db;
        private readonly int _maxPageSize = 20;

        public GetListOfBooksHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseData<ListModel<Book>>> Handle(GetListOfBooks request, CancellationToken cancellationToken)
        {
            if (request.PageSize <= 0) request = request with { PageSize = 3 };
            if (request.PageSize > _maxPageSize) request = request with { PageSize = _maxPageSize };
            if (request.PageNo <= 0) request = request with { PageNo = 1 };

            IQueryable<Book> query = _db.Books.Include(b => b.Category);

            if (!string.IsNullOrEmpty(request.CategoryNormalizedName))
            {
                query = query.Where(b => b.Category.NormalizedName == request.CategoryNormalizedName);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            if (request.PageNo > totalPages && totalPages > 0)
            {
                request = request with { PageNo = totalPages };
            }

            var items = await query
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var listModel = new ListModel<Book>
            {
                Items = items,
                CurrentPage = request.PageNo,
                TotalPages = totalPages
            };

            return ResponseData<ListModel<Book>>.Success(listModel);
        }
    }

    public class GetAllBooksHandler : IRequestHandler<GetAllBooks, ResponseData<List<Book>>>
    {
        private readonly AppDbContext _db;

        public GetAllBooksHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseData<List<Book>>> Handle(GetAllBooks request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Book> query = _db.Books.Include(b => b.Category);

                if (!string.IsNullOrEmpty(request.CategoryNormalizedName))
                {
                    query = query.Where(b => b.Category.NormalizedName == request.CategoryNormalizedName);
                }

                var items = await query.ToListAsync(cancellationToken);

                return ResponseData<List<Book>>.Success(items);
            }
            catch (Exception ex)
            {
                return ResponseData<List<Book>>.Error(ex.Message, new List<Book>());
            }
        }
    }
}

