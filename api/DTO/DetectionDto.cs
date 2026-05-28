namespace AccidentDetectionSysrem.DTO
{
    public class DetectionDto
    {
        public int Id { get; set; }

        public string Result { get; set; }

        public float Confidence { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CameraId { get; set; }

        public string Location { get; set; }
    }

    public class ClearStatusDto
    {
        public int CameraId { get; set; }

        public PredictionResultDto? Accident { get; set; }

        public PredictionResultDto? Traffic { get; set; }
    }

    public class PredictionResultDto
    {
        public string? Location { get; set; }

        public string? PredictionClass { get; set; }

        public float Confidence { get; set; }
    }
}