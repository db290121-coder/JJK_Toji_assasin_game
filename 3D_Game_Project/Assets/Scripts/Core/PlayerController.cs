using UnityEngine;
using System;

/// <summary>
/// Controls player movement, combat, and interactions
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;

    [Header("Combat")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private AnimationController animationController;

    private float currentHealth;
    private float currentStamina;
    private float lastAttackTime;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool canAttack = true;

    private SkillTree skillTree;
    private Inventory inventory;

    // Events
    public event Action<float> OnHealthChanged;
    public event Action<float> OnStaminaChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        rb.drag = groundDrag;
        skillTree = GetComponent<SkillTree>();
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleCombat();
        RegenerateStamina();
        UpdateGroundState();
    }

    /// <summary>
    /// Handle player input
    /// </summary>
    private void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            AimWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSpecialAbility();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenSkillTree();
        }
    }

    /// <summary>
    /// Handle player movement
    /// </summary>
    private void HandleMovement()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= 20f * Time.deltaTime;
            animationController.SetSpeed(currentSpeed);
        }
        else
        {
            animationController.SetSpeed(moveSpeed);
        }

        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
        OnStaminaChanged?.Invoke(currentStamina);
    }

    /// <summary>
    /// Handle jumping
    /// </summary>
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        animationController.PlayJumpAnimation();
    }

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void UpdateGroundState()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerCollider.bounds.extents.y + 0.1f);
    }

    /// <summary>
    /// Handle combat attacks
    /// </summary>
    private void HandleCombat()
    {
        if (canAttack)
        {
            lastAttackTime += Time.deltaTime;
            if (lastAttackTime >= attackCooldown)
            {
                canAttack = true;
            }
        }
    }

    /// <summary>
    /// Perform melee or ranged attack
    /// </summary>
    public void Attack()
    {
        if (!canAttack || currentStamina < 15f)
            return;

        currentStamina -= 15f;
        canAttack = false;
        lastAttackTime = 0f;

        // Check if using ranged weapon
        if (inventory.GetCurrentWeapon() != null && inventory.GetCurrentWeapon().IsRanged)
        {
            PerformRangedAttack();
        }
        else
        {
            PerformMeleeAttack();
        }

        animationController.PlayAttackAnimation();
        OnStaminaChanged?.Invoke(currentStamina);
    }

    /// <summary>
    /// Perform melee attack
    /// </summary>
    private void PerformMeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 2f, 3f);

        foreach (Collider hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float damage = 25f;
                
                // Apply skill tree bonuses
                if (skillTree != null)
                {
                    damage *= skillTree.GetDamageMultiplier();
                }

                enemy.TakeDamage(damage, DamageType.Physical);
            }
        }
    }

    /// <summary>
    /// Perform ranged attack
    /// </summary>
    private void PerformRangedAttack()
    {
        Weapon weapon = inventory.GetCurrentWeapon();
        if (weapon != null && weapon.Ammo > 0)
        {
            weapon.Fire(transform.position + transform.forward, transform.forward);
            GameManager.Instance.AddScore(50);
        }
    }

    /// <summary>
    /// Aim the current weapon
    /// </summary>
    private void AimWeapon()
    {
        Weapon weapon = inventory.GetCurrentWeapon();
        if (weapon != null && weapon.IsRanged)
        {
            animationController.SetAiming(true);
            cameraController.SetAimMode(true);
        }
    }

    /// <summary>
    /// Use special ability
    /// </summary>
    private void UseSpecialAbility()
    {
        if (skillTree == null || !skillTree.CanUseAbility())
            return;

        skillTree.UseAbility(transform.position, transform.forward);
        animationController.PlaySpecialAbilityAnimation();
    }

    /// <summary>
    /// Interact with objects
    /// </summary>
    private void Interact()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider col in nearby)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }

    /// <summary>
    /// Open skill tree UI
    /// </summary>
    private void OpenSkillTree()
    {
        if (skillTree != null)
        {
            UIManager.Instance.ShowSkillTreeUI(skillTree);
        }
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        animationController.PlayHitAnimation();
    }

    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Regenerate stamina
    /// </summary>
    private void RegenerateStamina()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    /// <summary>
    /// Handle player death
    /// </summary>
    private void Die()
    {
        animationController.PlayDeathAnimation();
        GameManager.Instance.EndGame(false);
    }

    /// <summary>
    /// Reset player to spawn point
    /// </summary>
    public void ResetPlayer(Vector3 spawnPoint)
    {
        transform.position = spawnPoint;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        rb.velocity = Vector3.zero;

        OnHealthChanged?.Invoke(currentHealth);
        OnStaminaChanged?.Invoke(currentStamina);
    }

    // Getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public bool IsGrounded() => isGrounded;
}

/// <summary>
/// Damage type enum
/// </summary>
public enum DamageType
{
    Physical,
    Cursed,
    Special
}

/// <summary>
/// Interface for interactable objects
/// </summary>
public interface IInteractable
{
    void Interact(PlayerController player);
}
