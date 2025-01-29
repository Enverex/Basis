using System;

public class BasisProgressReport
{
    // Delegate definitions for various stages of progress
    public delegate void ProgressStart(string UniqueID);
    public delegate void ProgressReportState(string UniqueID, float progress, string eventDescription);
    public delegate void ProgressComplete(string UniqueID);

    // Event handlers that can be set by the user
    public event ProgressStart OnProgressStart;
    public event ProgressReportState OnProgressReport;
    public event ProgressComplete OnProgressComplete;

    // Tracks whether progress has started, allowing for control over multiple calls
    private static bool _isProgressStarted = false;

    /// <summary>
    /// Initializes and starts the progress reporting, invoking the start callback.
    /// </summary>
    public void StartProgress(string UniqueID)
    {
        if (_isProgressStarted) return; // Prevent starting if already in progress

        _isProgressStarted = true;
        OnProgressStart?.Invoke(UniqueID);
    }

    /// <summary>
    /// Reports the current progress along with an event description.
    /// Ensures that progress is between 0 and 100.
    /// </summary>
    /// <param name="progress">A float value between 0 and 100 representing the progress.</param>
    /// <param name="eventDescription">A string describing the current event or stage.</param>
    public void ReportProgress(string UniqueID, float progress, string eventDescription)
    {
        if (!_isProgressStarted)
        {
            StartProgress(UniqueID);
        }

        progress = Math.Clamp(progress, 0f, 100f); // Ensuring progress is within bounds
        OnProgressReport?.Invoke(UniqueID, progress, eventDescription);

        // Automatically complete progress if it reaches 100
        if (progress >= 100f)
        {
            CompleteProgress(UniqueID);
        }
    }

    /// <summary>
    /// Completes the progress reporting, invokes the complete callback, and resets state.
    /// </summary>
    public void CompleteProgress(string UniqueID)
    {
        if (!_isProgressStarted) return; // Prevent completing if not started

        OnProgressComplete?.Invoke(UniqueID);
        _isProgressStarted = false; // Reset progress state for reuse
    }
}
