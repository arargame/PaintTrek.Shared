#nullable enable
using System;
using System.Collections.Generic;

namespace PaintTrek.Shared.Analytics
{
    /// <summary>
    /// Game-facing analytics facade.
    /// Manages event schemas, pooled dictionaries (zero GC allocation),
    /// pending identity flush, and safe exception isolation.
    ///
    /// Calling conventions:
    ///   - All methods are safe (wrapped in try/catch) and will never crash the game.
    ///   - Designed for MonoGame 60 FPS update thread.
    /// </summary>
    public static class AnalyticsManager
    {
        private static IAnalyticsService _service = NullAnalyticsService.Instance;
        private static readonly Dictionary<string, object> _payload = new(24);
        private static string? _lastLoggedScreenName;

        private static string? _pendingUserId;
        private static bool _identitySent;
        private static string? _pendingLanguage;

        /// <summary>Optional hook to query active screen when service attaches.</summary>
        public static Func<string?>? ActiveScreenProvider { get; set; }

        /// <summary>
        /// Active analytics backend service. Defaults to NullAnalyticsService.Instance.
        /// Android sets this to FirebaseAnalyticsService once UMP consent is determined.
        /// </summary>
        public static IAnalyticsService Service
        {
            get => _service;
            set
            {
                _service = value ?? NullAnalyticsService.Instance;
                FlushPendingIdentity();
                FlushActiveScreen();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // IDENTITY & USER PROPERTIES
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the persistent user/player ID (e.g. GameSettings.PlayerId).
        /// If the backend is not yet attached, queues it until Service is set.
        /// </summary>
        public static void SyncIdentity(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return;

                _pendingUserId = userId;
                _identitySent = false;
                FlushPendingIdentity();
            }
            catch (Exception ex) { Warn(ex); }
        }

        private static void FlushPendingIdentity()
        {
            try
            {
                if (_identitySent || _pendingUserId == null) return;
                if (!_service.IsEnabled) return;

                _service.SetUserId(_pendingUserId);
                _identitySent = true;

                if (!string.IsNullOrEmpty(_pendingLanguage))
                {
                    _service.SetUserProperty(AnalyticsUserProps.SelectedLanguage, _pendingLanguage);
                }
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Updates a GA4 user property on the current player.
        /// </summary>
        public static void SetUserProperty(string name, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return;
                _service.SetUserProperty(name, value);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Sets active game language.
        /// </summary>
        public static void SetSelectedLanguage(string languageCode)
        {
            try
            {
                if (string.IsNullOrEmpty(languageCode)) return;
                _pendingLanguage = languageCode;
                _service.SetUserProperty(AnalyticsUserProps.SelectedLanguage, languageCode);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Refreshes global user properties (max level, progress bucket, control style, device tier).
        /// </summary>
        public static void RefreshUserProperties(int maxLevel, string? controlStyle = null, string? deviceTier = null)
        {
            try
            {
                if (!_service.IsEnabled) return;

                _service.SetUserProperty(AnalyticsUserProps.MaxUnlockedLevel, maxLevel.ToString());
                _service.SetUserProperty(AnalyticsUserProps.HasPassedLevel1, maxLevel >= 2 ? "true" : "false");
                _service.SetUserProperty(AnalyticsUserProps.ProgressBucket, ProgressBucket(maxLevel));

                if (!string.IsNullOrEmpty(controlStyle))
                    _service.SetUserProperty(AnalyticsUserProps.ControlStyle, controlStyle);

                if (!string.IsNullOrEmpty(deviceTier))
                    _service.SetUserProperty(AnalyticsUserProps.DeviceTier, deviceTier);
            }
            catch (Exception ex) { Warn(ex); }
        }

        public static string ProgressBucket(int maxLevel)
        {
            if (maxLevel <= 1) return "level_1";
            if (maxLevel == 2) return "level_2";
            if (maxLevel <= 5) return "3-5";
            if (maxLevel <= 10) return "6-10";
            if (maxLevel <= 20) return "11-20";
            if (maxLevel <= 50) return "21-50";
            return "51+";
        }

        /// <summary>
        /// Dedicated conversion event when a player passes Level 1 / reaches Level 2.
        /// Ideal for Google Ads campaigns (tCPA target) and remarketing audiences.
        /// </summary>
        public static void LogLevel2Reached()
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.LevelNumber] = 2;
                _service.LogEvent(AnalyticsEvents.Level2Reached, _payload);
                _service.LogEvent(AnalyticsEvents.FirstLevelCompleted, _payload);
                _service.SetUserProperty(AnalyticsUserProps.HasPassedLevel1, "true");
            }
            catch (Exception ex) { Warn(ex); }
        }

        // ─────────────────────────────────────────────────────────────────────
        // EVENTS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Logs screen transition. ScreenManager.AddScreen calls this automatically.
        /// Deduplicates consecutive identical screens.
        /// </summary>
        public static void LogScreenView(string screenName)
        {
            try
            {
                if (string.IsNullOrEmpty(screenName)) return;

                if (screenName == _lastLoggedScreenName && _service is not NullAnalyticsService)
                    return;

                _lastLoggedScreenName = screenName;

                _payload.Clear();
                _payload[AnalyticsParams.ScreenName] = screenName;
                _payload[AnalyticsParams.ScreenClass] = screenName;

                _service.LogEvent(AnalyticsEvents.ScreenView, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        private static void FlushActiveScreen()
        {
            try
            {
                if (_service is NullAnalyticsService) return;

                string? active = ActiveScreenProvider?.Invoke();
                if (!string.IsNullOrEmpty(active))
                {
                    _lastLoggedScreenName = null;
                    LogScreenView(active);
                }
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs level started event.
        /// </summary>
        public static void LogLevelStarted(int levelNumber, string gameMode, string controlStyle)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.LevelNumber] = levelNumber;
                _payload[AnalyticsParams.GameMode] = gameMode;
                _payload[AnalyticsParams.ControlStyle] = controlStyle;

                _service.LogEvent(AnalyticsEvents.LevelStarted, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs level completed event.
        /// </summary>
        public static void LogLevelCompleted(int levelNumber, string gameMode, int score,
                                             int durationSeconds, int damageTaken,
                                             int enemiesKilled, int collectablesCount, bool isBossLevel)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.LevelNumber] = levelNumber;
                _payload[AnalyticsParams.GameMode] = gameMode;
                _payload[AnalyticsParams.Score] = score;
                _payload[AnalyticsParams.DurationSeconds] = durationSeconds;
                _payload[AnalyticsParams.DamageTaken] = damageTaken;
                _payload[AnalyticsParams.EnemiesKilled] = enemiesKilled;
                _payload[AnalyticsParams.CollectablesCount] = collectablesCount;
                _payload[AnalyticsParams.IsBossLevel] = isBossLevel ? 1 : 0;

                _service.LogEvent(AnalyticsEvents.LevelCompleted, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs level failed (Game Over) event.
        /// </summary>
        public static void LogLevelFailed(int levelNumber, string gameMode, int score,
                                          int durationSeconds, int damageTaken,
                                          string killedBy, bool isBossLevel)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.LevelNumber] = levelNumber;
                _payload[AnalyticsParams.GameMode] = gameMode;
                _payload[AnalyticsParams.Score] = score;
                _payload[AnalyticsParams.DurationSeconds] = durationSeconds;
                _payload[AnalyticsParams.DamageTaken] = damageTaken;
                _payload[AnalyticsParams.KilledBy] = string.IsNullOrEmpty(killedBy) ? AnalyticsValues.Unknown : killedBy;
                _payload[AnalyticsParams.IsBossLevel] = isBossLevel ? 1 : 0;

                _service.LogEvent(AnalyticsEvents.LevelFailed, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs level abandoned event (player quit to menu or exited before completing/failing).
        /// </summary>
        public static void LogLevelAbandoned(int levelNumber, string gameMode, int score,
                                             int durationSeconds, int remainingHealth,
                                             int enemiesKilled, string? reason = null)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.LevelNumber] = levelNumber;
                _payload[AnalyticsParams.GameMode] = gameMode;
                _payload[AnalyticsParams.Score] = score;
                _payload[AnalyticsParams.DurationSeconds] = durationSeconds;
                _payload[AnalyticsParams.RemainingHealth] = remainingHealth;
                _payload[AnalyticsParams.EnemiesKilled] = enemiesKilled;
                if (!string.IsNullOrEmpty(reason))
                {
                    _payload[AnalyticsParams.AbandonReason] = reason;
                }

                _service.LogEvent(AnalyticsEvents.LevelAbandoned, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs boss encounter result.
        /// </summary>
        public static void LogBossFightResult(int bossNumber, string bossName, bool isVictory,
                                              int durationSeconds, int damageTaken, string gameMode)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.BossNumber] = bossNumber;
                _payload[AnalyticsParams.BossName] = bossName;
                _payload[AnalyticsParams.IsVictory] = isVictory ? 1 : 0;
                _payload[AnalyticsParams.DurationSeconds] = durationSeconds;
                _payload[AnalyticsParams.DamageTaken] = damageTaken;
                _payload[AnalyticsParams.GameMode] = gameMode;

                _service.LogEvent(AnalyticsEvents.BossFightResult, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs diamond collected event (Red, Blue, Green, Black).
        /// </summary>
        public static void LogDiamondCollected(string color, string ability, int levelNumber)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.DiamondColor] = color;
                _payload[AnalyticsParams.AbilityType] = ability;
                _payload[AnalyticsParams.LevelNumber] = levelNumber;

                _service.LogEvent(AnalyticsEvents.DiamondCollected, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs weapon or supply crate collected event.
        /// </summary>
        public static void LogWeaponCollected(string weaponType, int levelNumber)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.WeaponType] = weaponType;
                _payload[AnalyticsParams.LevelNumber] = levelNumber;

                _service.LogEvent(AnalyticsEvents.WeaponCollected, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        /// <summary>
        /// Logs second chance / revive response.
        /// </summary>
        public static void LogSecondChanceUsed(bool accepted, int levelNumber)
        {
            try
            {
                _payload.Clear();
                _payload[AnalyticsParams.ReviveAccepted] = accepted ? 1 : 0;
                _payload[AnalyticsParams.LevelNumber] = levelNumber;

                _service.LogEvent(AnalyticsEvents.SecondChanceUsed, _payload);
            }
            catch (Exception ex) { Warn(ex); }
        }

        private static void Warn(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Analytics] Ignored exception: {ex.Message}");
        }
    }
}
