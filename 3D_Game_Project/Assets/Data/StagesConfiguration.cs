using UnityEngine;

/// <summary>
/// Stage configuration and data for all 8 stages
/// </summary>
[CreateAssetMenu(fileName = "StageData", menuName = "Game/Stage Data")]
public class StageData : ScriptableObject
{
    public Stage[] stages;
}

/// <summary>
/// Data for individual stages
/// </summary>
[System.Serializable]
public class StageConfig
{
    public int stageNumber;
    public string stageName;
    public string description;
    public string environmentPrefab;
    
    [Header("Spawn Data")]
    public Vector3 playerSpawnPoint;
    public EnemyWave[] enemyWaves;
    
    [Header("Objectives")]
    public string primaryObjective;
    public string secondaryObjective;
    public int targetCount;
    
    [Header("Rewards")]
    public int baseScoreReward = 1000;
    public int skillPointReward = 1;
    public string[] unlockedSkills;
}

/// <summary>
/// Wave of enemies for a stage
/// </summary>
[System.Serializable]
public class EnemyWave
{
    public string waveId;
    public EnemySpawnPoint[] enemies;
    public float spawnDelay = 0.5f;
    public bool waitForWaveCompletion = true;
}

/// <summary>
/// Individual enemy spawn point
/// </summary>
[System.Serializable]
public class EnemySpawnPoint
{
    public string characterType;
    public Vector3 spawnPosition;
    public int difficultyLevel = 1;
}

// Stage configurations as static data
public static class StagesConfiguration
{
    public static StageConfig[] GetAllStages()
    {
        return new[]
        {
            CreateStage1_TokyoJujutsuHigh(),
            CreateStage2_HiddenShrine(),
            CreateStage3_CursedGrounds(),
            CreateStage4_UndergroundTemple(),
            CreateStage5_ShiibuyaIncident(),
            CreateStage6_JujutsuSocietyHQ(),
            CreateStage7_CursedRealm(),
            CreateStage8_FinalConfrontation()
        };
    }

    private static StageConfig CreateStage1_TokyoJujutsuHigh()
    {
        return new StageConfig
        {
            stageNumber = 1,
            stageName = "Tokyo Jujutsu High",
            description = "Infiltrate the academy and eliminate low-level sorcerers",
            playerSpawnPoint = new Vector3(0, 1, 0),
            primaryObjective = "Eliminate all targets",
            secondaryObjective = "Complete without being detected",
            targetCount = 2,
            baseScoreReward = 1000,
            skillPointReward = 2,
            unlockedSkills = new[] { "BasicAttack", "Sprint" }
        };
    }

    private static StageConfig CreateStage2_HiddenShrine()
    {
        return new StageConfig
        {
            stageNumber = 2,
            stageName = "Hidden Shrine",
            description = "Face stronger opponents in an isolated shrine",
            playerSpawnPoint = new Vector3(-50, 1, 0),
            primaryObjective = "Eliminate all targets",
            secondaryObjective = "Destroy cursed artifacts",
            targetCount = 3,
            baseScoreReward = 1500,
            skillPointReward = 2,
            unlockedSkills = new[] { "PowerAttack", "DodgeRoll" }
        };
    }

    private static StageConfig CreateStage3_CursedGrounds()
    {
        return new StageConfig
        {
            stageNumber = 3,
            stageName = "Cursed Grounds",
            description = "Navigate through cursed terrain filled with enemies",
            playerSpawnPoint = new Vector3(50, 1, 0),
            primaryObjective = "Reach the target location",
            secondaryObjective = "Minimize damage taken",
            targetCount = 4,
            baseScoreReward = 2000,
            skillPointReward = 3,
            unlockedSkills = new[] { "CurseDetection", "ShadowStep" }
        };
    }

    private static StageConfig CreateStage4_UndergroundTemple()
    {
        return new StageConfig
        {
            stageNumber = 4,
            stageName = "Underground Temple",
            description = "Stealth-focused mission in a dark underground facility",
            playerSpawnPoint = new Vector3(0, -10, 0),
            primaryObjective = "Eliminate all targets silently",
            secondaryObjective = "Don't trigger alarms",
            targetCount = 3,
            baseScoreReward = 1800,
            skillPointReward = 2,
            unlockedSkills = new[] { "SilentAssassination", "InvisibilityCloak" }
        };
    }

    private static StageConfig CreateStage5_ShiibuyaIncident()
    {
        return new StageConfig
        {
            stageNumber = 5,
            stageName = "Shibuya Incident",
            description = "Dynamic environment with multiple objectives and obstacles",
            playerSpawnPoint = new Vector3(25, 1, 25),
            primaryObjective = "Complete all objectives",
            secondaryObjective = "Survive the chaos",
            targetCount = 5,
            baseScoreReward = 2500,
            skillPointReward = 3,
            unlockedSkills = new[] { "DivineWind", "TimeManipulation" }
        };
    }

    private static StageConfig CreateStage6_JujutsuSocietyHQ()
    {
        return new StageConfig
        {
            stageNumber = 6,
            stageName = "Jujutsu Society HQ",
            description = "High-security facility with elite defenders",
            playerSpawnPoint = new Vector3(-25, 1, -25),
            primaryObjective = "Reach the inner sanctum",
            secondaryObjective = "Neutralize security systems",
            targetCount = 6,
            baseScoreReward = 3000,
            skillPointReward = 4,
            unlockedSkills = new[] { "EnergyShield", "CombinedAttacks" }
        };
    }

    private static StageConfig CreateStage7_CursedRealm()
    {
        return new StageConfig
        {
            stageNumber = 7,
            stageName = "Cursed Realm",
            description = "Otherworldly dimension with powerful entities",
            playerSpawnPoint = new Vector3(0, 1, 50),
            primaryObjective = "Survive the cursed dimension",
            secondaryObjective = "Collect cursed energy fragments",
            targetCount = 4,
            baseScoreReward = 3500,
            skillPointReward = 4,
            unlockedSkills = new[] { "DimensionalShift", "SpatialDomain" }
        };
    }

    private static StageConfig CreateStage8_FinalConfrontation()
    {
        return new StageConfig
        {
            stageNumber = 8,
            stageName = "Final Confrontation",
            description = "Face Sukuna, the King of Curses himself!",
            playerSpawnPoint = new Vector3(0, 1, -50),
            primaryObjective = "Defeat Sukuna",
            secondaryObjective = "Survive the ultimate battle",
            targetCount = 1,
            baseScoreReward = 5000,
            skillPointReward = 5,
            unlockedSkills = new[] { "UltimateAbility" }
        };
    }
}
