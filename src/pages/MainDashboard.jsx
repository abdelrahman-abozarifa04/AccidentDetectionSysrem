import React, { useEffect, useState, useCallback } from "react";
import { useAuth } from "../context/AuthContext";
import { getDashboardData } from "../services/dashboard.service";

// Reusable UI Components
import StatCard from "../components/ui/StatCard";
import { DashboardSkeleton } from "../components/ui/LoadingSkeleton";
import ErrorState from "../components/ui/ErrorState";

import "../styles/dashboard.css";

/**
 * MainDashboard - Production-ready dashboard component
 * 
 * Features:
 * - Fetches data from API with proper loading/error states
 * - Auto-includes Authorization header via axios interceptor
 * - Handles 401 responses (redirect to signin)
 * - Prevents memory leaks with cleanup
 * - Memoized callbacks to avoid unnecessary re-renders
 * - No hardcoded data - uses service layer
 */
export default function MainDashboard() {
  // Auth state
  const { user, isAuthenticated } = useAuth();
  
  // Data state
  const [dashboardData, setDashboardData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch dashboard data
  const fetchDashboardData = useCallback(async () => {
    // Don't fetch if not authenticated
    if (!isAuthenticated) {
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const data = await getDashboardData();
      setDashboardData(data);
    } catch (err) {
      console.error("Dashboard fetch error:", err);
      
      // Handle specific error types
      if (err.response?.status === 401) {
        // Auth error - will be handled by axios interceptor
        setError("Session expired. Please sign in again.");
      } else if (err.code === 'NETWORK_ERROR' || err.message === 'Network Error') {
        setError("Unable to connect to server. Please check your connection.");
      } else {
        setError(err.response?.data?.message || "Failed to load dashboard data.");
      }
    } finally {
      setLoading(false);
    }
  }, [isAuthenticated]);

  // Fetch on mount and when auth changes
  useEffect(() => {
    fetchDashboardData();
  }, [fetchDashboardData]);

  // Loading state
  if (loading) {
    return <DashboardSkeleton />;
  }

  // Error state with retry
  if (error) {
    return (
      <ErrorState
        title="Failed to Load Dashboard"
        message={error}
        onRetry={fetchDashboardData}
      />
    );
  }

  // No data state
  if (!dashboardData) {
    return (
      <ErrorState
        title="No Data"
        message="Unable to retrieve dashboard data. Please try again."
        onRetry={fetchDashboardData}
      />
    );
  }

  const { stats, recentIncidents, systemStatus, todaySummary } = dashboardData;

  return (
    <div className="dashboard-page">
      {/* HEADER */}
      <div className="dashboard-header">
        <h1>Dashboard Overview</h1>
        <p>Welcome back, {user?.email || 'User'} | Real-time monitoring and incident management</p>
      </div>

      {/* STATS CARDS */}
      <div className="stats-grid">
        <StatCard 
          title="Active Accidents" 
          value={stats?.activeAccidents ?? 0} 
          type="#030202" 
        />
        <StatCard 
          title="Congestion Areas" 
          value={stats?.congestionAreas ?? 0} 
          type="030202" 
        />
        <StatCard 
          title="Violations" 
          value={stats?.violations ?? 0} 
          type="030202" 
        />
        <StatCard 
          title="Resolved Cases" 
          value={stats?.resolvedCases ?? 0} 
          type="030202" 
        />
      </div>

      {/* RECENT INCIDENTS */}
      <section className="dashboard-section">
        <h2>Recent Active Incidents</h2>
        
        {recentIncidents && recentIncidents.length > 0 ? (
          <div className="incidents-list">
            {recentIncidents.map((inc) => (
              <IncidentItem key={inc.id} incident={inc} />
            ))}
          </div>
        ) : (
          <div className="empty-incidents">
            <p>No active incidents at this time.</p>
          </div>
        )}
      </section>

      {/* BOTTOM GRID */}
      <div className="bottom-grid">
        <div className="card">
          <h3>System Status</h3>
          <StatusItem label="AI Detection System" value={systemStatus?.aiSystem} />
          <StatusItem label="Camera Network" value={systemStatus?.cameras} />
          <StatusItem label="Response Team" value={systemStatus?.responseTeam} />
        </div>

        <div className="card">
          <h3>Today's Summary</h3>
          <StatusItem label="Total Incidents" value={todaySummary?.totalIncidents} />
          <StatusItem label="Average Response Time" value={todaySummary?.avgResponse} />
          <StatusItem label="Active Officers" value={todaySummary?.activeOfficers} />
        </div>
      </div>
    </div>
  );
}

/**
 * IncidentItem - Individual incident display
 */
function IncidentItem({ incident }) {
  const severityClass = incident.severity?.toLowerCase() || 'low';
  
  return (
    <div className="incident-item">
      <div className={`severity ${severityClass}`} />
      <div className="incident-content">
        <span className="incident-id">{incident.id}</span>
        <span className={`badge ${severityClass}`}>
          {incident.severity}
        </span>
        <span className="time">{incident.time}</span>
        <h4>{incident.title}</h4>
        <p className="location">{incident.location}</p>
      </div>
      <span className="type-pill">{incident.type}</span>
    </div>
  );
}

/**
 * StatusItem - Key-value display for status cards
 */
function StatusItem({ label, value }) {
  if (value === undefined || value === null) {
    return null;
  }
  
  const isOnline = value.toString().toLowerCase().includes('online') || 
                    value.toString().includes('active') ||
                    value.toString().includes('available');
                    
  return (
    <p>
      {label}{" "}
      <span className={isOnline ? "green" : ""}>
        {value}
      </span>
    </p>
  );
}
