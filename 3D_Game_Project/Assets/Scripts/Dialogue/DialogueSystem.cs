using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Complete dialogue system with branching conversations and character interactions
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private TextAsset[] dialogueDataFiles;

    private Dictionary<string, DialogueNode> dialogueDatabase = new Dictionary<string, DialogueNode>();
    private DialogueNode currentNode;
    private bool isInConversation = false;

    public event Action<DialogueNode> OnDialogueNodeChanged;
    public event Action OnConversationEnded;

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
        LoadDialogueData();
    }

    /// <summary>
    /// Load dialogue data from JSON files
    /// </summary>
    private void LoadDialogueData()
    {
        foreach (var file in dialogueDataFiles)
        {
            DialogueData data = JsonUtility.FromJson<DialogueData>(file.text);
            foreach (var node in data.nodes)
            {
                dialogueDatabase[node.id] = node;
            }
        }
    }

    /// <summary>
    /// Start a conversation with an NPC
    /// </summary>
    public void StartConversation(string conversationId, NPC npc)
    {
        if (!dialogueDatabase.ContainsKey(conversationId))
        {
            Debug.LogWarning($"Dialogue ID not found: {conversationId}");
            return;
        }

        isInConversation = true;
        currentNode = dialogueDatabase[conversationId];
        DisplayNode(currentNode);
        GameManager.Instance.TogglePause();
    }

    /// <summary>
    /// Display a dialogue node
    /// </summary>
    private void DisplayNode(DialogueNode node)
    {
        OnDialogueNodeChanged?.Invoke(node);
        dialogueUI.DisplayDialogue(node);
    }

    /// <summary>
    /// Select a dialogue choice
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (currentNode == null || choiceIndex >= currentNode.choices.Length)
            return;

        DialogueChoice choice = currentNode.choices[choiceIndex];

        // Execute any dialogue effects (rewards, conditions, etc.)
        ExecuteDialogueEffect(choice.effect);

        if (string.IsNullOrEmpty(choice.nextNodeId))
        {
            EndConversation();
        }
        else
        {
            if (dialogueDatabase.TryGetValue(choice.nextNodeId, out var nextNode))
            {
                currentNode = nextNode;
                DisplayNode(currentNode);
            }
            else
            {
                EndConversation();
            }
        }
    }

    /// <summary>
    /// Execute dialogue effects (skill rewards, item rewards, etc.)
    /// </summary>
    private void ExecuteDialogueEffect(DialogueEffect effect)
    {
        if (effect == null)
            return;

        switch (effect.effectType)
        {
            case DialogueEffectType.SkillReward:
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    SkillTree skillTree = player.GetComponent<SkillTree>();
                    if (skillTree != null)
                    {
                        skillTree.UnlockSkill(effect.skillId);
                    }
                }
                break;

            case DialogueEffectType.ItemReward:
                Inventory inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
                if (inventory != null)
                {
                    // inventory.AddItem(effect.itemId, effect.amount);
                }
                break;

            case DialogueEffectType.ScoreReward:
                GameManager.Instance.AddScore(effect.amount);
                break;

            case DialogueEffectType.TriggerEvent:
                // Handle custom events
                break;
        }
    }

    /// <summary>
    /// End the current conversation
    /// </summary>
    private void EndConversation()
    {
        isInConversation = false;
        currentNode = null;
        dialogueUI.HideDialogue();
        OnConversationEnded?.Invoke();
        GameManager.Instance.TogglePause();
    }

    public bool IsInConversation() => isInConversation;
}

/// <summary>
/// A single node in a dialogue tree
/// </summary>
[System.Serializable]
public class DialogueNode
{
    public string id;
    public string characterName;
    public string dialogue;
    public DialogueChoice[] choices;
    public string[] audioClips;
    public float displayDuration;
}

/// <summary>
/// A choice option in dialogue
/// </summary>
[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string nextNodeId;
    public DialogueEffect effect;
    public bool requiresCondition;
    public string conditionId;
}

/// <summary>
/// Effects that occur from dialogue choices
/// </summary>
[System.Serializable]
public class DialogueEffect
{
    public DialogueEffectType effectType;
    public string skillId;
    public string itemId;
    public int amount;
}

public enum DialogueEffectType
{
    None,
    SkillReward,
    ItemReward,
    ScoreReward,
    TriggerEvent,
    QuestUpdate
}

/// <summary>
/// Container for loaded dialogue data
/// </summary>
[System.Serializable]
public class DialogueData
{
    public DialogueNode[] nodes;
}

/// <summary>
/// NPC that can have conversations
/// </summary>
public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName;
    [SerializeField] private string conversationId;
    [SerializeField] private Animator animator;

    public void Interact(PlayerController player)
    {
        DialogueSystem.Instance.StartConversation(conversationId, this);
        
        if (animator != null)
        {
            animator.SetTrigger("Talk");
        }
    }

    public string GetNPCName() => npcName;
}
