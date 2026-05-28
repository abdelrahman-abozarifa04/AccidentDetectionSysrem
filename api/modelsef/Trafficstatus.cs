using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccidentDetectionSysrem.modelsef
{
    public class Trafficstatus
    {

        [Key]
        public int TrafficStatusId { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Description { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;

        public string? Location { get; set; }
        [ForeignKey("Cameras")]
        public int ?CameraId { get; set; }
        public virtual Camera? Cameras
        { get; set; }
        public virtual List<Notification> Notifications { get; set; }





    }
}
