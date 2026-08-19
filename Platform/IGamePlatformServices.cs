using System.Threading.Tasks;

namespace PaintTrek.Shared.Platform
{
    /// <summary>Game modes that can have platform-specific access rules.</summary>
    public enum GameModeId
    {
        Normal,
        Endless,
        UfoInvasion,
        AgainstAllBosses
    }

    public enum StorePlatform
    {
        GooglePlay,
        MicrosoftStore,
        Steam
    }

    /// <summary>
    /// Boundary between game UI and an app-store implementation. Screens query mode access here;
    /// they never reference a billing SDK or platform leaderboard directly.
    /// </summary>
    public interface IGamePlatformServices
    {
        StorePlatform Platform { get; }
        bool HasLeaderboards { get; }
        bool IsModeAvailable(GameModeId mode);
        Task<bool> RequestModeAccessAsync(GameModeId mode);
        void SubmitScore(string leaderboardId, long score);
    }
}
