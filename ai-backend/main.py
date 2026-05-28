"""
FastAPI Backend for Smart Road Monitor AI Analysis
Integrating ultralytics YOLO for Road Anomaly and Accident Detection.
"""

from fastapi import FastAPI, File, UploadFile, Form, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import logging
import io
import os
import time
from PIL import Image
import uvicorn
from ultralytics import YOLO

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(
    title="Smart Road Monitor AI API",
    description="API for road defect and accident detection using YOLO",
    version="1.0.0"
)

# ============================
# CORS CONFIGURATION
# ============================
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allow all origins for development and deployment
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ============================
# YOLO MODEL INITIALIZATION
# ============================
model = None

def load_yolo_model():
    """Loads custom best.pt weights if present, falling back to yolov8n.pt"""
    global model
    try:
        model_path = "best.pt"
        if os.path.exists(model_path):
            logger.info(f"Loading custom YOLO weights from '{model_path}'...")
            model = YOLO(model_path)
        else:
            logger.info("Custom model 'best.pt' not found in workspace. Falling back to pre-trained 'yolov8n.pt'...")
            model = YOLO("yolov8n.pt")
        logger.info("YOLO model initialized successfully!")
    except Exception as e:
        logger.error(f"Failed to load YOLO model: {str(e)}", exc_info=True)
        # Attempt to load on demand if startup fails

@app.on_event("startup")
async def startup_event():
    load_yolo_model()

# ============================
# TAXONOMY MAPPING & SEVERITY
# ============================
def map_class_to_type(class_name: str) -> str:
    """Maps custom YOLO or COCO classes to a normalized system taxonomy."""
    class_lower = class_name.lower().strip()
    
    # Severe Accident Indicators
    accident_keywords = ["accident", "crash", "collision", "fire", "smoke", "overturned", "wreckage"]
    if any(kw in class_lower for kw in accident_keywords):
        return "accident"
    
    # Road Anomalies / Pavement deterioration
    anomaly_keywords = ["pothole", "crack", "defect", "rutting", "bump", "depression", "debris", "hole", "road_defect"]
    if any(kw in class_lower for kw in anomaly_keywords):
        return "anomaly"
    
    # Standard Telemetry/Objects
    vehicle_keywords = ["car", "truck", "bus", "motorcycle", "vehicle", "van", "bicycle"]
    if any(kw in class_lower for kw in vehicle_keywords):
        return "vehicle"
        
    pedestrian_keywords = ["person", "pedestrian", "walker"]
    if any(kw in class_lower for kw in pedestrian_keywords):
        return "pedestrian"
        
    return "other"

def run_inference(image: Image.Image):
    """Executes YOLO model on PIL Image and extracts normalized telemetry features."""
    global model
    if model is None:
        load_yolo_model()
    if model is None:
        raise RuntimeError("YOLO model could not be initialized.")

    start_time = time.time()
    results = model(image)
    processing_time = time.time() - start_time

    detections = []
    has_anomaly = False
    has_accident = False
    max_confidence = 0.0

    for result in results:
        # Extract detections
        for box in result.boxes:
            cls_id = int(box.cls[0])
            class_name = model.names[cls_id]
            confidence = float(box.conf[0])
            bbox = [float(coord) for coord in box.xyxy[0]]  # [x1, y1, x2, y2]
            
            # Categorize the detection
            det_type = map_class_to_type(class_name)
            
            if det_type == "accident":
                has_accident = True
            elif det_type == "anomaly":
                has_anomaly = True

            if confidence > max_confidence:
                max_confidence = confidence

            detections.append({
                "class_name": class_name,
                "type": det_type,
                "confidence": round(confidence, 4),
                "bbox": [round(c, 2) for c in bbox]
            })

    # Calculate overall severity
    if has_accident:
        severity = "critical"
    elif has_anomaly:
        severity = "high" if max_confidence > 0.8 else "medium"
    elif len(detections) > 0:
        severity = "low"
    else:
        severity = "none"

    return detections, has_anomaly, has_accident, severity, max_confidence, processing_time

# ============================
# REQUEST/RESPONSE MODELS
# ============================
class Detection(BaseModel):
    class_name: str
    type: str  # "anomaly", "accident", "vehicle", "pedestrian", "other"
    confidence: float
    bbox: List[float]

class AnalysisResult(BaseModel):
    status: str
    message: str
    model_name: str
    camera_id: str
    analysis_type: Optional[str]
    location: Optional[str]
    filename: str
    detections: List[Detection]
    has_anomaly: bool
    has_accident: bool
    severity: str  # "none", "low", "medium", "high", "critical"
    confidence: float
    processing_time: float
    image_size: dict  # {"width": int, "height": int}

