# Statistics System - Test Scenarios

## Test Senaryoları

### Senaryo 1: Temel Damage Tracking
**Amaç:** Player'ın enemy'den hasar almasını test et

**Adımlar:**
1. Oyunu başlat (Level 1)
2. Bir enemy ile çarpış
3. Player'ın health'ini kontrol et
4. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
// StatisticsManager.Instance.GetCurrentSession()
DamageEvents.Count > 0
DamageEvents[0].DamageSource = "Cacao" (veya başka enemy tipi)
DamageEvents[0].DamageAmount > 0
DamageEvents[0].PlayerHealthAfter < 100
```

---

### Senaryo 2: Enemy Kill Tracking
**Amaç:** Enemy öldürme istatistiklerini test et

**Adımlar:**
1. Oyunu başlat
2. Laser ile bir enemy'yi öldür
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
EnemyKills["Cacao"] = 1 (veya öldürülen enemy tipi)
WeaponUsage["Laser"] = 1
TotalEnemyKills = 1
```

---

### Senaryo 3: Farklı Silahlarla Kill Tracking
**Amaç:** Farklı silahların kill istatistiklerini test et

**Adımlar:**
1. Oyunu başlat
2. Rocket supply topla
3. Rocket ile enemy öldür
4. Laser ile enemy öldür
5. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
WeaponUsage["Rocket"] = 1
WeaponUsage["Laser"] = 1
TotalEnemyKills = 2
```

---

### Senaryo 4: Player Death Tracking
**Amaç:** Player ölümünü test et

**Adımlar:**
1. Oyunu başlat
2. Kasıtlı olarak enemy'lerle çarpışarak öl
3. Game Over ekranına gel
4. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
DeathCount = 1
IsGameOver = true
IsCompleted = false
DamageEvents.Any(d => d.WasFatal) = true
```

---

### Senaryo 5: Level Completion
**Amaç:** Level tamamlama istatistiklerini test et

**Adımlar:**
1. Oyunu başlat
2. Level'i tamamla (exit door'a ulaş)
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
IsCompleted = true
IsGameOver = false
FinalScore > 0
PlayDuration > TimeSpan.Zero
```

---

### Senaryo 6: Multiple Enemy Types
**Amaç:** Farklı enemy tiplerinin istatistiklerini test et

**Adımlar:**
1. Oyunu başlat
2. Farklı enemy tiplerini öldür (Cacao, Trilobit, Boss, vb.)
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
EnemyKills.Count > 1
EnemyKills["Cacao"] > 0
EnemyKills["Trilobit"] > 0
// Her enemy tipi için ayrı sayaç
```

---

### Senaryo 7: Enemy Bullet Damage
**Amaç:** Enemy bullet'lardan alınan hasarı test et

**Adımlar:**
1. Oyunu başlat
2. Enemy bullet'ına çarp
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
DamageEvents.Any(d => d.DamageSource.Contains("Bullet")) = true
// veya
DamageEvents.Any(d => d.DamageSource == "EnemyLaser") = true
```

---

### Senaryo 8: Player Collision Kill
**Amaç:** Player'ın enemy ile çarpışarak öldürmesini test et

**Adımlar:**
1. Oyunu başlat
2. Bir enemy ile çarpış (enemy'nin health'i düşükse öldürür)
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
WeaponUsage["PlayerCollision"] > 0
// Player hem hasar alır hem de enemy'yi öldürür
DamageEvents.Count > 0
EnemyKills.Count > 0
```

---

### Senaryo 9: Off-Screen Enemy (No Kill Credit)
**Amaç:** Ekrandan kaçan enemy'lerin kill olarak sayılmadığını test et

**Adımlar:**
1. Oyunu başlat
2. Enemy'lerin ekranın sol tarafından kaçmasını bekle
3. İstatistikleri kontrol et

**Beklenen Sonuç:**
```csharp
// Kaçan enemy'ler için kill kaydı YOK
TotalEnemyKills = sadece öldürülen enemy sayısı
```

