using AccidentDetectionSysrem.DTO;
using AccidentDetectionSysrem.modelsef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccidentDetectionSysrem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficStatusController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect db;

        public TrafficStatusController(AccidentDetectionSystemContect db)
        {
            this.db = db;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trafficList = await db.Trafficstatuses
                .Include(t => t.Cameras)
                .Select(t => new TrafficStatusDto
                {
                    TrafficStatusId = t.TrafficStatusId,
                    Status = t.Status ?? "Normal",
                    RecordedAt = t.RecordedAt == default ? DateTime.Now : t.RecordedAt,
                    CameraId = t.CameraId,
                    Location = t.Location ?? "Unknown"
                })
                .ToListAsync();

            return Ok(trafficList);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var traffic = await db.Trafficstatuses
                .Include(t => t.Cameras)
                .Where(t => t.TrafficStatusId == id)
                .Select(t => new TrafficStatusDto
                {
                    TrafficStatusId = t.TrafficStatusId,
                    Status = t.Status ?? "Normal",
                    RecordedAt = t.RecordedAt == default ? DateTime.Now : t.RecordedAt,
                    CameraId = t.CameraId,
                    Location = t.Location ?? "Unknown"
                })
                .FirstOrDefaultAsync();

            if (traffic == null)
                return Ok(new
                {
                    message = "No traffic data found",
                    data = (object)null
                });

            return Ok(traffic);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(TrafficStatusDto dto)
        {
            var traffic = new Trafficstatus
            {
                Status = dto.Status ?? "Normal",
                Description = "AI / Manual Detection",
                RecordedAt = dto.RecordedAt == default ? DateTime.Now : dto.RecordedAt,
                CameraId = dto.CameraId,
                Location = dto.Location ?? "El Sadat"
            };

            db.Trafficstatuses.Add(traffic);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Traffic saved successfully",
                data = new
                {
                    traffic.TrafficStatusId,
                    traffic.Status,
                    traffic.Location,
                    traffic.RecordedAt,
                    traffic.CameraId
                }
            });
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TrafficStatusDto dto)
        {
            var traffic = await db.Trafficstatuses.FindAsync(id);

            if (traffic == null)
                return Ok(new
                {
                    message = "Traffic not found",
                    data = (object)null
                });

            traffic.Status = dto.Status ?? "Normal";
            traffic.RecordedAt = dto.RecordedAt == default ? DateTime.Now : dto.RecordedAt;
            traffic.Location = dto.Location ?? traffic.Location ?? "Unknown";
            traffic.CameraId = dto.CameraId;

            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Traffic updated successfully",
                data = traffic
            });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var traffic = await db.Trafficstatuses.FindAsync(id);

            if (traffic == null)
                return Ok(new
                {
                    message = "Traffic not found"
                });

            db.Trafficstatuses.Remove(traffic);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Traffic deleted successfully"
            });
        }
    }
}