using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Manager for all game screens and overlays
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayHUDPanel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject skillTreePanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TMPro.TextMeshProUGUI stageNameText;
    [SerializeField] private TMPro.TextMeshProUGUI stageDescriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI staminaText;

    private Dictionary<string, GameObject> uiPanels = new Dictionary<string, GameObject>();
    private SkillTreeUI skillTreeUI;
    private DialogueUI dialogueUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeUI();
    }

    /// <summary>
    /// Initialize all UI elements
    /// </summary>
    private void InitializeUI()
    {
        uiPanels["MainMenu"] = mainMenuPanel;
        uiPanels["GameplayHUD"] = gameplayHUDPanel;
        uiPanels["Dialogue"] = dialoguePanel;
        uiPanels["SkillTree"] = skillTreePanel;
        uiPanels["PauseMenu"] = pauseMenuPanel;
        uiPanels["GameOver"] = gameOverPanel;

        // Hide all except main menu
        HideAllPanels();
        ShowPanel("MainMenu");

        skillTreeUI = skillTreePanel.GetComponent<SkillTreeUI>();
        dialogueUI = dialoguePanel.GetComponent<DialogueUI>();
    }

    /// <summary>
    /// Show a specific UI panel
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (uiPanels.ContainsKey(panelName))
        {
            uiPanels[panelName].SetActive(true);
        }
    }

    /// <summary>
    /// Hide a specific UI panel
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (uiPanels.ContainsKey(panelName))
        {
            uiPanels[panelName].SetActive(false);
        }
    }

    /// <summary>
    /// Hide all UI panels
    /// </summary>
    private void HideAllPanels()
    {
        foreach (var panel in uiPanels.Values)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// Update stage information display
    /// </summary>
    public void UpdateStageInfo(string stageName, string description)
    {
        if (stageNameText != null) stageNameText.text = stageName;
        if (stageDescriptionText != null) stageDescriptionText.text = description;
    }

    /// <summary>
    /// Update HUD stats
    /// </summary>
    public void UpdateHUDStats(int score, float health, float stamina)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (healthText != null) healthText.text = $"HP: {Mathf.CeilToInt(health)}";
        if (staminaText != null) staminaText.text = $"Stamina: {Mathf.CeilToInt(stamina)}";
    }

    /// <summary>
    /// Show pause menu
    /// </summary>
    public void ShowPauseMenu()
    {
        ShowPanel("PauseMenu");
    }

    /// <summary>
    /// Hide pause menu
    /// </summary>
    public void HidePauseMenu()
    {
        HidePanel("PauseMenu");
    }

    /// <summary>
    /// Show stage complete screen
    /// </summary>
    public void ShowStageCompleteScreen(int score)
    {
        // Show custom stage complete message
    }

    /// <summary>
    /// Show game over screen
    /// </summary>
    public void ShowGameOverScreen(bool victory, int score, int targets)
    {
        ShowPanel("GameOver");
        
        TMPro.TextMeshProUGUI titleText = gameOverPanel.transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI statsText = gameOverPanel.transform.Find("Stats").GetComponent<TMPro.TextMeshProUGUI>();

        titleText.text = victory ? "MISSION COMPLETE!" : "MISSION FAILED";
        statsText.text = $"Final Score: {score}\nTargets Eliminated: {targets}";
    }

    /// <summary>
    /// Show skill tree UI
    /// </summary>
    public void ShowSkillTreeUI(SkillTree skillTree)
    {
        ShowPanel("SkillTree");
        if (skillTreeUI != null)
        {
            skillTreeUI.DisplaySkillTree(skillTree);
        }
    }

    /// <summary>
    /// Show dialogue UI
    /// </summary>
    public void ShowDialogueUI()
    {
        ShowPanel("Dialogue");
    }

    /// <summary>
    /// Hide dialogue UI
    /// </summary>
    public void HideDialogueUI()
    {
        HidePanel("Dialogue");
    }

    /// <summary>
    /// Show main menu
    /// </summary>
    public void ShowMainMenu()
    {
        Time.timeScale = 1f;
        HideAllPanels();
        ShowPanel("MainMenu");
    }

    /// <summary>
    /// Start game from UI
    /// </summary>
    public void StartGameButton()
    {
        HideAllPanels();
        ShowPanel("GameplayHUD");
        GameManager.Instance.StartNewGame();
    }

    /// <summary>
    /// Quit game from UI
    /// </summary>
    public void QuitGameButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

/// <summary>
/// Skill Tree UI Display
/// </summary>
public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private Transform skillNodesContainer;
    [SerializeField] private GameObject skillNodePrefab;

    public void DisplaySkillTree(SkillTree skillTree)
    {
        // Clear existing nodes
        foreach (Transform child in skillNodesContainer)
        {
            Destroy(child.gameObject);
        }

        // Display skill tree nodes
        var unlockedSkills = skillTree.GetUnlockedSkills();
        // Create UI for each skill node
    }
}

/// <summary>
/// Dialogue UI Display
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choicePrefab;

    public void DisplayDialogue(DialogueNode node)
    {
        characterNameText.text = node.characterName;
        dialogueText.text = node.dialogue;

        // Clear previous choices
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        // Display choices
        foreach (var choice in node.choices)
        {
            var choiceButton = Instantiate(choicePrefab, choicesContainer);
            var buttonText = choiceButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            buttonText.text = choice.text;
        }
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }
}
