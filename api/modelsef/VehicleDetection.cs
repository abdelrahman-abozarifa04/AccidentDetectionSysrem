using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccidentDetectionSysrem.modelsef
{
    public class VehicleDetection
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        public string VehicleName { get; set; }


        public int CameraId { get; set; }

        [ForeignKey("CameraId")]
        public virtual Camera Camera { get; set; }

    }
}
