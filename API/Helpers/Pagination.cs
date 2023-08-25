namespace API.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        /// <summary>
        /// How many results are there in total, not just on this page, after applying the requested filters. 
        /// </summary>
        public int Count { get; set; } 
        // We retrieve paginated results from the DB, instead of all results and then sending the relevant ones to the user. Thus, we do not know the total count. We need another method that explicitly counts all viable results in the DB, without retrieving them.

        public IReadOnlyList<T> Data { get; set; }

    }
}