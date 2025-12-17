using System;
using System.Collections.Generic;
using System.Linq;

namespace PaintTrek.Shared.Statistics
{
    /// <summary>
    /// Bir level'ın tüm oynanışlarının toplu istatistikleri
    /// Maksimum, minimum, ortalama değerler
    /// </summary>
    public class LevelAggregateStats
    {
        public int LevelNumber { get; set; }
        public DateTime FirstPlayed { get; set; }
        public DateTime LastPlayed { get; set; }
        
        // Oynanış sayısı
        public int TotalAttempts { get; set; }
        public int CompletedAttempts { get; set; }
        public int FailedAttempts { get; set; }
        
        // Skor istatistikleri
        public int HighScore { get; set; }
        public int LowestScore { get; set; }
        public double AverageScore { get; set; }
        
        // Süre istatistikleri
        public TimeSpan FastestCompletion { get; set; }
        public TimeSpan SlowestCompletion { get; set; }
        public TimeSpan AverageCompletion { get; set; }
        
        // Düşman istatistikleri
        public Dictionary<string, EnemyTypeStats> EnemyStats { get; set; }
        public int TotalKills { get; set; }
        public int MaxKillsInSingleRun { get; set; }
        
        // Collectable istatistikleri
        public Dictionary<string, int> TotalCollectables { get; set; }
        public int MaxCollectablesInSingleRun { get; set; }
        
        // Hasar istatistikleri
        public int TotalDamageTaken { get; set; }
        public int TotalDeaths { get; set; }
        public int LeastDamageTaken { get; set; }
        public int MostDamageTaken { get; set; }
        
        // Performans
        public float BestAccuracy { get; set; }
        public float WorstAccuracy { get; set; }
        public float AverageAccuracy { get; set; }

        public LevelAggregateStats()
        {
            EnemyStats = new Dictionary<string, EnemyTypeStats>();
            TotalCollectables = new Dictionary<string, int>();
            FastestCompletion = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Yeni bir session'dan istatistikleri güncelle
        /// </summary>
        public void UpdateFromSession(GameSessionStats session)
        {
            if (session.LevelNumber != LevelNumber)
                return;

            // İlk/son oynanış
            if (FirstPlayed == default || session.CreatedDate < FirstPlayed)
                FirstPlayed = session.CreatedDate;
            
            LastPlayed = session.CreatedDate;

            // Oynanış sayıları
            TotalAttempts++;
            if (session.IsCompleted)
                CompletedAttempts++;
            else if (session.IsGameOver)
                FailedAttempts++;

            // Skor
            if (session.FinalScore > HighScore)
                HighScore = session.FinalScore;
            
            if (LowestScore == 0 || session.FinalScore < LowestScore)
                LowestScore = session.FinalScore;
            
            AverageScore = ((AverageScore * (TotalAttempts - 1)) + session.FinalScore) / TotalAttempts;

            // Süre (sadece tamamlananlar)
            if (session.IsCompleted)
            {
                if (session.PlayDuration < FastestCompletion)
                    FastestCompletion = session.PlayDuration;
                
                if (session.PlayDuration > SlowestCompletion)
                    SlowestCompletion = session.PlayDuration;
            }

            // Düşman kills
            TotalKills += session.TotalEnemyKills;
            if (session.TotalEnemyKills > MaxKillsInSingleRun)
                MaxKillsInSingleRun = session.TotalEnemyKills;

            foreach (var enemy in session.EnemyKills)
            {
                if (!EnemyStats.ContainsKey(enemy.Key))
                    EnemyStats[enemy.Key] = new EnemyTypeStats { EnemyType = enemy.Key };
                
                EnemyStats[enemy.Key].TotalKills += enemy.Value;
                if (enemy.Value > EnemyStats[enemy.Key].MaxKillsInSingleRun)
                    EnemyStats[enemy.Key].MaxKillsInSingleRun = enemy.Value;
            }

            // Collectables
            foreach (var collectable in session.CollectablesCollected)
            {
                if (!TotalCollectables.ContainsKey(collectable.Key))
                    TotalCollectables[collectable.Key] = 0;
                
                TotalCollectables[collectable.Key] += collectable.Value;
            }

            if (session.TotalCollectables > MaxCollectablesInSingleRun)
                MaxCollectablesInSingleRun = session.TotalCollectables;

            // Hasar
            TotalDamageTaken += session.TotalDamageTaken;
            TotalDeaths += session.DeathCount;
            
            if (LeastDamageTaken == 0 || session.TotalDamageTaken < LeastDamageTaken)
                LeastDamageTaken = session.TotalDamageTaken;
            
            if (session.TotalDamageTaken > MostDamageTaken)
                MostDamageTaken = session.TotalDamageTaken;

            // Accuracy
            if (session.Accuracy > BestAccuracy)
                BestAccuracy = session.Accuracy;
            
            if (WorstAccuracy == 0 || session.Accuracy < WorstAccuracy)
                WorstAccuracy = session.Accuracy;
            
            AverageAccuracy = ((AverageAccuracy * (TotalAttempts - 1)) + session.Accuracy) / TotalAttempts;
        }
    }

    /// <summary>
    /// Belirli bir düşman türü için istatistikler
    /// </summary>
    public class EnemyTypeStats
    {
        public string EnemyType { get; set; }
        public int TotalKills { get; set; }
        public int MaxKillsInSingleRun { get; set; }
    }
}
