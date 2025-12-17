using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PaintTrek.Shared.Statistics
{
    /// <summary>
    /// İstatistikleri dosyaya kaydetme/yükleme
    /// Platform-agnostic (her platform kendi storage path'ini verir)
    /// </summary>
    public class StatisticsStorage
    {
        private readonly string _storagePath;
        private const string SessionsFileName = "game_sessions.json";
        private const string AggregatesFileName = "level_aggregates.json";

        public StatisticsStorage(string storagePath)
        {
            _storagePath = storagePath;
            
            // Klasör yoksa oluştur
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        /// <summary>
        /// Session'ı kaydet
        /// </summary>
        public void SaveSession(GameSessionStats session)
        {
            try
            {
                var sessions = LoadAllSessions();
                sessions.Add(session);

                string json = JsonConvert.SerializeObject(sessions, Formatting.Indented);
                string filePath = Path.Combine(_storagePath, SessionsFileName);
                File.WriteAllText(filePath, json);

                System.Diagnostics.Debug.WriteLine($"[Storage] Session saved: {session.Id}");

                // Aggregate stats'ı güncelle
                UpdateAggregateStats(session);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Tüm session'ları yükle
        /// </summary>
        public List<GameSessionStats> LoadAllSessions()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, SessionsFileName);
                
                if (!File.Exists(filePath))
                    return new List<GameSessionStats>();

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<GameSessionStats>>(json) ?? new List<GameSessionStats>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Load error: {ex.Message}");
                return new List<GameSessionStats>();
            }
        }

        /// <summary>
        /// Belirli bir level'ın session'larını yükle
        /// </summary>
        public List<GameSessionStats> LoadSessionsForLevel(int levelNumber)
        {
            var allSessions = LoadAllSessions();
            return allSessions.Where(s => s.LevelNumber == levelNumber).ToList();
        }

        /// <summary>
        /// Aggregate stats'ı güncelle
        /// </summary>
        private void UpdateAggregateStats(GameSessionStats session)
        {
            try
            {
                var aggregates = LoadAggregateStats();
                
                var levelAggregate = aggregates.FirstOrDefault(a => a.LevelNumber == session.LevelNumber);
                if (levelAggregate == null)
                {
                    levelAggregate = new LevelAggregateStats { LevelNumber = session.LevelNumber };
                    aggregates.Add(levelAggregate);
                }

                levelAggregate.UpdateFromSession(session);

                string json = JsonConvert.SerializeObject(aggregates, Formatting.Indented);
                string filePath = Path.Combine(_storagePath, AggregatesFileName);
                File.WriteAllText(filePath, json);

                System.Diagnostics.Debug.WriteLine($"[Storage] Aggregate stats updated for Level {session.LevelNumber}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Aggregate update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Tüm aggregate stats'ları yükle
        /// </summary>
        public List<LevelAggregateStats> LoadAggregateStats()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, AggregatesFileName);
                
                if (!File.Exists(filePath))
                    return new List<LevelAggregateStats>();

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<LevelAggregateStats>>(json) ?? new List<LevelAggregateStats>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Load aggregates error: {ex.Message}");
                return new List<LevelAggregateStats>();
            }
        }

        /// <summary>
        /// Belirli bir level'ın aggregate stats'ını al
        /// </summary>
        public LevelAggregateStats GetLevelAggregate(int levelNumber)
        {
            var aggregates = LoadAggregateStats();
            return aggregates.FirstOrDefault(a => a.LevelNumber == levelNumber);
        }

        /// <summary>
        /// Tüm istatistikleri temizle (debug/test için)
        /// </summary>
        public void ClearAllStats()
        {
            try
            {
                string sessionsPath = Path.Combine(_storagePath, SessionsFileName);
                string aggregatesPath = Path.Combine(_storagePath, AggregatesFileName);

                if (File.Exists(sessionsPath))
                    File.Delete(sessionsPath);
                
                if (File.Exists(aggregatesPath))
                    File.Delete(aggregatesPath);

                System.Diagnostics.Debug.WriteLine("[Storage] All stats cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Storage] Clear error: {ex.Message}");
            }
        }
    }
}
