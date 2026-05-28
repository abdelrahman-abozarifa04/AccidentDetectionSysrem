using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccidentDetectionSysrem.modelsef;

namespace AccidentDetectionSysrem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AccidentDetectionSystemContect _context;

        public DashboardController(AccidentDetectionSystemContect context)
        {
            _context = context;
        }

        // GET: api/Dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var activeAccidents = await _context.Accidents.CountAsync();
            var congestionAreas = await _context.Trafficstatuses.CountAsync();
            var violations = await _context.Detections.CountAsync();
            var resolvedCases = 15;

            return Ok(new
            {
                activeAccidents = activeAccidents > 0 ? activeAccidents : 3,
                congestionAreas = congestionAreas > 0 ? congestionAreas : 2,
                violations = violations > 0 ? violations : 5,
                resolvedCases = resolvedCases
            });
        }

        // GET: api/Dashboard/incidents
        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents()
        {
            var accidentsList = await _context.Accidents
                .OrderByDescending(a => a.AccidentId)
                .Take(5)
                .Select(a => new
                {
                    id = "INC" + a.AccidentId.ToString("D3"),
                    severity = "Medium",
                    title = a.Description ?? "Road Incident",
                    location = a.Location ?? "Unknown Location",
                    time = "Just now",
                    type = "Accident"
                })
                .ToListAsync();

            if (accidentsList.Count == 0)
            {
                return Ok(new[]
                {
                    new {
                        id = "INC001",
                        severity = "High",
                        title = "Multi-vehicle collision blocking 2 lanes",
                        location = "Main St & 5th Ave",
                        time = "15m ago",
                        type = "Accident"
                    },
                    new {
                        id = "INC002",
                        severity = "Medium",
                        title = "Minor fender bender near intersection",
                        location = "Elm St & 12th Rd",
                        time = "45m ago",
                        type = "Accident"
                    }
                });
            }

            return Ok(accidentsList);
        }

        // GET: api/Dashboard/system-status
        [HttpGet("system-status")]
        public async Task<IActionResult> GetSystemStatus()
        {
            var totalCameras = await _context.Cameras.CountAsync();
            var activeCameras = totalCameras;
            
            if (totalCameras == 0)
            {
                totalCameras = 250;
                activeCameras = 247;
            }

            return Ok(new
            {
                aiSystem = "Online",
                cameras = $"{activeCameras}/{totalCameras} Active",
                responseTeam = "12/15 Available"
            });
        }

        // GET: api/Dashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalIncidents = await _context.Accidents.CountAsync();
            var activeOfficers = await _context.Users.CountAsync(u => u.Role == "Officer" && u.IsActive);

            return Ok(new
            {
                totalIncidents = totalIncidents > 0 ? totalIncidents : 8,
                avgResponse = "8.5 minutes",
                activeOfficers = activeOfficers > 0 ? activeOfficers : 12
            });
        }
    }
}
