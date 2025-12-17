# Damage & Kill Tracking System

## Overview
This document explains how damage and kill statistics are tracked throughout the game.

## Damage Flow Architecture

### 1. Player Takes Damage
**Location:** `Player.TakeDamage(Sprite another)`

**Sources:**
- Enemy collision (direct contact)
- EnemyBullet collision
- Boss attacks

**Tracking:**
```csharp
StatisticsManager.Instance.RecordDamage(
    damageSource: another.GetType().Name,  // "Cacao", "EnemyLaser", "Boss", etc.
    damageAmount: (int)damageAmount,
    playerHealthAfter: (float)GetHealth(),
    wasFatal: GetHealth() <= 0
);
```

**Collision Points:**
- `Player.CollisionDetectionWithOthers()` → Enemy collision → `enemy.TakeDamage(this)` + `TakeDamage(enemy)`
- `EnemyBullet.CollisionDetectionWithPlayer()` → `player.TakeDamage(this)`

---

### 2. Enemy Takes Damage & Dies
**Location:** `Enemy.TakeDamage(Sprite another)`

**Sources:**
- PlayerBullet collision (Laser, Rocket, TripleFire, etc.)
- Player collision (direct contact)
- Boss special attacks (friendly fire)

**Tracking:**
```csharp
// Check if damage was fatal
if (healthBefore > 0 && GetHealth() <= 0)
{
    string weaponUsed = "Unknown";
    if (another is PlayerBullet)
        weaponUsed = another.GetType().Name;  // "Laser", "Rocket", etc.
    else if (another is Player)
        weaponUsed = "PlayerCollision";
    
    StatisticsManager.Instance.RecordEnemyKill(
        enemyType: this.GetType().Name,  // "Cacao", "Trilobit", "Boss", etc.
        weaponUsed: weaponUsed
    );
}
```

**Collision Points:**
- `PlayerBullet.CollisionDetectionWithEnemies()` → `enemy.TakeDamage(this)`
- `Player.CollisionDetectionWithOthers()` → `enemy.TakeDamage(this)`

---

## Kill Detection Logic

### Enemy Kill
An enemy is considered "killed" when:
1. `healthBefore > 0` (was alive)
2. `GetHealth() <= 0` (now dead)
3. Killed by player weapon or collision (not by going off-screen)

### Off-Screen Enemies
Enemies that escape off-screen are NOT counted as kills:
- `Enemy.Kill()` method handles off-screen removal
- No statistics recorded for escaped enemies

---

## Weapon Type Detection

### Player Weapons
- **Laser** - Basic weapon
- **Rocket** - Explosive projectile
- **TripleFire** - Three-way shot
- **BouncingFire** - Bouncing projectile
- **DiffusedPlayerFire** - Spread shot
- **WaveGun** - Wave pattern
- **OrbitalFire** - Orbital attack
- **PlayerCollision** - Direct contact with enemy

### Enemy Weapons
- **EnemyLaser** - Basic enemy projectile
- **BossBullet** - Boss special attacks
- **Enemy** - Direct collision with enemy
- **Boss** - Direct collision with boss

---

## Statistics Storage

### Per-Session Tracking
All damage and kills are tracked in `GameSessionStats`:

```csharp
public class GameSessionStats
{
    public Dictionary<string, int> DamageBySource { get; set; }
    public Dictionary<string, int> EnemyKillsByType { get; set; }
    public Dictionary<string, int> WeaponKills { get; set; }
    public int TotalDamageTaken { get; set; }
    public int TotalEnemiesKilled { get; set; }
    public int Deaths { get; set; }
}
```

### Persistence
- Statistics are stored in RAM during gameplay
- Saved to JSON file when level ends
- Aggregated across multiple sessions

---

## Integration Points

### Desktop (PaintTrekMonogameDesktop)
✅ Player.TakeDamage() - Records damage taken
✅ Enemy.TakeDamage() - Records enemy kills

### Android (PaintTrek.Android)
✅ Player.TakeDamage() - Records damage taken
✅ Enemy.TakeDamage() - Records enemy kills

### Xbox (PaintTrek.Xbox)
⚠️ Needs integration (copy from Desktop)

---

## Testing Checklist

- [ ] Player takes damage from enemy collision
- [ ] Player takes damage from enemy bullets
- [ ] Player death is recorded
- [ ] Enemy killed by Laser
- [ ] Enemy killed by Rocket
- [ ] Enemy killed by special weapons
- [ ] Enemy killed by player collision
- [ ] Boss damage tracking
- [ ] Off-screen enemies NOT counted as kills
- [ ] Statistics persist after level completion

---

## Future Enhancements

1. **Accuracy Tracking**: Shots fired vs shots hit
2. **Combo System**: Track consecutive kills
3. **Damage Per Second**: Calculate DPS for each weapon
4. **Survival Time**: Track time between damage events
5. **Weapon Efficiency**: Kills per ammo used
