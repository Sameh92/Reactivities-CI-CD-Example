using System.Text.Json;


namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));

            /*
            because this is a custom header, we need to specifically expose this so that our brownnoser will be able to read it.
            If we don't expose it, then it would just be invisible to our clients browser and it won't be able to be retrieved.
            */
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}