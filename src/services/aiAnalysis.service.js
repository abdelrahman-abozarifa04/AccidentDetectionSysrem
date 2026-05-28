/**
 * AI Analysis Service
 * Handles media file analysis requests to the FastAPI backend using standard Axios client.
 */

import { aiApi } from "./api";

/**
 * Analyze a single media file using AI backend
 * Backend endpoint: POST /predict/{model_name}
 * Request: multipart/form-data with "file" field
 * 
 * @param {File} file - Image file
 * @param {string} modelName - Model name: "detection" | "accident" | "congestion"
 * @param {Object} options - Optional parameters: { cameraId, location, notes }
 * @returns {Promise<Object>} Analysis result with detections, confidence, etc.
 */
export const analyzeMedia = async (file, modelName = 'detection', options = {}) => {
  // Validate file
  if (!file) {
    throw new Error('No file provided. Please select an image file.');
  }

  // Create FormData for prediction payload
  const formData = new FormData();
  formData.append('file', file);
  
  // Attach optional parameters if present
  if (options.cameraId) formData.append('camera_id', options.cameraId);
  if (options.location) formData.append('location', options.location);
  if (options.notes) formData.append('notes', options.notes);

  try {
    const response = await aiApi.post(`/predict/${modelName}`, formData);
    return response.data;
  } catch (error) {
    // Process and normalize standard Axios errors
    if (error.response) {
      const data = error.response.data;
      const status = error.response.status;
      
      const msg = data?.detail || data?.message || `AI prediction failed (HTTP ${status})`;
      
      if (status === 404) {
        throw new Error(`Model "${modelName}" not found. Available models: detection, accident, congestion`);
      }
      if (status === 422) {
        throw new Error('Unprocessable payload. Please upload a valid image (JPEG, PNG, WEBP).');
      }
      if (status === 500) {
        throw new Error(`AI prediction engine server error: ${msg}`);
      }
      throw new Error(msg);
    } else if (error.request) {
      // The request was made but no response was received
      throw new Error('AI analysis service is unreachable. Ensure the FastAPI/Hugging Face space is online.');
    } else {
      // Something happened in setting up the request that triggered an Error
      throw new Error(error.message);
    }
  }
};

/**
 * Analyze media files (UI supports multiple, but sends ONE to backend per contract)
 * 
 * @param {File[]} files - Array of files from UI (only files[0] sent)
 * @param {string} modelName - Model name: "detection" | "accident" | "congestion"
 * @returns {Promise<Object>} Analysis result
 */
export const analyzeMultipleMedia = async (files, modelName = 'detection') => {
  if (!files || files.length === 0) {
    throw new Error('No files provided. Please select a file to upload.');
  }

  // Backend only accepts single file - send only first file
  const file = files[0];
  
  return analyzeMedia(file, modelName);
};
