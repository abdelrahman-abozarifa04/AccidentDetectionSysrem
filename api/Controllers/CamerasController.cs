using AccidentDetectionSysrem.modelsef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace AccidentDetectionSysrem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CamerasController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly AccidentDetectionSystemContect _context;

        public CamerasController(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            AccidentDetectionSystemContect context)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _context = context;
        }

        [HttpPost("process-image")]
        public async Task<IActionResult> ProcessImage(IFormFile file, [FromForm] string camera_id)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Image is required");

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60);

                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                var baseUrl = _config["HuggingFaceApi:BaseUrl"];

               
                var accidentContent = new MultipartFormDataContent();
                var accidentImage = new ByteArrayContent(fileBytes);
                accidentImage.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                accidentContent.Add(accidentImage, "file", file.FileName);
                accidentContent.Add(new StringContent(camera_id), "camera_id");

                var accidentTask = httpClient.PostAsync(baseUrl + "/predict/accident", accidentContent);

                
                var trafficContent = new MultipartFormDataContent();
                var trafficImage = new ByteArrayContent(fileBytes);
                trafficImage.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                trafficContent.Add(trafficImage, "file", file.FileName);
                trafficContent.Add(new StringContent(camera_id), "camera_id");

                var trafficTask = httpClient.PostAsync(baseUrl + "/predict/congestion", trafficContent);

                await Task.WhenAll(accidentTask, trafficTask);

                var accidentJson = await accidentTask.Result.Content.ReadAsStringAsync();
                var trafficJson = await trafficTask.Result.Content.ReadAsStringAsync();

                var accidentResult = System.Text.Json.JsonSerializer.Deserialize<HuggingFaceResult>(accidentJson);
                var trafficResult = System.Text.Json.JsonSerializer.Deserialize<HuggingFaceResult>(trafficJson);

                // ---------------- Camera ----------------
                Camera? camera = null;
                int? camId = null;

                if (int.TryParse(camera_id, out int parsedId))
                {
                    camera = await _context.Cameras.FindAsync(parsedId);

                    if (camera == null)
                    {
                        camera = new Camera
                        {
                            CameraName = "Camera " + camera_id,
                            Location = accidentResult?.location
                                       ?? trafficResult?.location
                                       ?? "Unknown",
                            CreatedAt = DateTime.Now
                        };

                        _context.Cameras.Add(camera);
                        await _context.SaveChangesAsync();
                    }

                    camId = camera.CameraId;
                }

                string finalLocation = camera?.Location ?? "Unknown";

                // ---------------- Accident Save ----------------
                if (accidentResult?.prediction?.@class == "Accident")
                {
                    _context.Accidents.Add(new Accident
                    {
                        Description = $"Detected by AI - Confidence: {accidentResult.prediction.confidence:P0}",
                        CameraId = camId,
                        Location = accidentResult?.location ?? finalLocation,
                        AccidentTime = DateTime.Now
                    });

                    _context.Detections.Add(new Detection
                    {
                        CameraId = camId,
                        
                        Result = "Accident",
                        Confidence = accidentResult.prediction.confidence,
                        Location = finalLocation,
                        CreatedAt = DateTime.Now
                    });
                }

                // ---------------- Traffic Save ----------------
                if (trafficResult?.prediction?.@class == "CONGESTION")
                {
                    _context.Trafficstatuses.Add(new Trafficstatus
                    {
                        Status = "Heavy Traffic",
                        Description = $"Detected by AI - Confidence: {trafficResult.prediction.confidence:P0}",
                        CameraId = camId,
                        Location = trafficResult?.location ?? finalLocation,
                        RecordedAt = DateTime.Now
                    });

                    _context.Detections.Add(new Detection
                    {
                        CameraId = camId,
                        
                        Result = "CONGESTION",
                        Confidence = trafficResult.prediction.confidence,
                        Location = finalLocation,
                        CreatedAt = DateTime.Now
                    });
                }

                // ---------------- NORMAL Save ----------------
                if (accidentResult?.prediction?.@class == "No Accident" &&
                    trafficResult?.prediction?.@class == "NO CONGESTION")
                {
                    _context.Detections.Add(new Detection
                    {
                        CameraId = camId,
                       
                        Result = "Normal",
                        Confidence = Math.Max(
                            accidentResult?.prediction?.confidence ?? 0,
                            trafficResult?.prediction?.confidence ?? 0
                        ),
                        Location = finalLocation,
                        CreatedAt = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accident = new
                    {
                        location = accidentResult?.location ?? finalLocation,
                        predictionClass = accidentResult?.prediction?.@class ?? "No Accident",
                        confidence = accidentResult?.prediction?.confidence ?? 0f
                    },
                    traffic = new
                    {
                        location = trafficResult?.location ?? finalLocation,
                        predictionClass = trafficResult?.prediction?.@class ?? "NORMAL",
                        confidence = trafficResult?.prediction?.confidence ?? 0f
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error occurred",
                    error = ex.Message
                });
            }
        }

        // ---------------- GET CAMERAS ----------------
        [HttpGet("cameras")]
        public async Task<IActionResult> GetCameras()
        {
            var cameras = await _context.Cameras.ToListAsync();
            return Ok(cameras);
        }

        // ---------------- GET ACCIDENTS ----------------
        [HttpGet("accidents")]
        public async Task<IActionResult> GetAccidents()
        {
            var accidents = await _context.Accidents
                .Include(a => a.Camera)
                .ToListAsync();

            return Ok(accidents);
        }

        // ---------------- GET TRAFFIC ----------------
        [HttpGet("traffic")]
        public async Task<IActionResult> GetTraffic()
        {
            var traffic = await _context.Trafficstatuses
                .ToListAsync();

            return Ok(traffic);
        }

    
        private class HuggingFaceResult
        {
            public bool success { get; set; }
            public string? camera_id { get; set; }
            public string? location { get; set; }
            public string? model { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("prediction")]
            public PredictionResult? prediction { get; set; }
        }

        private class PredictionResult
        {
            [System.Text.Json.Serialization.JsonPropertyName("class")]
            public string? @class { get; set; }
            public float confidence { get; set; }
        }
    }
}