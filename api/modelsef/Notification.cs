using AccidentDetectionSystem.efmodels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccidentDetectionSysrem.modelsef
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public int? AccidentId { get; set; }
        [ForeignKey("AccidentId")]
        public virtual Accident Accident { get; set; }


        public int? TrafficStatusId { get; set; }
        [ForeignKey("TrafficStatusId")]
        public virtual Trafficstatus TrafficStatus { get; set; }

        public virtual List<UserNotifiction> Notifictionsuser { get; } = new List<UserNotifiction>();

















    }
}
