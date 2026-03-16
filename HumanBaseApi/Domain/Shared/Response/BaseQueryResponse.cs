namespace Domain.Shared.Response
{
    public class BaseQueryResponse<T>
    {
        public T Data { get; set; }
        public string Search { get; set; }
        public int CurrentPage { get; set; }
        public decimal PageCount { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool IsPaginated { get; set; }

        public BaseQueryResponse(T data)
        {
            Data = data;
        }

        public static BaseQueryResponse<IEnumerable<T>> CreatePaginated<T>(
            IEnumerable<T> data,
            int totalRecords,
            int page,
            int pageSize,
            string search = null)
        {
            var pageCount = (decimal)Math.Ceiling((double)totalRecords / pageSize);

            return new BaseQueryResponse<IEnumerable<T>>(data)
            {
                Search = search,
                CurrentPage = page,
                PageSize = pageSize,
                PageCount = pageCount,
                HasNextPage = page < pageCount,
                HasPreviousPage = page > 1,
                IsPaginated = true
            };
        }
    }
}