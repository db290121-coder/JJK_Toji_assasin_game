# Setup Guide: Toji 3D Assassin Game

## Quick Start

### Prerequisites
- **Unity 2021 LTS** or newer
- **Visual Studio 2019+** or **Rider** (optional but recommended)
- **TextMesh Pro** (usually included with Unity)
- 4GB+ RAM
- 2GB free disk space

### Installation Steps

#### 1. Create New Unity Project
```
1. Open Unity Hub
2. Click "New Project"
3. Select "3D (URP)" or "3D (Built-in Render Pipeline)"
4. Name: "Toji_3D_Game"
5. Create
```

#### 2. Import Project Files
```
1. Copy all files from 3D_Game_Project/Assets to your Unity project's Assets folder
2. Let Unity reimport all assets
3. Open Window > TextMeshPro > Import TMP Essential Resources
```

#### 3. Scene Setup

**Main Scene Structure:**
```
Scene Hierarchy:
├── GameManager (Prefab)
├── Player
│   ├── Model (3D Model)
│   ├── Camera
│   └── Weapon
├── Environment
│   ├── Ground
│   ├── Obstacles
│   └── Props
├── Enemies
│   └── (Spawned at runtime)
├── UI Canvas
│   ├── MainMenuPanel
│   ├── GameplayHUDPanel
│   ├── DialoguePanel
│   ├── SkillTreePanel
│   ├── PauseMenuPanel
│   └── GameOverPanel
└── Audio
    ├── MusicSource
    └── SFXSource
```

### Component Setup

#### Player GameObject
Attach these scripts:
- `PlayerController.cs`
- `SkillTree.cs`
- `Inventory.cs`
- `AnimationController.cs`
- `Rigidbody` (IsKinematic: false)
- `CapsuleCollider`

#### GameManager (Singleton)
Attach to any GameObject:
- `GameManager.cs`

#### UI Manager
Attach to Canvas:
- `UIManager.cs`

#### Audio Manager
Create empty GameObject:
- `AudioManager.cs`
- Two `AudioSource` components

#### Enemy Prefabs
Each enemy needs:
- `Enemy.cs` (inherits behavior)
- `Animator`
- `NavMeshAgent` or `Rigidbody`
- Collider
- Model with animations

### Configuration

#### 1. Player Stats
Edit in PlayerController Inspector:
```
- Move Speed: 5
- Sprint Speed: 8
- Jump Force: 5
- Max Health: 100
- Max Stamina: 100
- Stamina Regen Rate: 10
- Attack Cooldown: 0.5
```

#### 2. Stages Configuration
Stages are defined in `StagesConfiguration.cs`:
- 8 unique stages
- Customizable difficulty
- Enemy spawn points
- Reward systems

#### 3. Skill Tree
Configure in Inspector:
- Create `SkillNode` instances
- Set up skill dependencies
- Configure stat bonuses
- Unlock conditions

#### 4. Dialogue System
Edit `DialogueData.json`:
```json
{
  "nodes": [
    {
      "id": "unique_id",
      "characterName": "NPC Name",
      "dialogue": "What they say",
      "choices": [...]
    }
  ]
}
```

### Build & Run

#### In Editor
1. Open any Scene
2. Press Play (or Ctrl+P)
3. Use controls to test

#### Build for Windows
```
File > Build Settings
- Select Windows Standalone
- Click Build
- Choose output folder
- Run the .exe file
```

#### Build for Mac
```
File > Build Settings
- Select macOS
- Click Build
- Follow prompts
- Run the .app file
```

### Extending the Project

#### Add New Stages
Edit `StagesConfiguration.cs`:
```csharp
private static StageConfig CreateStage9_CustomStage()
{
    return new StageConfig
    {
        stageNumber = 9,
        stageName = "Custom Stage",
        // ... configure
    };
}
```

#### Add New Skills
1. Create `SkillNode` in editor
2. Configure in `SkillTree.cs`
3. Add effects to `ExecuteAbility()` method

#### Add New Enemies
1. Create enemy prefab
2. Attach `Enemy.cs` script
3. Configure stats
4. Add to stage spawn data

#### Add Dialogue
1. Add to `DialogueData.json`
2. Create NPC GameObject
3. Attach `NPC.cs` script
4. Set conversation ID

### Troubleshooting

**Problem: Missing scripts**
- Solution: Reimport assets, check namespace declarations

**Problem: No audio**
- Solution: Ensure audio files in Resources/Audio, check AudioManager setup

**Problem: UI not showing**
- Solution: Check Canvas scale, verify UI references in UIManager

**Problem: Enemies not spawning**
- Solution: Check NavMesh bake, verify spawn points not in obstacles

**Problem: Game crashes**
- Solution: Check console for errors, verify all prefab references assigned

### Performance Tips

1. **Use Object Pooling** for projectiles and effects
2. **Optimize Animations** using animation layers
3. **Cull Distant Objects** using LOD groups
4. **Use SimpleShader** on less important objects
5. **Profile** with Unity Profiler (Window > Analysis > Profiler)

### Version Control

Recommended `.gitignore`:
```
Library/
Logs/
Temp/
.vs/
*.csproj
*.sln
obj/
*.userprefs
```

### Next Steps

1. **Import 3D Models** - Get character and environment models
2. **Add Animations** - Create or import attack/movement animations
3. **Polish Effects** - Add particle effects and visual feedback
4. **Balance Gameplay** - Adjust difficulty and enemy stats
5. **Add Sound Design** - Record or find sound effects
6. **Beta Test** - Play through all stages

### Resources

- Unity Documentation: https://docs.unity3d.com
- C# Programming Guide: https://docs.microsoft.com/en-us/dotnet/csharp/
- Jujutsu Kaisen Wiki: https://jujutsu-kaisen.fandom.com
- Asset Stores: Unity Asset Store, TurboSquid, Sketchfab

### Support

For issues or questions:
1. Check console errors (Ctrl+Shift+C)
2. Review scripts documentation
3. Check sample scenes
4. Review GitHub issues if applicable

Enjoy developing your game!
