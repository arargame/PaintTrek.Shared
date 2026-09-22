#nullable enable
using System.Collections.Generic;

namespace PaintTrek.Shared.Analytics
{
    /// <summary>
    /// Analytics backend interface. Responsible only for passing telemetry events to platform SDK.
    /// Does not contain game-specific logic or rules.
    ///
    /// Contract:
    ///   1) Never throws exceptions. Telemetry must never crash the game.
    ///   2) Methods are called from game thread and return quickly.
    ///   3) The dictionary passed to LogEvent is reused by caller during the call;
    ///      implementations must copy values if processing asynchronously.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>Whether a real backend is attached and enabled.</summary>
        bool IsEnabled { get; }

        /// <summary>Sets the persistent user identifier (PlayerId).</summary>
        void SetUserId(string userId);

        /// <summary>Sets a user-level segmentation property (GA4 user property).</summary>
        void SetUserProperty(string name, string value);

        /// <summary>Logs a telemetry event with optional parameters.</summary>
        void LogEvent(string eventName, IReadOnlyDictionary<string, object>? parameters = null);

        /// <summary>Enables or disables analytics collection (for GDPR / UMP consent).</summary>
        void SetCollectionEnabled(bool enabled);
    }
}