---

### Senaryo 10: Session Persistence
**Amaç:** İstatistiklerin dosyaya kaydedildiğini test et

**Adımlar:**
1. Oyunu başlat
2. Birkaç enemy öldür
3. Level'i tamamla veya öl
4. Dosya sistemini kontrol et

**Beklenen Dosya:**
```
%USERPROFILE%\Documents\PaintTrek\Statistics\
  - session_YYYYMMDD_HHMMSS.json
```

**Dosya İçeriği:**
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
      "WasFatal": false,
      "Timestamp": "2025-12-07T..."
    }
  ],
  "TotalEnemyKills": 8,
  "TotalDamageTaken": 45,
  "DeathCount": 0
}
```

---

## Debug Komutları

### İstatistikleri Console'da Göster
```csharp
var session = StatisticsManager.Instance.GetCurrentSession();
if (session != null)
{
    System.Diagnostics.Debug.WriteLine($"Level: {session.LevelNumber}");
    System.Diagnostics.Debug.WriteLine($"Score: {session.FinalScore}");
    System.Diagnostics.Debug.WriteLine($"Kills: {session.TotalEnemyKills}");
    System.Diagnostics.Debug.WriteLine($"Damage: {session.TotalDamageTaken}");
    System.Diagnostics.Debug.WriteLine($"Deaths: {session.DeathCount}");
    
    foreach (var kvp in session.EnemyKills)
        System.Diagnostics.Debug.WriteLine($"  {kvp.Key}: {kvp.Value}");
    
    foreach (var kvp in session.WeaponUsage)
        System.Diagnostics.Debug.WriteLine($"  {kvp.Key}: {kvp.Value}");
}
```

### Kaydedilen Dosyaları Listele
```csharp
var storage = new StatisticsStorage(storagePath);
var sessions = storage.LoadAllSessions();
foreach (var session in sessions)
{
    System.Diagnostics.Debug.WriteLine(
        $"Level {session.LevelNumber} - Score: {session.FinalScore} - " +
        $"Kills: {session.TotalEnemyKills} - Duration: {session.PlayDuration}"
    );
}
```

---

## Bilinen Sorunlar ve Çözümler

### Sorun 1: İstatistikler kaydedilmiyor
**Çözüm:** 
- `Game1.cs`'de `OnSessionCompleted` event handler'ının bağlı olduğunu kontrol et
- Storage path'in doğru olduğunu kontrol et
- Dosya yazma izinlerini kontrol et

### Sorun 2: Enemy kill sayılmıyor
**Çözüm:**
- `Enemy.TakeDamage()` metodunun çağrıldığını kontrol et
- `healthBefore > 0 && GetHealth() <= 0` koşulunun sağlandığını kontrol et
- `StatisticsManager.Instance.IsSessionActive()` true dönüyor mu?

### Sorun 3: Damage tracking çalışmıyor
**Çözüm:**
- `Player.TakeDamage()` metodunun çağrıldığını kontrol et
- `another.GetType().Name` doğru enemy tipini döndürüyor mu?
- Session başlatıldı mı? (`StartSession()` çağrıldı mı?)

---

## Performance Notları

- İstatistik kayıtları RAM'de tutulur, level bitince dosyaya yazılır
- Her frame'de istatistik kaydedilmez, sadece event'ler tetiklendiğinde
- Dosya yazma işlemi async değil, level geçişinde kısa bir gecikme olabilir
- Çok fazla damage event'i varsa (örn. 1000+), JSON serialize yavaş olabilir

---

## Gelecek Geliştirmeler

1. **Real-time Dashboard**: Oyun içinde istatistikleri göster
2. **Leaderboard Integration**: Xbox Live / Google Play ile entegrasyon
3. **Achievement System**: İstatistiklere dayalı başarımlar
4. **Analytics**: Aggregate statistics across all players
5. **Replay System**: Damage events'lerden replay oluştur
