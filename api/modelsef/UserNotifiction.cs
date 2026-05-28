using AccidentDetectionSysrem.modelsef;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccidentDetectionSystem.efmodels
{

    [PrimaryKey(nameof(UserId), nameof(NotificationId))]
    public class UserNotifiction
    {


        [ForeignKey("users")]
        public int UserId { get; set; }
        [ForeignKey("notification")]
        public int NotificationId { get; set; }




        public virtual Notification notification { get; set; }
        public virtual user users { get; set; }

    }


}
