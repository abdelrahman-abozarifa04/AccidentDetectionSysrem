using AccidentDetectionSystem.efmodels;
using Microsoft.EntityFrameworkCore;

namespace AccidentDetectionSysrem.modelsef
{
    public class AccidentDetectionSystemContect : DbContext
    {
        public AccidentDetectionSystemContect(
            DbContextOptions<AccidentDetectionSystemContect> options
        ) : base(options)
        {
        }

        public virtual DbSet<Camera> Cameras { get; set; }
        public virtual DbSet<Accident> Accidents { get; set; }
        public virtual DbSet<VehicleDetection> VehicleDetections { get; set; }
        public virtual DbSet<Trafficstatus> Trafficstatuses { get; set; }
        public virtual DbSet<user> Users { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<UserNotifiction> UserNotifictions { get; set; }
        public DbSet<Detection> Detections { get; set; }
    }
}