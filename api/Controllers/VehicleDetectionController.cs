using AccidentDetectionSysrem.modelsef;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AccidentDetectionSysrem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleDetectionController : ControllerBase
    {
        AccidentDetectionSystemContect db;

        public VehicleDetectionController(AccidentDetectionSystemContect db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var detections = await db.VehicleDetections.ToListAsync();
            return Ok(detections);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detection = await db.VehicleDetections
                .Include(v => v.Camera)
                 .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (detection == null)
                return NotFound();

            return Ok(detection);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VehicleDetection detection)
        {
            db.VehicleDetections.Add(detection);
            await db.SaveChangesAsync();
            return Ok(detection);
        }
    





















    }
}
