namespace RTCodingExercise.Monolithic.Models
{
    public class PagedList<T> : IPagingInfo
    {
        public PagedList()
        {
        }

        public PagedList(IPagingInfo other)
        {
            FirstItem = other.FirstItem;
            LastItem = other.LastItem;
            PageCount = other.PageCount;
            PageNumber = other.PageNumber;
            TotalItemCount = other.TotalItemCount;
        }

        public List<T>? Items { get; set; }
        public int FirstItem { get; set; }
        public int LastItem { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalItemCount { get; set; }
    }
}
