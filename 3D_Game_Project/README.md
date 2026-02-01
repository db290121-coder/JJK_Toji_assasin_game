# Toji: 3D Assassin Game
## A Complete 3D Implementation using Unity (C#)

### Project Overview
This is a complete 3D game built with C# for Unity, featuring Toji from Jujutsu Kaisen as an assassin eliminating targets across multiple stages.

### Features Included
- ✅ 3D Character Movement & Combat
- ✅ Advanced Dialogue System
- ✅ Skill Tree & Progression
- ✅ 8+ Stages with Unique Environments
- ✅ Enemy AI with Behavior Trees
- ✅ Weapon System & Customization
- ✅ Mission System with Rewards
- ✅ Dynamic Difficulty Scaling
- ✅ Audio Management
- ✅ Save/Load System

### Project Structure
```
3D_Game_Project/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   ├── PlayerController.cs
│   │   │   └── CameraController.cs
│   │   ├── Combat/
│   │   │   ├── CombatSystem.cs
│   │   │   ├── WeaponSystem.cs
│   │   │   └── DamageCalculator.cs
│   │   ├── AI/
│   │   │   ├── EnemyAI.cs
│   │   │   ├── BehaviorTree.cs
│   │   │   └── PatrolState.cs
│   │   ├── UI/
│   │   │   ├── UIManager.cs
│   │   │   ├── DialogueUI.cs
│   │   │   └── SkillTreeUI.cs
│   │   ├── Dialogue/
│   │   │   ├── DialogueSystem.cs
│   │   │   └── DialogueParser.cs
│   │   ├── Skills/
│   │   │   ├── SkillTree.cs
│   │   │   ├── Skill.cs
│   │   │   └── SkillNode.cs
│   │   └── Managers/
│   │       ├── AudioManager.cs
│   │       ├── SaveManager.cs
│   │       └── MissionManager.cs
│   ├── Data/
│   │   ├── Characters/
│   │   ├── Stages/
│   │   ├── Dialogue/
│   │   ├── Skills/
│   │   └── Missions/
│   ├── Scenes/
│   └── UI/
└── ProjectSettings/
```

### Installation & Setup

#### Requirements
- Unity 2021 LTS or later
- C# 9.0+
- Visual Studio / Rider IDE

#### Steps
1. Clone or download the project
2. Open with Unity Hub
3. Let Unity import all assets
4. Open `MainMenu` scene to start

### How to Play

**Objectives:**
- Eliminate all targets in each stage
- Use stealth when possible
- Manage stamina and health
- Unlock new skills to progress

**Controls:**
- **WASD** - Move
- **Mouse** - Look Around
- **Left Click** - Attack/Shoot
- **Right Click** - Aim
- **Q** - Special Ability
- **E** - Interact
- **Tab** - Skill Tree
- **Esc** - Pause Menu

### Game Progression

#### Stages
1. **Tokyo Jujutsu High** - Introduction & Training
2. **Hidden Shrine** - Mid-Tier Enemies
3. **Cursed Grounds** - Challenging Combat
4. **Underground Temple** - Stealth Focus
5. **Shibuya Incident** - Dynamic Environment
6. **Jujutsu Society HQ** - Defensive Scenario
7. **Cursed Realm** - Advanced Combat
8. **Final Confrontation** - Boss Battle

#### Skill Tree
- **Combat Branch**: Enhanced attacks, combos, critical strikes
- **Stealth Branch**: Silent movement, invisibility, detection avoidance
- **Defensive Branch**: Dodging, blocking, damage reduction
- **Utility Branch**: Stamina efficiency, movement speed, ability cooldown

### API Documentation

See individual script files for detailed API documentation and usage examples.

### Development Notes

This project uses:
- Dependency Injection for Manager classes
- Observer Pattern for UI updates
- State Machine for AI behavior
- Command Pattern for input handling
- Scriptable Objects for data configuration

### License
Jujutsu Kaisen is property of Gege Akutami. This is a fan project for educational purposes.

### Contributing
Feel free to extend this project with new features, stages, characters, or improvements!
