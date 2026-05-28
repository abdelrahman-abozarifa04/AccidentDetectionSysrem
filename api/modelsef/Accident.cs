using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccidentDetectionSysrem.modelsef
{
    public class Accident
    {
        [Key]
        public int AccidentId { get; set; }

        public DateTime AccidentTime { get; set; } = DateTime.Now;


        public string? Description { get; set; }

        public string? Location { get; set; }

        [ForeignKey("Camera")]
        public int ?CameraId { get; set; }
        public virtual Camera? Camera { get; set; }

        public virtual List<Notification> Notification { get; set; }





    }
}
 