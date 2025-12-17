# PaintTrek Statistics System

## 🎯 Overview

Comprehensive statistics tracking system for PaintTrek game across all platforms (Desktop, Android, Xbox).

## 📦 Components

### Core Classes
- **StatisticsManager** - Singleton manager for tracking game events
- **GameSessionStats** - Data model for a single game session
- **StatisticsStorage** - JSON file persistence layer
- **DamageEvent** - Model for damage tracking

### Integration Points
- **Player.TakeDamage()** - Tracks damage taken by player
- **Enemy.TakeDamage()** - Tracks enemy kills and weapon usage
- **Level** - Manages session lifecycle (start/complete)
- **Game1** - Initializes storage and event handlers

## 🚀 Quick Start

### 1. Start a Session
```csharp
// In Level constructor
StatisticsManager.Instance.StartSession(levelNumber);
```

### 2. Track Events
```csharp
// Damage tracking (automatic in Player.TakeDamage)
StatisticsManager.Instance.RecordDamage(
    damageSource: "Cacao",
    damageAmount: 10,
    playerHealthAfter: 90f,
    wasFatal: false
);

// Enemy kill tracking (automatic in Enemy.TakeDamage)
StatisticsManager.Instance.RecordEnemyKill(
    enemyType: "Cacao",
    weaponUsed: "Laser"
);
```

### 3. Complete Session
```csharp
// On level completion or game over
StatisticsManager.Instance.CompleteSession(
    finalScore: Score,
    isCompleted: true,
    isGameOver: false
);
```

### 4. Save to File
```csharp
// In Game1.LoadContent() - automatic via event
StatisticsManager.Instance.OnSessionCompleted += (session) =>
{
    statisticsStorage.SaveSession(session);
};
```

## 📊 Tracked Data

### Player Stats
- Damage taken (source, amount, health after)
- Death count
- Fatal damage events

### Enemy Stats
- Kills by enemy type (Cacao, Trilobit, Boss, etc.)
- Weapon usage (Laser, Rocket, TripleFire, etc.)
- Total kills

### Session Stats
- Level number
- Final score
- Play duration
- Completion status
- Timestamps

## 💾 Storage

### File Format
JSON files stored in platform-specific locations:

**Desktop**: `%USERPROFILE%\Documents\PaintTrek\Statistics\`
**Android**: `/data/data/com.painttrek.android/files/PaintTrek/Statistics/`

### Example Session File
```json
{
  "LevelNumber": 1,
  "FinalScore": 1500,
  "IsCompleted": true,
  "IsGameOver": false,
  "PlayDuration": "00:02:30",
  "EnemyKills": {
    "Cacao": 5,
    "Trilobit": 3
  },
  "WeaponUsage": {
    "Laser": 8,
    "Rocket": 2
  },
  "DamageEvents": [
    {
      "DamageSource": "Cacao",
      "DamageAmount": 10,
      "PlayerHealthAfter": 90.0,
      "WasFatal": false
    }
  ],
  "TotalEnemyKills": 8,
  "TotalDamageTaken": 45,
  "DeathCount": 0
}
```

## 🧪 Testing

### Debug Output
```csharp
var session = StatisticsManager.Instance.GetCurrentSession();
if (session != null)
{
    Debug.WriteLine($"Kills: {session.TotalEnemyKills}");
    Debug.WriteLine($"Damage: {session.TotalDamageTaken}");
    Debug.WriteLine($"Deaths: {session.DeathCount}");
}
```

### Test Scenarios
See `STATISTICS_TEST_SCENARIOS.md` for comprehensive test cases.

## 📚 Documentation

- **STATISTICS_INTEGRATION_GUIDE.md** - Full integration guide
- **STATISTICS_QUICK_START.md** - Quick start for Desktop
- **STATISTICS_DAMAGE_TRACKING.md** - Damage flow architecture
- **STATISTICS_TEST_SCENARIOS.md** - Test scenarios
- **STATISTICS_IMPLEMENTATION_SUMMARY.md** - Implementation status

## ✅ Platform Status

| Platform | Status | Notes |
|----------|--------|-------|
| Desktop  | ✅ Complete | Fully tested |
| Android  | ✅ Complete | Fully integrated |
| Xbox     | ⚠️ TODO | Copy from Desktop |

## 🔧 API Reference

### StatisticsManager

#### Methods
- `StartSession(int levelNumber)` - Start tracking a new session
- `CompleteSession(int finalScore, bool isCompleted, bool isGameOver)` - End session
- `RecordDamage(string damageSource, int damageAmount, float playerHealthAfter, bool wasFatal)` - Track damage
- `RecordEnemyKill(string enemyType, string weaponUsed)` - Track enemy kill
- `RecordCollectable(string collectableType)` - Track collectable (future)
- `RecordShot(bool hit)` - Track shot accuracy (future)
- `GetCurrentSession()` - Get current session data
- `IsSessionActive()` - Check if session is active

#### Events
- `OnSessionCompleted` - Fired when session ends
- `OnEnemyKilled` - Fired when enemy is killed
- `OnCollectableCollected` - Fired when collectable is picked up
- `OnPlayerDamaged` - Fired when player takes damage

### StatisticsStorage

#### Methods
- `SaveSession(GameSessionStats session)` - Save session to JSON file
- `LoadAllSessions()` - Load all saved sessions
- `GetSessionsByLevel(int levelNumber)` - Get sessions for specific level
- `GetLatestSession()` - Get most recent session

## 🎮 Usage Examples

### Example 1: Track Custom Event
```csharp
// In your game code
if (player.CollectedPowerup())
{
    StatisticsManager.Instance.RecordCollectable("RocketSupply");
}
```

### Example 2: Display Stats in UI
```csharp
var session = StatisticsManager.Instance.GetCurrentSession();
if (session != null)
{
    killsText.Text = $"Kills: {session.TotalEnemyKills}";
    scoreText.Text = $"Score: {session.FinalScore}";
}
```

### Example 3: Load Historical Data
```csharp
var storage = new StatisticsStorage(storagePath);
var allSessions = storage.LoadAllSessions();

int totalKills = allSessions.Sum(s => s.TotalEnemyKills);
Debug.WriteLine($"Total kills across all sessions: {totalKills}");
```

## 🐛 Troubleshooting

### Statistics not saving?
1. Check if `OnSessionCompleted` event is subscribed in `Game1.LoadContent()`
2. Verify storage path exists and is writable
3. Check debug output for error messages

### Enemy kills not counting?
1. Verify `Enemy.TakeDamage()` is being called
2. Check if session is active: `StatisticsManager.Instance.IsSessionActive()`
3. Ensure enemy health goes from > 0 to <= 0

### Damage not tracking?
1. Verify `Player.TakeDamage()` has `RecordDamage()` call
2. Check if session was started in Level constructor
3. Verify damage amount is > 0

## 🚀 Future Enhancements

- [ ] Aggregate statistics across sessions
- [ ] Leaderboard integration (Xbox Live, Google Play)
- [ ] Achievement system
- [ ] In-game statistics dashboard
- [ ] Shot accuracy tracking
- [ ] Combo system
- [ ] Weapon efficiency metrics

## 📞 Support

For questions or issues, check the documentation files in this folder or enable debug mode:

```csharp
Globals.DeveloperMode = true;
```

---

**Version**: 1.0  
**Last Updated**: December 7, 2025  
**Status**: Production Ready
