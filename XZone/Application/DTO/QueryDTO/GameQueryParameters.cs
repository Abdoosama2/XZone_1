namespace XZone.Application.DTO.QueryDTO
{
    public class GameQueryParameters
    {

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string?  Search { get; set; }

        public string? Category { get; set; }

        public decimal? MaxPrice { get; set; }

        public decimal? MinPrice { get; set; }

        public string? SortBy { get; set; }

        public string? SortOrder { get; set; } = "asc";
    }
}
