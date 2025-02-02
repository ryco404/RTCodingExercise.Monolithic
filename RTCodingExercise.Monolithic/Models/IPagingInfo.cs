namespace RTCodingExercise.Monolithic.Models
{
    public interface IPagingInfo
    {
        public int FirstItem { get; set; }
        public int LastItem { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalItemCount { get; set; }
    }
}
