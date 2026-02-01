# Toji 3D Assassin Game - Architecture & API Documentation

## Overview

This is a complete 3D action game featuring Toji from Jujutsu Kaisen. The game includes:
- Full 3D combat system
- Advanced dialogue branching system
- Comprehensive skill tree
- 8+ unique stages
- Enemy AI with behavior trees
- Mission system with rewards

## Architecture

### Core Systems

#### 1. **Game Manager** (`GameManager.cs`)
Central game state controller handling:
- Game flow (Menu → Playing → Pause → GameOver)
- Stage progression
- Score tracking
- Target elimination tracking

**Key Methods:**
```csharp
void StartNewGame()              // Begin new game
void LoadStage(int stageIndex)   // Load specific stage
void TogglePause()               // Pause/Resume game
void EndGame(bool victory)       // End game with result
void AddScore(int points)        // Add to total score
```

**Events:**
```csharp
event Action<int> OnStageChanged
event Action<int> OnScoreChanged
event Action<int> OnTargetEliminated
event Action OnGamePaused
event Action OnGameResumed
event Action OnGameOver
```

#### 2. **Player Controller** (`PlayerController.cs`)
Manages player movement, combat, and abilities:
- 3D movement with sprinting
- Attack system (melee/ranged)
- Stamina management
- Health system
- Special abilities

**Key Methods:**
```csharp
void Attack()                    // Perform attack
void UseSpecialAbility()         // Use special power
void TakeDamage(float damage)    // Take damage
void Heal(float amount)          // Restore health
void ResetPlayer(Vector3 spawn)  // Reset to spawn
```

**Events:**
```csharp
event Action<float> OnHealthChanged
event Action<float> OnStaminaChanged
```

#### 3. **Dialogue System** (`DialogueSystem.cs`)
Complete branching dialogue system:
- Load dialogue from JSON files
- Support for choices and branching
- Reward integration (skills, items, score)
- NPC interaction

**Key Methods:**
```csharp
void StartConversation(string id, NPC npc)  // Begin dialogue
void SelectChoice(int choiceIndex)          // Choose dialogue option
void ExecuteDialogueEffect(DialogueEffect)  // Apply rewards
```

**Dialogue Data Structure:**
```json
{
  "nodes": [
    {
      "id": "unique_id",
      "characterName": "NPC Name",
      "dialogue": "Text spoken",
      "choices": [
        {
          "text": "Player response",
          "nextNodeId": "next_id",
          "effect": {"effectType": "SkillReward", ...}
        }
      ]
    }
  ]
}
```

#### 4. **Skill Tree** (`SkillTree.cs`)
Progressive skill unlocking system:
- Multiple skill branches
- Stat bonuses from skills
- Cooldown management
- Ability execution

**Skill Branches:**
- **Combat**: Enhanced attacks, combos, critical strikes
- **Stealth**: Silent movement, invisibility, detection evasion
- **Defense**: Dodging, blocking, damage reduction
- **Utility**: Stamina efficiency, speed boost, cooldown reduction

**Key Methods:**
```csharp
bool UnlockSkill(string skillId)        // Unlock new skill
bool IsSkillUnlocked(string skillId)    // Check if unlocked
bool CanUseAbility()                    // Check ability ready
void UseAbility(Vector3 pos, Vec3 dir)  // Execute ability
void AddSkillPoints(int amount)         // Add progression points
```

**Stat Multipliers:**
```csharp
float GetDamageMultiplier()       // Combat bonus
float GetDefenseMultiplier()      // Defense bonus
float GetStaminaEfficiency()      // Efficiency bonus
float GetMovementSpeedBonus()     // Speed bonus
```

#### 5. **Enemy AI** (`EnemyAI.cs`)
Sophisticated enemy behavior:
- State machine (Idle → Chase → Attack)
- Pathfinding and navigation
- Detection range system
- Combat behavior

**Enemy States:**
```csharp
enum EnemyState {
    Idle,      // Default state
    Chase,     // Pursuing player
    Attack,    // In combat
    Stunned,   // Disabled temporarily
    Dead       // Defeated
}
```

**Key Methods:**
```csharp
void Initialize(EnemySpawnData data)    // Setup enemy
void TakeDamage(float dmg, DamageType)  // Damage handling
void Stun(float duration)               // Disable briefly
float GetHealthPercent()                // Get health ratio
```

#### 6. **UI Manager** (`UIManager.cs`)
All UI screen management:
- Main menu
- Gameplay HUD
- Dialogue UI
- Skill tree UI
- Pause menu
- Game over screen

**Key Methods:**
```csharp
void ShowPanel(string panelName)                // Show UI panel
void HidePanel(string panelName)                // Hide UI panel
void UpdateHUDStats(int score, float hp, stamina)
void ShowSkillTreeUI(SkillTree tree)
void ShowGameOverScreen(bool victory, int score, int targets)
```

### Data Structures

