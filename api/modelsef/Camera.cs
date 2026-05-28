using System.ComponentModel.DataAnnotations;

namespace AccidentDetectionSysrem.modelsef
{
    public class Camera
    {
        [Key]
        public int CameraId { get; set; }

        [StringLength(50)]
        public string? CameraName { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual List<Accident> Accidents { get; set; }

        public virtual List<Trafficstatus> TrafficStatuses { get; set; }

        public virtual List<VehicleDetection> VehicleDetections { get; set; }

    }
}
