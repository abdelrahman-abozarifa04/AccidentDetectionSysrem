using AccidentDetectionSysrem.modelsef;
using AccidentDetectionSysrem.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccidentDetectionSysrem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetectionsController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect _context;

        public DetectionsController(AccidentDetectionSystemContect context)
        {
            _context = context;
        }

        // GET: api/detections (Normal ONLY)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var detections = await _context.Detections
                .Where(d => d.Result.ToLower() == "normal") // 🔥 filter
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DetectionDto
                {
                    Id = d.Id,
                    Result = d.Result,
                    Confidence = d.Confidence,
                    CreatedAt = d.CreatedAt,
                    CameraId = d.CameraId,
                    Location = d.Location
                })
                .ToListAsync();

            return Ok(detections);
        }

        // GET: api/detections/clear
        [HttpGet("clear")]
        public async Task<IActionResult> GetClearStatus()
        {
            var detections = await _context.Detections
                .Where(d => d.Result.ToLower() == "normal")
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DetectionDto
                {
                    Id = d.Id,
                    Result = d.Result,
                    Confidence = d.Confidence,
                    CreatedAt = d.CreatedAt,
                    CameraId = d.CameraId,
                    Location = d.Location
                })
                .ToListAsync();

            if (!detections.Any())
                return Ok(new { message = "No clear locations — accidents or congestion detected", data = detections });

            return Ok(new { message = "All clear", data = detections });
        }

       
        [HttpGet("result/{result}")]
        public async Task<IActionResult> GetByResult(string result)
        {
          
            if (!result.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only 'Normal' is allowed");

            var detections = await _context.Detections
                .Where(d => d.Result.ToLower() == "normal")
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DetectionDto
                {
                    Id = d.Id,
                    Result = d.Result,
                    Confidence = d.Confidence,
                    CreatedAt = d.CreatedAt,
                    CameraId = d.CameraId,
                    Location = d.Location
                })
                .ToListAsync();

            return Ok(detections);
        }

        // GET: api/detections/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detection = await _context.Detections
                .Where(d => d.Id == id && d.Result.ToLower() == "normal") // 🔥 filter
                .Select(d => new DetectionDto
                {
                    Id = d.Id,
                    Result = d.Result,
                    Confidence = d.Confidence,
                    CreatedAt = d.CreatedAt,
                    CameraId = d.CameraId,
                    Location = d.Location
                })
                .FirstOrDefaultAsync();

            if (detection == null)
                return NotFound("Detection not found");

            return Ok(detection);
        }

        // POST: api/detections (Normal ONLY)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DetectionDto dto)
        {
            if (string.IsNullOrEmpty(dto.Result))
                return BadRequest("Result is required");

            if (dto.CameraId <= 0)
                return BadRequest("CameraId is required");

            
            if (!dto.Result.Equals("Normal", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only 'Normal' detections are allowed");

            var detection = new Detection
            {
                Result = "Normal",
                Confidence = dto.Confidence,
                CameraId = dto.CameraId,
                Location = dto.Location,
                CreatedAt = DateTime.Now
            };

            _context.Detections.Add(detection);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = detection.Id }, new
            {
                message = "Normal detection stored successfully",
                id = detection.Id
            });
        }

        // POST: api/detections/store-if-clear
        [HttpPost("store-if-clear")]
        public async Task<IActionResult> StoreIfClear([FromBody] ClearStatusDto dto)
        {
            if (dto.Accident == null || dto.Traffic == null)
                return BadRequest("Both accident and traffic results are required");

            bool isNoAccident = dto.Accident.PredictionClass
                ?.Replace(" ", "").ToLower() == "noaccident";

            bool isNoCongestion = dto.Traffic.PredictionClass
                ?.Replace(" ", "").ToLower() == "nocongestion";

           
            if (!isNoAccident || !isNoCongestion)
                return Ok(new
                {
                    stored = false,
                    message = "Only clear  (no accident + no congestion) is allowed"
                });

            var detection = new Detection
            {
                Result = "Normal",
                Confidence = (dto.Accident.Confidence + dto.Traffic.Confidence) / 2,
                CameraId = dto.CameraId,
                Location = dto.Accident.Location ?? dto.Traffic.Location,
                CreatedAt = DateTime.Now
            };

            _context.Detections.Add(detection);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = detection.Id }, new
            {
                stored = true,
                message = "Normal detection stored successfully",
                id = detection.Id
            });
        }

        // DELETE: api/detections/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detection = await _context.Detections.FindAsync(id);

            if (detection == null)
                return NotFound("Detection not found");

            _context.Detections.Remove(detection);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}