# ============================
# ENDPOINTS
# ============================
@app.get("/")
async def root():
    model_loaded = "None" if model is None else getattr(model, "ckpt_path", "Loaded")
    return {
        "status": "online",
        "message": "Smart Road Monitor AI API is running",
        "version": "1.0.0",
        "model_loaded": model_loaded
    }

@app.get("/health")
async def health_check():
    if model is None:
        return {"status": "unhealthy", "error": "Model not loaded"}
    return {"status": "healthy"}

@app.post("/predict/{model_name}", response_model=AnalysisResult)
async def predict(
    model_name: str,
    file: UploadFile = File(..., description="Image file to analyze"),
    camera_id: str = Form("1", description="Camera ID"),
    analysis_type: Optional[str] = Form(None, description="Type of analysis"),
    location: Optional[str] = Form(None, description="Location/Road ID"),
    notes: Optional[str] = Form(None, description="Additional notes")
):
    """
    Analyze a road image using YOLO to identify Anomalies, Accidents, and Objects.
    """
    try:
        logger.info(f"Prediction requested: model={model_name}, camera_id={camera_id}, location={location}")
        
        # Validate File Content-Type
        allowed_types = {"image/jpeg", "image/jpg", "image/png", "image/webp"}
        if file.content_type not in allowed_types:
            raise HTTPException(
                status_code=400,
                detail=f"Unsupported file type: {file.content_type}. Supported: JPEG, PNG, WEBP"
            )

        # Read File Contents
        contents = await file.read()
        if len(contents) == 0:
            raise HTTPException(status_code=400, detail="Uploaded file is empty.")

        # Load into PIL Image
        try:
            image = Image.open(io.BytesIO(contents))
            width, height = image.size
            # Convert to RGB (to handle grayscale, RGBA, etc. safely in YOLO)
            if image.mode != "RGB":
                image = image.convert("RGB")
        except Exception as e:
            logger.error(f"Image load failure: {str(e)}")
            raise HTTPException(status_code=400, detail="Invalid image payload.")

        # Run Inference
        detections, has_anomaly, has_accident, severity, max_conf, proc_time = run_inference(image)

        # Build Response message
        if has_accident and has_anomaly:
            msg = "ALERT: Severe accident and road anomalies detected."
        elif has_accident:
            msg = "ALERT: Severe accident incident detected."
        elif has_anomaly:
            msg = "Road anomalies detected."
        else:
            msg = "Road condition is normal. No defects or incidents detected."

        return AnalysisResult(
            status="success",
            message=msg,
            model_name=model_name,
            camera_id=camera_id,
            analysis_type=analysis_type,
            location=location,
            filename=file.filename,
            detections=detections,
            has_anomaly=has_anomaly,
            has_accident=has_accident,
            severity=severity,
            confidence=round(max_conf, 4),
            processing_time=round(proc_time, 4),
            image_size={"width": width, "height": height}
        )

    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Prediction Error: {str(e)}", exc_info=True)
        raise HTTPException(status_code=500, detail=f"Internal prediction engine error: {str(e)}")

@app.post("/predict/batch/{model_name}")
async def predict_batch(
    model_name: str,
    files: List[UploadFile] = File(..., description="Image files to analyze"),
    camera_id: str = Form("1"),
    analysis_type: Optional[str] = Form(None),
    location: Optional[str] = Form(None),
    notes: Optional[str] = Form(None)
):
    """
    Analyze multiple road images in batch.
    """
    results = []
    
    for file in files:
        try:
            contents = await file.read()
            image = Image.open(io.BytesIO(contents))
            if image.mode != "RGB":
                image = image.convert("RGB")
                
            detections, has_anomaly, has_accident, severity, max_conf, proc_time = run_inference(image)
            
            results.append({
                "filename": file.filename,
                "status": "success",
                "detections": detections,
                "has_anomaly": has_anomaly,
                "has_accident": has_accident,
                "severity": severity,
                "confidence": round(max_conf, 4)
            })
        except Exception as e:
            logger.error(f"Batch prediction failure for {file.filename}: {str(e)}")
            results.append({
                "filename": file.filename,
                "status": "error",
                "message": str(e)
            })
            
    return {
        "status": "success",
        "model_name": model_name,
        "camera_id": camera_id,
        "results": results
    }

# ============================
# ERROR HANDLING
# ============================
@app.exception_handler(Exception)
async def global_exception_handler(request, exc):
    logger.error(f"Global exception: {str(exc)}", exc_info=True)
    return {
        "status": "error",
        "message": "An unhandled execution error occurred in the AI backend.",
        "detail": str(exc)
    }

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
