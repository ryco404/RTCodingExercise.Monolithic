namespace RTCodingExercise.Monolithic.Models
{
    public class PlateReservationResponseJson 
    {
        public bool isReserved { get; set; }
        public bool success { get; set; }
        public string? errorMessage { get; set; }

        public PlateReservationResponseJson(bool isReserved, bool success, string? errorMessage)
        {
            this.isReserved = isReserved;
            this.success = success;
            this.errorMessage = errorMessage;
        }
    }
}