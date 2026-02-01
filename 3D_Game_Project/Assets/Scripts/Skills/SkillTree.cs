using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Skill tree system with progression and upgrades
/// </summary>
public class SkillTree : MonoBehaviour
{
    [Header("Skill Configuration")]
    [SerializeField] private SkillNode[] skillNodes;
    [SerializeField] private float specialAbilityCooldown = 10f;

    private Dictionary<string, SkillNode> skillNodeMap = new Dictionary<string, SkillNode>();
    private Dictionary<string, bool> unlockedSkills = new Dictionary<string, bool>();
    private float specialAbilityTimer;
    private int skillPoints;

    // Combat multipliers from skill tree
    private float damageMultiplier = 1f;
    private float defenseMultiplier = 1f;
    private float staminaEfficiency = 1f;
    private float movementSpeedBonus = 1f;

    public event Action<SkillNode> OnSkillUnlocked;
    public event Action<int> OnSkillPointsChanged;

    private void Start()
    {
        InitializeSkillTree();
        specialAbilityTimer = specialAbilityCooldown;
        skillPoints = 0;
    }

    private void Update()
    {
        if (specialAbilityTimer < specialAbilityCooldown)
        {
            specialAbilityTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Initialize skill tree with all nodes
    /// </summary>
    private void InitializeSkillTree()
    {
        foreach (var node in skillNodes)
        {
            skillNodeMap[node.skillId] = node;
            unlockedSkills[node.skillId] = node.isStartingSkill;
        }
    }

    /// <summary>
    /// Attempt to unlock a skill
    /// </summary>
    public bool UnlockSkill(string skillId)
    {
        if (!skillNodeMap.ContainsKey(skillId))
        {
            Debug.LogWarning($"Skill not found: {skillId}");
            return false;
        }

        SkillNode node = skillNodeMap[skillId];

        // Check prerequisites
        foreach (var prerequisite in node.prerequisites)
        {
            if (!unlockedSkills.ContainsKey(prerequisite) || !unlockedSkills[prerequisite])
            {
                Debug.Log($"Prerequisite {prerequisite} not met for {skillId}");
                return false;
            }
        }

        // Check skill points
        if (skillPoints < node.skillPointCost)
        {
            Debug.Log($"Not enough skill points for {skillId}");
            return false;
        }

        unlockedSkills[skillId] = true;
        skillPoints -= node.skillPointCost;
        ApplySkillEffects(node);
        OnSkillUnlocked?.Invoke(node);
        OnSkillPointsChanged?.Invoke(skillPoints);

        return true;
    }

    /// <summary>
    /// Apply stat changes from unlocked skill
    /// </summary>
    private void ApplySkillEffects(SkillNode node)
    {
        damageMultiplier += node.damageBonus;
        defenseMultiplier += node.defenseBonus;
        staminaEfficiency += node.staminaEfficiency;
        movementSpeedBonus += node.movementSpeedBonus;
    }

    /// <summary>
    /// Use special ability if available
    /// </summary>
    public bool CanUseAbility() => specialAbilityTimer >= specialAbilityCooldown;

    /// <summary>
    /// Execute special ability
    /// </summary>
    public void UseAbility(Vector3 position, Vector3 direction)
    {
        if (!CanUseAbility())
            return;

        // Determine which ability based on unlocked skills
        if (IsSkillUnlocked("DivineWind"))
        {
            ExecuteDivineWind(position, direction);
        }
        else if (IsSkillUnlocked("SpatialDomain"))
        {
            ExecuteSpatialDomain(position);
        }
        else
        {
            ExecuteBasicAbility(position, direction);
        }

        specialAbilityTimer = 0;
    }

    /// <summary>
    /// Execute Divine Wind ability
    /// </summary>
    private void ExecuteDivineWind(Vector3 position, Vector3 direction)
    {
        // Create projectiles in an arc
        for (int i = -4; i <= 4; i++)
        {
            float angle = i * 10f;
            Vector3 spreadDirection = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            
            // Instantiate projectile
            // var projectile = Instantiate(projectilePrefab, position, Quaternion.identity);
            // projectile.Initialize(spreadDirection, 30f);
        }
    }

    /// <summary>
    /// Execute Spatial Domain ability
    /// </summary>
    private void ExecuteSpatialDomain(Vector3 position)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(position, 15f);

        foreach (Collider hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(50f, DamageType.Special);
            }
        }
    }

    /// <summary>
    /// Execute basic ability
    /// </summary>
    private void ExecuteBasicAbility(Vector3 position, Vector3 direction)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(position + direction * 5f, 5f);

        foreach (Collider hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(25f, DamageType.Physical);
            }
        }
    }

    /// <summary>
    /// Check if a skill is unlocked
    /// </summary>
    public bool IsSkillUnlocked(string skillId)
    {
        return unlockedSkills.ContainsKey(skillId) && unlockedSkills[skillId];
    }

    /// <summary>
    /// Get all unlocked skills
    /// </summary>
    public List<SkillNode> GetUnlockedSkills()
    {
        List<SkillNode> result = new List<SkillNode>();
        foreach (var skillId in unlockedSkills.Keys)
        {
            if (unlockedSkills[skillId])
            {
                result.Add(skillNodeMap[skillId]);
            }
        }
        return result;
    }

    /// <summary>
    /// Add skill points
    /// </summary>
    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    // Getters for stat multipliers
    public float GetDamageMultiplier() => damageMultiplier;
    public float GetDefenseMultiplier() => defenseMultiplier;
    public float GetStaminaEfficiency() => staminaEfficiency;
    public float GetMovementSpeedBonus() => movementSpeedBonus;
    public int GetSkillPoints() => skillPoints;
    public float GetAbilityCooldownRemaining() => Mathf.Max(0, specialAbilityCooldown - specialAbilityTimer);
}

/// <summary>
/// Individual skill node in the skill tree
/// </summary>
[System.Serializable]
public class SkillNode
{
    public string skillId;
    public string skillName;
    public string description;
    [TextArea(3, 5)]
    public string fullDescription;
    
    [Header("Progression")]
    public int skillPointCost = 1;
    public bool isStartingSkill = false;
    public string[] prerequisites;

    [Header("Effects")]
    public float damageBonus = 0f;
    public float defenseBonus = 0f;
    public float staminaEfficiency = 0f;
    public float movementSpeedBonus = 0f;
    
    [Header("Visual")]
    public Sprite skillIcon;
    public Color nodeColor = Color.white;

    [Header("Tree Position")]
    public SkillBranch branch;
    public int treeLevel;
}

public enum SkillBranch
{
    Combat,      // Enhanced attacks, combos
    Stealth,     // Silent movement, invisibility
    Defense,     // Dodging, blocking
    Utility      // Stamina efficiency, speed
}
