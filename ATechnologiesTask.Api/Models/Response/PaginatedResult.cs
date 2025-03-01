namespace PropertyApplication.Core.Base
{
    public class PaginatedResponse<T> : ResponseModel<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PaginatedResponse(List<T> data, string? message = null,
            int count = 0,
            int page = 1,
            int pageSize = 10) : base(data, message)
        {
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

    }
}
