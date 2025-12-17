using System;
using System.Collections.Generic;

namespace PaintTrek.Shared.Statistics
{
    /// <summary>
    /// Tek bir oyun oturumunun tüm istatistikleri
    /// Her level oynanışında yeni bir session oluşur
    /// </summary>
    public class GameSessionStats
    {
        public Guid Id { get; set; }
        public int LevelNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        
        // Genel bilgiler
        public bool IsCompleted { get; set; }
        public bool IsGameOver { get; set; }
        public int FinalScore { get; set; }
        public TimeSpan PlayDuration { get; set; }
        
        // Düşman istatistikleri
        public Dictionary<string, int> EnemyKills { get; set; }
        public int TotalEnemyKills { get; set; }
        
        // Collectable istatistikleri
        public Dictionary<string, int> CollectablesCollected { get; set; }
        public int TotalCollectables { get; set; }
        
        // Player hasar istatistikleri
        public List<DamageEvent> DamageEvents { get; set; }
        public int TotalDamageTaken { get; set; }
        public int DeathCount { get; set; }
        
        // Silah kullanımı
        public Dictionary<string, int> WeaponUsage { get; set; }
        public int TotalShotsFired { get; set; }
        public int TotalHits { get; set; }
        
        // Performans
        public float Accuracy => TotalShotsFired > 0 ? (float)TotalHits / TotalShotsFired * 100 : 0;
        public float KillsPerMinute => PlayDuration.TotalMinutes > 0 ? TotalEnemyKills / (float)PlayDuration.TotalMinutes : 0;

        public GameSessionStats()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            EnemyKills = new Dictionary<string, int>();
            CollectablesCollected = new Dictionary<string, int>();
            DamageEvents = new List<DamageEvent>();
            WeaponUsage = new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// Player'ın aldığı her hasar kaydı
    /// </summary>
    public class DamageEvent
    {
        public DateTime Timestamp { get; set; }
        public string DamageSource { get; set; } // "Cacao", "Bullet", "Collision"
        public int DamageAmount { get; set; }
        public float PlayerHealthAfter { get; set; }
        public bool WasFatal { get; set; }

        public DamageEvent(string source, int amount, float healthAfter, bool fatal)
        {
            Timestamp = DateTime.UtcNow;
            DamageSource = source;
            DamageAmount = amount;
            PlayerHealthAfter = healthAfter;
            WasFatal = fatal;
        }
    }
}
