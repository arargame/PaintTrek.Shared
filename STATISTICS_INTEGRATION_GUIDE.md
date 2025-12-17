# Statistics System Integration Guide

## 📊 Genel Bakış

PaintTrek.Shared istatistik sistemi, tüm platformlarda (Desktop, Android, Xbox) ortak çalışır.

### Özellikler:
- ✅ Her level oynanışı ayrı kaydedilir (Guid ID ile)
- ✅ Düşman öldürme sayıları (türe göre)
- ✅ Collectable toplama sayıları
- ✅ Player hasar kayıtları (kaynak, miktar, zaman)
- ✅ Silah kullanım istatistikleri
- ✅ Level bazlı toplu istatistikler (max, min, avg)
- ✅ DateTime CreatedDate & UpdateDate
- ✅ JSON dosyaya kayıt
- ✅ Performansa sıfır etki (level bitince kayıt)

## 🎯 Entegrasyon Adımları

### 1. Projelere Referans Ekle

**Desktop (PaintTrek.csproj):**
```xml
<ItemGroup>
  <ProjectReference Include="..\PaintTrek.Shared\PaintTrek.Shared.csproj" />
</ItemGroup>
```

**Android (PaintTrek.Android.csproj):**
```xml
<ItemGroup>
  <ProjectReference Include="..\PaintTrek.Shared\PaintTrek.Shared.csproj" />
</ItemGroup>
```

**Xbox (PaintTrek.Xbox.csproj):**
```xml
<ItemGroup>
  <ProjectReference Include="..\PaintTrek.Shared\PaintTrek.Shared.csproj" />
</ItemGroup>
```

### 2. Game1.cs - Initialize

```csharp
using PaintTrek.Shared.Statistics;

protected override void Initialize()
{
    base.Initialize();
    
    // Storage path platform'a göre değişir
    string storagePath = GetStatisticsStoragePath();
    var storage = new StatisticsStorage(storagePath);
    
    // StatisticsManager event'lerini dinle
    StatisticsManager.Instance.OnSessionCompleted += (session) =>
    {
        // Session tamamlandı, kaydet
        storage.SaveSession(session);
    };
}

private string GetStatisticsStoragePath()
{
#if XBOX
    return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#elif ANDROID
    return Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
#else
    return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PaintTrek", "Statistics");
#endif
}
```

### 3. Level.cs - Session Başlat/Bitir

```csharp
using PaintTrek.Shared.Statistics;

public class Level
{
    public void LoadContent()
    {
        // Level başladığında session başlat
        StatisticsManager.Instance.StartSession(LevelCounter);
    }

    public void Update()
    {
        // Level tamamlandığında
        if (ReachedExit)
        {
            StatisticsManager.Instance.CompleteSession(
                finalScore: player.Score,
                isCompleted: true,
                isGameOver: false
            );
        }
        
        // Game over olduğunda
        if (GetGameState() == GameState.GameOver)
        {
            StatisticsManager.Instance.CompleteSession(
                finalScore: player.Score,
                isCompleted: false,
                isGameOver: true
            );
        }
    }
}
```

### 4. Enemy.cs - Düşman Öldürme

```csharp
using PaintTrek.Shared.Statistics;

public class Enemy : Sprite
{
    public override void Kill()
    {
        base.Kill();
        
        // İstatistiğe kaydet
        StatisticsManager.Instance.RecordEnemyKill(
            enemyType: this.GetType().Name, // "Cacao", "Boss", vb.
            weaponUsed: "PlayerBullet"
        );
    }
}
```

### 5. CollectableObject.cs - Item Toplama

```csharp
using PaintTrek.Shared.Statistics;

public class CollectableObject : Sprite
{
    public void Collect()
    {
        // İstatistiğe kaydet
        StatisticsManager.Instance.RecordCollectable(
            collectableType: this.GetType().Name // "HealthPack", "Coin", vb.
        );
        
        // Normal collect logic
        this.IsActive = false;
    }
}
```

### 6. Player.cs - Hasar Alma

```csharp
using PaintTrek.Shared.Statistics;

public class Player : Sprite
{
    public void TakeDamage(int damage, string damageSource)
    {
        Health -= damage;
        
        bool wasFatal = Health <= 0;
        
        // İstatistiğe kaydet
        StatisticsManager.Instance.RecordDamage(
            damageSource: damageSource, // "Cacao", "EnemyBullet", "Collision"
            damageAmount: damage,
            playerHealthAfter: Health,
            wasFatal: wasFatal
        );
        
        if (wasFatal)
        {
            Die();
        }
    }
}
```

### 7. Bullet.cs - Atış İstatistikleri

```csharp
using PaintTrek.Shared.Statistics;

public class Bullet : Sprite
{
    public void Fire()
    {
        // Atış kaydedildi
        StatisticsManager.Instance.RecordShot(hit: false);
    }
    
    public void OnHit(Sprite target)
    {
        if (target is Enemy)
        {
            // İsabet kaydedildi
            StatisticsManager.Instance.RecordShot(hit: true);
        }
    }
}
```