#### Stage Configuration
```csharp
public class StageConfig
{
    public int stageNumber;
    public string stageName;
    public string description;
    public Vector3 playerSpawnPoint;
    public EnemyWave[] enemyWaves;
    public int baseScoreReward;
    public int skillPointReward;
    public string[] unlockedSkills;
}
```

#### Skill Node
```csharp
public class SkillNode
{
    public string skillId;
    public string skillName;
    public SkillBranch branch;
    public int skillPointCost;
    public string[] prerequisites;
    public float damageBonus;
    public float defenseBonus;
    public float staminaEfficiency;
    public float movementSpeedBonus;
}
```

#### Dialogue Node
```csharp
[System.Serializable]
public class DialogueNode
{
    public string id;
    public string characterName;
    public string dialogue;
    public DialogueChoice[] choices;
    public float displayDuration;
}
```

### Supporting Systems

#### Camera Controller (`CameraController.cs`)
Third-person camera system with:
- Mouse look support
- Smooth follow
- Aim mode for ranged attacks
- Configurable distance and height

#### Animation Controller (`AnimationController.cs`)
Bridges Animator with game logic:
- Movement animations
- Attack animations
- Special ability animations
- Hit/death reactions

#### Audio Manager (`AudioManager.cs`)
Sound management:
- Background music
- Sound effects
- Music pause/resume
- Audio clipping support

#### Inventory System (`Inventory.cs`)
Player inventory management:
- Weapon equipment
- Item tracking
- Ammunition management

#### Save Manager (`SaveManager.cs`)
Game persistence:
- Save game data
- Load saved games
- Player progress
- Stage completion

## Game Flow

```
Start
  ↓
Main Menu
  ├─ Start Game → Select Stage 1
  ├─ Story → Show Narrative
  └─ Controls → Show Input Guide
  ↓
Gameplay Loop
  ├─ Player Input Processing
  ├─ AI Update
  ├─ Combat Resolution
  ├─ Physics Simulation
  ├─ UI Refresh
  └─ Render
  ↓
Stage Complete/Failed
  ├─ Calculate Rewards
  ├─ Unlock Skills
  └─ Next Stage or Game Over
  ↓
End Game
```

## Stages Overview

| Stage | Name | Difficulty | Enemies | Theme |
|-------|------|-----------|---------|-------|
| 1 | Tokyo Jujutsu High | Easy | 2 | Introduction |
| 2 | Hidden Shrine | Medium | 3 | Isolation |
| 3 | Cursed Grounds | Medium | 4 | Outdoor Combat |
| 4 | Underground Temple | Medium | 3 | Stealth |
| 5 | Shibuya Incident | Hard | 5 | Chaos |
| 6 | Society HQ | Hard | 6 | Infiltration |
| 7 | Cursed Realm | Hard | 4 | Supernatural |
| 8 | Final Confrontation | Extreme | 1 Boss | Epic |

## Input System

| Input | Action |
|-------|--------|
| WASD | Move |
| Space | Jump |
| Mouse | Look Around |
| Left Click | Attack |
| Right Click | Aim |
| Q | Special Ability |
| E | Interact |
| Tab | Skill Tree |
| Shift | Sprint |
| Esc | Pause |

## Enemy Types

### Character Classes
1. **Yuji Itadori** - HP: 80, Damage: 15
2. **Megumi Fushiguro** - HP: 100, Damage: 20
3. **Nobara Kugisaki** - HP: 75, Damage: 18
4. **Jogo** - HP: 110, Damage: 25
5. **Mahito** - HP: 95, Damage: 22
6. **Satoru Gojo** - HP: 150, Damage: 30
7. **Ryomen Sukuna** (Boss) - HP: 180, Damage: 35

## Customization Guide

### Add New Stage
1. Edit `StagesConfiguration.cs`
2. Create new environment prefab
3. Define enemy spawn points
4. Set rewards and objectives
5. Add dialogue for the stage

### Add New Skill
1. Create `SkillNode` in editor
2. Configure prerequisites and costs
3. Implement ability in `SkillTree.UseAbility()`
4. Add visual effects
5. Balance stat bonuses

### Add New Dialogue
1. Add node to `DialogueData.json`
2. Link with choices
3. Define effects (rewards, unlocks)
4. Test dialogue flow

## Performance Optimization

1. **Object Pooling**: Reuse projectiles and effects
2. **LOD Groups**: Reduce detail at distance
3. **Occlusion Culling**: Hide off-screen objects
4. **Animation Optimization**: Use animation layers
5. **Shader Optimization**: Use simple shaders for less important objects

## Extension Points

- `EnemyAI.cs`: Add new enemy behaviors
- `SkillTree.cs`: Implement new special abilities
- `DialogueSystem.cs`: Add dialogue effects
- `CombatSystem.cpp`: Advanced physics/networking
- `UIManager.cs`: Add new UI screens

## Version Information

- **Engine**: Unity 2021 LTS+
- **Language**: C# (with optional C++ plugins)
- **Target Platforms**: Windows, Mac, Linux
- **Current Version**: 1.0.0

## License & Credits

Original Character: Jujutsu Kaisen © Gege Akutami

This is a fan project for educational purposes.
