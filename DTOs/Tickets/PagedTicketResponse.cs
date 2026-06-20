namespace TicketSystem.DTOs.Tickets
{
    public class PagedTicketResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public List<TicketResponse> Items { get; set; } = [];
    }
}
