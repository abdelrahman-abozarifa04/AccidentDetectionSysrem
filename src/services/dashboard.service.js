import api from "./api";

// Flag to use mock API (set to false when real backend is ready)
const USE_MOCK_API = false;

// Mock data functions (for development/testing)
const mockDashboardData = {
  stats: {
    activeAccidents: 1,
    congestionAreas: 3,
    violations: 0,
    resolvedCases: 3,
  },
  recentIncidents: [
    {
      id: "INC001",
      severity: "High",
      title: "Multi-vehicle collision blocking 2 lanes",
      location: "Main St & 5th Ave",
      time: "15m ago",
      type: "Accident",
    },
  ],
  systemStatus: {
    aiSystem: "Online",
    cameras: "247/250 Active",
    responseTeam: "12/15 Available",
  },
  todaySummary: {
    totalIncidents: 8,
    avgResponse: "8.5 minutes",
    activeOfficers: 45,
  },
};

// Simulate network delay
const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

/**
 * Fetch all dashboard data
 * @param {boolean} useMock - Whether to use mock API
 * @returns {Promise<Object>} Dashboard data object
 */
export async function getDashboardData(useMock = USE_MOCK_API) {
  if (useMock) {
    await delay(800); // Simulate network delay
    return mockDashboardData;
  }

  // Real API call - expects endpoints like:
  // GET /dashboard/stats
  // GET /dashboard/incidents
  // GET /dashboard/system-status
  // GET /dashboard/summary
  try {
    const [statsRes, incidentsRes, systemRes, summaryRes] = await Promise.all([
      api.get("/Dashboard/stats"),
      api.get("/Dashboard/incidents"),
      api.get("/Dashboard/system-status"),
      api.get("/Dashboard/summary"),
    ]);

    return {
      stats: statsRes.data,
      recentIncidents: incidentsRes.data,
      systemStatus: systemRes.data,
      todaySummary: summaryRes.data,
    };
  } catch (error) {
    console.error("Failed to fetch dashboard data:", error);
    throw error;
  }
}

/**
 * Fetch dashboard stats only
 */
export async function getDashboardStats(useMock = USE_MOCK_API) {
  if (useMock) {
    await delay(500);
    return mockDashboardData.stats;
  }

  const response = await api.get("/Dashboard/stats");
  return response.data;
}

/**
 * Fetch recent incidents
 */
export async function getRecentIncidents(useMock = USE_MOCK_API) {
  if (useMock) {
    await delay(600);
    return mockDashboardData.recentIncidents;
  }

  const response = await api.get("/Dashboard/incidents");
  return response.data;
}

/**
 * Fetch system status
 */
export async function getSystemStatus(useMock = USE_MOCK_API) {
  if (useMock) {
    await delay(400);
    return mockDashboardData.systemStatus;
  }

  const response = await api.get("/Dashboard/system-status");
  return response.data;
}

/**
 * Fetch today's summary
 */
export async function getTodaySummary(useMock = USE_MOCK_API) {
  if (useMock) {
    await delay(500);
    return mockDashboardData.todaySummary;
  }

  const response = await api.get("/Dashboard/summary");
  return response.data;
}

export default {
  getDashboardData,
  getDashboardStats,
  getRecentIncidents,
  getSystemStatus,
  getTodaySummary,
};
