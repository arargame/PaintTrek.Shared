using System;
using System.Collections.Generic;

namespace PaintTrek.Shared.Statistics
{
    /// <summary>
    /// Oyun istatistiklerini yöneten merkezi sistem
    /// Singleton pattern - tüm platformlarda aynı şekilde çalışır
    /// </summary>
    public class StatisticsManager
    {
        private static StatisticsManager _instance;
        public static StatisticsManager Instance => _instance ??= new StatisticsManager();

        private GameSessionStats _currentSession;
        private DateTime _sessionStartTime;
        private bool _isSessionActive;

        // Event handlers
        public event Action<GameSessionStats> OnSessionCompleted;
        public event Action<string, int> OnEnemyKilled;
        public event Action<string> OnCollectableCollected;
        public event Action<DamageEvent> OnPlayerDamaged;

        private StatisticsManager() { }

        /// <summary>
        /// Yeni level başladığında session başlat
        /// </summary>
        public void StartSession(int levelNumber)
        {
            _currentSession = new GameSessionStats
            {
                LevelNumber = levelNumber,
                CreatedDate = DateTime.UtcNow
            };
            _sessionStartTime = DateTime.UtcNow;
            _isSessionActive = true;

            System.Diagnostics.Debug.WriteLine($"[Stats] Session started for Level {levelNumber}");
        }

        /// <summary>
        /// Level tamamlandığında session'ı bitir
        /// </summary>
        public void CompleteSession(int finalScore, bool isCompleted, bool isGameOver)
        {
            if (!_isSessionActive || _currentSession == null)
                return;

            _currentSession.CompletedDate = DateTime.UtcNow;
            _currentSession.PlayDuration = DateTime.UtcNow - _sessionStartTime;
            _currentSession.FinalScore = finalScore;
            _currentSession.IsCompleted = isCompleted;
            _currentSession.IsGameOver = isGameOver;
            _isSessionActive = false;

            // Toplamları hesapla
            _currentSession.TotalEnemyKills = 0;
            foreach (var kills in _currentSession.EnemyKills.Values)
                _currentSession.TotalEnemyKills += kills;

            _currentSession.TotalCollectables = 0;
            foreach (var count in _currentSession.CollectablesCollected.Values)
                _currentSession.TotalCollectables += count;

            _currentSession.TotalDamageTaken = 0;
            foreach (var dmg in _currentSession.DamageEvents)
                _currentSession.TotalDamageTaken += dmg.DamageAmount;

            System.Diagnostics.Debug.WriteLine($"[Stats] Session completed - Score: {finalScore}, Kills: {_currentSession.TotalEnemyKills}");

            // Event fırlat
            OnSessionCompleted?.Invoke(_currentSession);
        }

        /// <summary>
        /// Düşman öldürüldüğünde kaydet
        /// </summary>
        public void RecordEnemyKill(string enemyType, string weaponUsed = null)
        {
            if (!_isSessionActive || _currentSession == null)
                return;

            // Düşman sayısını artır
            if (!_currentSession.EnemyKills.ContainsKey(enemyType))
                _currentSession.EnemyKills[enemyType] = 0;
            
            _currentSession.EnemyKills[enemyType]++;

            // Silah kullanımını kaydet
            if (!string.IsNullOrEmpty(weaponUsed))
            {
                if (!_currentSession.WeaponUsage.ContainsKey(weaponUsed))
                    _currentSession.WeaponUsage[weaponUsed] = 0;
                
                _currentSession.WeaponUsage[weaponUsed]++;
            }

            OnEnemyKilled?.Invoke(enemyType, _currentSession.EnemyKills[enemyType]);
        }

        /// <summary>
        /// Collectable toplandığında kaydet
        /// </summary>
        public void RecordCollectable(string collectableType)
        {
            if (!_isSessionActive || _currentSession == null)
                return;

            if (!_currentSession.CollectablesCollected.ContainsKey(collectableType))
                _currentSession.CollectablesCollected[collectableType] = 0;
            
            _currentSession.CollectablesCollected[collectableType]++;

            OnCollectableCollected?.Invoke(collectableType);
        }

        /// <summary>
        /// Player hasar aldığında kaydet
        /// </summary>
        public void RecordDamage(string damageSource, int damageAmount, float playerHealthAfter, bool wasFatal)
        {
            if (!_isSessionActive || _currentSession == null)
                return;

            // Sadece gerçek hasar varsa kaydet (DamageAmount > 0)
            if (damageAmount <= 0)
                return;

            var damageEvent = new DamageEvent(damageSource, damageAmount, playerHealthAfter, wasFatal);
            _currentSession.DamageEvents.Add(damageEvent);

            if (wasFatal)
                _currentSession.DeathCount++;

            OnPlayerDamaged?.Invoke(damageEvent);
        }

        /// <summary>
        /// Atış istatistiklerini kaydet
        /// </summary>
        public void RecordShot(bool hit)
        {
            if (!_isSessionActive || _currentSession == null)
                return;

            _currentSession.TotalShotsFired++;
            if (hit)
                _currentSession.TotalHits++;
        }

        /// <summary>
        /// Mevcut session'ı al (debug/UI için)
        /// </summary>
        public GameSessionStats GetCurrentSession()
        {
            return _currentSession;
        }

        /// <summary>
        /// Session aktif mi?
        /// </summary>
        public bool IsSessionActive()
        {
            return _isSessionActive;
        }
    }
}
