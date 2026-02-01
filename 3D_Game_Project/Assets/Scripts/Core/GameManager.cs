using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Central game manager handling game state, stages, and overall game flow
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int CurrentStage { get; private set; } = 0;
    public int CurrentScore { get; private set; } = 0;
    public int TargetsEliminated { get; private set; } = 0;
    public bool IsGameRunning { get; private set; } = false;
    public bool IsGamePaused { get; private set; } = false;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;

    [Header("Game Configuration")]
    [SerializeField] private Stage[] stages;
    [SerializeField] private float stageTransitionDelay = 2f;

    private Stage currentStageData;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private GameState gameState = GameState.Menu;

    // Events
    public event Action<int> OnStageChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnTargetEliminated;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameOver;

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        StageClear,
        GameOver
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioManager.PlayBackgroundMusic("MainTheme");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Start a new game from the beginning
    /// </summary>
    public void StartNewGame()
    {
        CurrentStage = 0;
        CurrentScore = 0;
        TargetsEliminated = 0;
        IsGameRunning = true;
        gameState = GameState.Playing;
        LoadStage(0);
    }

    /// <summary>
    /// Load a specific stage
    /// </summary>
    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            EndGame(true);
            return;
        }

        CurrentStage = stageIndex;
        currentStageData = stages[stageIndex];
        activeEnemies.Clear();

        // Setup stage
        playerController.ResetPlayer(currentStageData.PlayerSpawnPoint);
        SpawnEnemies();

        OnStageChanged?.Invoke(CurrentStage);
        audioManager.PlaySound($"Stage{stageIndex + 1}Theme");

        uiManager.UpdateStageInfo(currentStageData.StageName, currentStageData.Description);
    }

    /// <summary>
    /// Spawn enemies for the current stage
    /// </summary>
    private void SpawnEnemies()
    {
        foreach (var enemyData in currentStageData.Enemies)
        {
            Enemy enemy = Instantiate(enemyData.EnemyPrefab, enemyData.SpawnPosition, Quaternion.identity);
            enemy.Initialize(enemyData);
            enemy.OnDefeated += HandleEnemyDefeated;
            activeEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// Handle enemy defeat
    /// </summary>
    private void HandleEnemyDefeated(Enemy defeatedEnemy)
    {
        activeEnemies.Remove(defeatedEnemy);
        TargetsEliminated++;
        CurrentScore += defeatedEnemy.RewardScore;

        OnTargetEliminated?.Invoke(TargetsEliminated);
        OnScoreChanged?.Invoke(CurrentScore);

        audioManager.PlaySound("TargetEliminated");

        // Check if stage is complete
        if (activeEnemies.Count == 0)
        {
            StageComplete();
        }
    }

    /// <summary>
    /// Called when all enemies in a stage are defeated
    /// </summary>
    private void StageComplete()
    {
        gameState = GameState.StageClear;
        CurrentScore += currentStageData.StageBonus;
        OnScoreChanged?.Invoke(CurrentScore);

        uiManager.ShowStageCompleteScreen(CurrentScore);

        if (CurrentStage < stages.Length - 1)
        {
            Invoke(nameof(NextStage), stageTransitionDelay);
        }
        else
        {
            EndGame(true);
        }
    }

    /// <summary>
    /// Move to the next stage
    /// </summary>
    private void NextStage()
    {
        LoadStage(CurrentStage + 1);
    }

    /// <summary>
    /// Toggle game pause state
    /// </summary>
    public void TogglePause()
    {
        if (gameState != GameState.Playing && gameState != GameState.Paused)
            return;

        IsGamePaused = !IsGamePaused;

        if (IsGamePaused)
        {
            gameState = GameState.Paused;
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
            uiManager.ShowPauseMenu();
            audioManager.PauseMusic();
        }
        else
        {
            gameState = GameState.Playing;
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
            uiManager.HidePauseMenu();
            audioManager.ResumeMusic();
        }
    }

    /// <summary>
    /// End the game
    /// </summary>
    public void EndGame(bool victory)
    {
        IsGameRunning = false;
        gameState = GameState.GameOver;
        Time.timeScale = 0f;

        OnGameOver?.Invoke();
        uiManager.ShowGameOverScreen(victory, CurrentScore, TargetsEliminated);
        audioManager.PlaySound(victory ? "Victory" : "Defeat");
    }

    /// <summary>
    /// Add score to the total
    /// </summary>
    public void AddScore(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>
    /// Get the current stage data
    /// </summary>
    public Stage GetCurrentStageData() => currentStageData;

    /// <summary>
    /// Get all active enemies in the stage
    /// </summary>
    public List<Enemy> GetActiveEnemies() => new List<Enemy>(activeEnemies);
}

/// <summary>
/// Data structure for a game stage
/// </summary>
[System.Serializable]
public class Stage
{
    public string StageName;
    public string Description;
    public Vector3 PlayerSpawnPoint;
    public EnemySpawnData[] Enemies;
    public int StageBonus = 1000;
}

/// <summary>
/// Data structure for enemy spawn data
/// </summary>
[System.Serializable]
public class EnemySpawnData
{
    public Enemy EnemyPrefab;
    public Vector3 SpawnPosition;
    public int Difficulty;
}
