/**
 * AI Analysis Service
 * Handles media file analysis requests to the FastAPI backend
 */

// Use environment variable for API base URL
const BASE_URL = import.meta.env.VITE_AI_API_BASE_URL || 'http://localhost:8000';

/**
 * Analyze a single media file using AI backend
 * Backend endpoint: POST /predict/{model_name}
 * Request: multipart/form-data with "file" field
 * 
 * @param {File} file - Image file
 * @param {string} modelName - Model name: "detection" | "accident" | "congestion"
 * @returns {Promise<Object>} Analysis result with detections, confidence, etc.
 */
export const analyzeMedia = async (file, modelName = 'detection') => {
  // Validate file
  if (!file) {
    throw new Error('No file provided. Please select an image file.');
  }

  // Create FormData with only "file" field (backend requirement)
  const formData = new FormData();
  formData.append('file', file);

  const url = `${BASE_URL}/predict/${modelName}`;

  const response = await fetch(url, {
    method: 'POST',
    body: formData,
    // Do NOT set Content-Type - browser sets multipart boundary automatically
  });

  // Parse response
  let data;
  const contentType = response.headers.get('content-type');
  if (contentType && contentType.includes('application/json')) {
    data = await response.json();
  } else {
    data = { detail: await response.text() };
  }

  // Handle errors
  if (!response.ok) {
    const msg = data.detail || data.message || `Request failed (${response.status})`;
    
    if (response.status === 404) {
      throw new Error(`Model "${modelName}" not found. Available: detection, accident, congestion`);
    }
    if (response.status === 422) {
      throw new Error(`Invalid file. Please upload a valid image (JPG, PNG, WEBP).`);
    }
    if (response.status === 500) {
      throw new Error('Server error. Please try again later.');
    }
    throw new Error(msg);
  }

  return data;
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

