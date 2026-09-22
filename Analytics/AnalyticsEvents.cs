#nullable enable

namespace PaintTrek.Shared.Analytics
{
    /// <summary>
    /// Telemetry event names conforming to GA4 standards (<= 40 chars, alphanumeric + underscore).
    /// </summary>
    public static class AnalyticsEvents
    {
        /// <summary>Standard GA4 screen view transition.</summary>
        public const string ScreenView = "screen_view";

        /// <summary>Level started.</summary>
        public const string LevelStarted = "level_started";

        /// <summary>Level completed successfully.</summary>
        public const string LevelCompleted = "level_completed";

        /// <summary>Level failed (Game Over).</summary>
        public const string LevelFailed = "level_failed";

        /// <summary>Boss fight encounter outcome (win or loss).</summary>
        public const string BossFightResult = "boss_fight_result";

        /// <summary>Diamond collected (Red, Blue, Green, Black) granting special ability.</summary>
        public const string DiamondCollected = "diamond_collected";

        /// <summary>Weapon or item crate picked up by the player.</summary>
        public const string WeaponCollected = "weapon_collected";

        /// <summary>Second chance prompt response (revive accepted/declined).</summary>
        public const string SecondChanceUsed = "second_chance_used";

        /// <summary>Specific milestone event when player finishes Level 1 / reaches Level 2 (Conversion event for Google Ads).</summary>
        public const string Level2Reached = "level_2_reached";

        /// <summary>First level successfully finished.</summary>
        public const string FirstLevelCompleted = "first_level_completed";

        /// <summary>Level abandoned or quit early by player (e.g. return to menu or exit).</summary>
        public const string LevelAbandoned = "level_abandoned";
    }

    /// <summary>
    /// Parameter names for events (<= 40 chars, alphanumeric + underscore).
    /// String values must be <= 100 chars.
    /// </summary>
    public static class AnalyticsParams
    {
        // -- General & Screen --
        public const string ScreenName = "screen_name";
        public const string ScreenClass = "screen_class";

        // -- Level Flow --
        public const string LevelNumber = "level_number";
        public const string GameMode = "game_mode";
        public const string ControlStyle = "control_style";
        public const string Score = "score";
        public const string DurationSeconds = "duration_seconds";
        public const string DamageTaken = "damage_taken";
        public const string EnemiesKilled = "enemies_killed";
        public const string CollectablesCount = "collectables_count";
        public const string KilledBy = "killed_by";
        public const string IsBossLevel = "is_boss_level";
        public const string RemainingHealth = "remaining_health";
        public const string AbandonReason = "abandon_reason";

        // -- Boss --
        public const string BossNumber = "boss_number";
        public const string BossName = "boss_name";
        public const string IsVictory = "is_victory";

        // -- Diamond & Powerups --
        public const string DiamondColor = "diamond_color";
        public const string AbilityType = "ability_type";
        public const string WeaponType = "weapon_type";

        // -- Second Chance --
        public const string ReviveAccepted = "revive_accepted";
    }

    /// <summary>
    /// User properties (GA4 user properties) for player segmentation (<= 36 chars value limit).
    /// </summary>
    public static class AnalyticsUserProps
    {
        public const string SelectedLanguage = "selected_language";
        public const string ProgressBucket = "progress_bucket";
        public const string MaxUnlockedLevel = "max_unlocked_level";
        public const string HasPassedLevel1 = "has_passed_level_1";
        public const string ControlStyle = "control_style";
        public const string DeviceTier = "device_tier";
        public const string TotalPlaytimeMinutes = "total_playtime_minutes";
        public const string TotalDeaths = "total_deaths";
    }

    /// <summary>
    /// Standardized parameter and property values to prevent typos across callers.
    /// </summary>
    public static class AnalyticsValues
    {
        public const string Unknown = "unknown";

        // Diamonds
        public const string DiamondRed = "red";
        public const string DiamondBlue = "blue";
        public const string DiamondGreen = "green";
        public const string DiamondBlack = "black";

        // Abilities
        public const string AbilityPower = "power_attack";
        public const string AbilitySpeedy = "speedy_attack";
        public const string AbilityCritical = "critical_attack";
        public const string AbilityPoison = "poison_attack";

        // Weapons & Supplies
        public const string WeaponWrench = "wrench";
        public const string WeaponBubbleShield = "bubble_shield";
        public const string WeaponPixelSupply = "pixel_supply";
        public const string WeaponTripleFire = "triple_fire";
        public const string WeaponDiffusedFire = "diffused_fire";
        public const string WeaponWaveGun = "wave_gun";
        public const string WeaponRocket = "rocket";
        public const string WeaponBouncingBall = "bouncing_ball";
        public const string WeaponOrbitalFire = "orbital_fire";
    }
}
