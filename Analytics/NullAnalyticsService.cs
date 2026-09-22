#nullable enable
using System.Collections.Generic;

namespace PaintTrek.Shared.Analytics
{
    /// <summary>
    /// Default no-op analytics backend. Used on Desktop and during early Android startup.
    /// Safe, zero overhead, never throws.
    /// </summary>
    public sealed class NullAnalyticsService : IAnalyticsService
    {
        private static NullAnalyticsService? _instance;
        public static NullAnalyticsService Instance => _instance ??= new NullAnalyticsService();

        private NullAnalyticsService() { }

        public bool IsEnabled => false;

        public void SetUserId(string userId)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[Analytics/Null] SetUserId: {userId}");
#endif
        }

        public void SetUserProperty(string name, string value)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[Analytics/Null] UserProperty: {name}={value}");
#endif
        }

        public void LogEvent(string eventName, IReadOnlyDictionary<string, object>? parameters = null)
        {
#if DEBUG
            if (parameters == null || parameters.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[Analytics/Null] {eventName}");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.Append("[Analytics/Null] ").Append(eventName).Append(" { ");
            bool first = true;
            foreach (var kv in parameters)
            {
                if (!first) sb.Append(", ");
                sb.Append(kv.Key).Append('=').Append(kv.Value);
                first = false;
            }
            sb.Append(" }");
            System.Diagnostics.Debug.WriteLine(sb.ToString());
#endif
        }

        public void SetCollectionEnabled(bool enabled)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[Analytics/Null] SetCollectionEnabled: {enabled}");
#endif
        }
    }
}
