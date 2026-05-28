using AccidentDetectionSysrem.DTO;
using AccidentDetectionSysrem.modelsef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccidentDetectionSysrem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccidentController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect db;

        public AccidentController(AccidentDetectionSystemContect db)
        {
            this.db = db;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accidents = await db.Accidents
                .Select(a => new AccidentDto
                {
                    AccidentId = a.AccidentId,
                    AccidentTime = a.AccidentTime,

                    Location = a.Location,

                    PredictionClass = a.Description != null && a.Description.Contains("Confidence")
                        ? "Accident"
                        : "No Accident",

                    Confidence = a.Description != null && a.Description.Contains("Confidence")
                        ? float.Parse(
                            a.Description
                            .Replace("Detected by AI - Confidence: ", "")
                            .Replace("%", "")
                          ) / 100
                        : null
                })
                .ToListAsync();

            return Ok(accidents);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var accident = await db.Accidents
                .Where(a => a.AccidentId == id)
                .Select(a => new AccidentDto
                {
                    AccidentId = a.AccidentId,
                    AccidentTime = a.AccidentTime,

                    Location = a.Location, 

                    PredictionClass = a.Description != null && a.Description.Contains("Confidence")
                        ? "Accident"
                        : "No Accident",

                    Confidence = a.Description != null && a.Description.Contains("Confidence")
                        ? float.Parse(
                            a.Description
                            .Replace("Detected by AI - Confidence: ", "")
                            .Replace("%", "")
                          ) / 100
                        : null
                })
                .FirstOrDefaultAsync();

            if (accident == null)
                return NotFound($"Accident with id {id} not found");

            return Ok(accident);
        }

        
        [HttpPost]
        public async Task<IActionResult> Add(AccidentDto dto)
        {
            var accident = new Accident
            {
                AccidentTime = dto.AccidentTime,
                Location = dto.Location 
            };

            db.Accidents.Add(accident);
            await db.SaveChangesAsync();

            dto.AccidentId = accident.AccidentId;

            return Ok(dto);
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AccidentDto dto)
        {
            if (id != dto.AccidentId)
                return BadRequest();

            var accident = await db.Accidents.FindAsync(id);

            if (accident == null)
                return NotFound();

            accident.AccidentTime = dto.AccidentTime;
            accident.Location = dto.Location; 

            await db.SaveChangesAsync();

            return Ok(dto);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var accident = await db.Accidents.FindAsync(id);

            if (accident == null)
                return NotFound();

            db.Accidents.Remove(accident);
            await db.SaveChangesAsync();

            return Ok("Accident deleted");
        }
    }
}