## 📈 İstatistikleri Görüntüleme

### Mevcut Session'ı Göster (Oyun içi UI)

```csharp
var session = StatisticsManager.Instance.GetCurrentSession();

if (session != null)
{
    Console.WriteLine($"Kills: {session.TotalEnemyKills}");
    Console.WriteLine($"Score: {session.FinalScore}");
    Console.WriteLine($"Accuracy: {session.Accuracy:F1}%");
}
```

### Level Toplu İstatistikleri

```csharp
var storage = new StatisticsStorage(storagePath);
var levelStats = storage.GetLevelAggregate(levelNumber: 4);

if (levelStats != null)
{
    Console.WriteLine($"Level 4 Stats:");
    Console.WriteLine($"  Played: {levelStats.TotalAttempts} times");
    Console.WriteLine($"  Completed: {levelStats.CompletedAttempts} times");
    Console.WriteLine($"  High Score: {levelStats.HighScore}");
    Console.WriteLine($"  Fastest: {levelStats.FastestCompletion}");
    Console.WriteLine($"  Total Kills: {levelStats.TotalKills}");
    
    foreach (var enemy in levelStats.EnemyStats)
    {
        Console.WriteLine($"  {enemy.Key}: {enemy.Value.TotalKills} kills");
    }
}
```

### Tüm Session'ları Listele

```csharp
var storage = new StatisticsStorage(storagePath);
var sessions = storage.LoadSessionsForLevel(levelNumber: 4);

foreach (var session in sessions.OrderByDescending(s => s.FinalScore).Take(10))
{
    Console.WriteLine($"{session.CreatedDate:yyyy-MM-dd HH:mm} - Score: {session.FinalScore}");
}
```

## 🎮 Platform-Specific Storage Paths

### Desktop (Windows)
```
C:\Users\[Username]\AppData\Roaming\PaintTrek\Statistics\
├── game_sessions.json
└── level_aggregates.json
```

### Android
```
/storage/emulated/0/Android/data/com.painttrek/files/Statistics/
├── game_sessions.json
└── level_aggregates.json
```

### Xbox (UWP)
```
LocalState\Statistics\
├── game_sessions.json
└── level_aggregates.json
```

## 📊 JSON Dosya Formatı

### game_sessions.json
```json
[
  {
    "Id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "LevelNumber": 4,
    "CreatedDate": "2025-01-15T14:30:00Z",
    "CompletedDate": "2025-01-15T14:45:00Z",
    "IsCompleted": true,
    "IsGameOver": false,
    "FinalScore": 125000,
    "PlayDuration": "00:15:00",
    "EnemyKills": {
      "Cacao": 45,
      "Boss": 1
    },
    "TotalEnemyKills": 46,
    "CollectablesCollected": {
      "HealthPack": 3,
      "Coin": 20
    },
    "TotalCollectables": 23,
    "DamageEvents": [
      {
        "Timestamp": "2025-01-15T14:32:00Z",
        "DamageSource": "Cacao",
        "DamageAmount": 10,
        "PlayerHealthAfter": 90.0,
        "WasFatal": false
      }
    ],
    "TotalDamageTaken": 50,
    "DeathCount": 0,
    "WeaponUsage": {
      "PlayerBullet": 200
    },
    "TotalShotsFired": 200,
    "TotalHits": 150,
    "Accuracy": 75.0,
    "KillsPerMinute": 3.07
  }
]
```

## 🔧 Performans Notları

- ✅ **Sıfır runtime overhead**: İstatistikler sadece event'lerde güncellenir
- ✅ **Async kayıt yok**: Level bitince senkron kayıt (hızlı)
- ✅ **Memory efficient**: Sadece mevcut session bellekte
- ✅ **File size**: ~1KB per session, ~10KB per level aggregate

## 🐛 Debug & Test

### Test Session Oluştur

```csharp
StatisticsManager.Instance.StartSession(1);
StatisticsManager.Instance.RecordEnemyKill("Cacao");
StatisticsManager.Instance.RecordEnemyKill("Cacao");
StatisticsManager.Instance.RecordCollectable("Coin");
StatisticsManager.Instance.RecordDamage("Cacao", 10, 90, false);
StatisticsManager.Instance.CompleteSession(5000, true, false);
```

### İstatistikleri Temizle

```csharp
var storage = new StatisticsStorage(storagePath);
storage.ClearAllStats();
```

## 📱 UI Ekranları (Gelecek)

1. **Stats Screen** - Genel istatistikler
2. **Level Stats Screen** - Level bazlı detaylar
3. **Session History** - Geçmiş oynanışlar
4. **Leaderboard** - En yüksek skorlar

---

**Not**: Sistem şu an tamamen hazır ve çalışır durumda. Sadece projelere referans ekleyip yukarıdaki entegrasyonları yapman yeterli!
