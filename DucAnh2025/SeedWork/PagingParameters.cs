using System.Text.Json.Serialization;

namespace DucAnh2025.SeedWork
{
    public class PagingParameters
    {
        const int maxPageSize = 50;
        [JsonPropertyName("page_number")]
        public int PageNumber { get; set; } = 1;

        public int pageSize { get; set; } = 10;
        [JsonPropertyName("page_size")]
        public int PageSize
        {
            get
            {
                return pageSize;
            }

            set
            {
                pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
