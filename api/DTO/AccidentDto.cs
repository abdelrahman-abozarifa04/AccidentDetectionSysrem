namespace AccidentDetectionSysrem.DTO
{
    public class AccidentDto
    {
        public int AccidentId { get; set; }
        public DateTime AccidentTime { get; set; }

        public string? Location { get; set; }
        public string? PredictionClass { get; set; }
        public float? Confidence { get; set; }
    }
}