namespace AccidentDetectionSysrem.modelsef
{
    public class Detection
    {

        public int Id { get; set; }
        public int? CameraId { get; set; }
        public Camera? Camera { get; set; }

        public string Result { get; set; } 

        public float Confidence { get; set; }

        public string Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;







    }
}
