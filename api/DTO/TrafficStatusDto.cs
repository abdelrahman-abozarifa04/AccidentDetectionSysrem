namespace AccidentDetectionSysrem.DTO
{
    public class TrafficStatusDto
    {
        public int? TrafficStatusId { get; set; }

        public string? Status { get; set; }

        public DateTime RecordedAt { get; set; }

        public int? CameraId { get; set; }

        public string? Location { get; set; } 
    }
}