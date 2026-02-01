using UnityEngine;
using System;

/// <summary>
/// Base enemy class with AI and combat behavior
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] public int RewardScore = 500;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent navAgent;
    
    private float currentHealth;
    private PlayerController player;
    private float attackCooldown;
    private EnemyState currentState = EnemyState.Idle;
    private float stateTimer;

    public event Action<Enemy> OnDefeated;
    public event Action<float> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        
        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // State machine
        switch (currentState)
        {
            case EnemyState.Idle:
                if (distToPlayer < detectionRange)
                {
                    SetState(EnemyState.Chase);
                }
                break;

            case EnemyState.Chase:
                if (distToPlayer < attackRange)
                {
                    SetState(EnemyState.Attack);
                }
                else if (distToPlayer > detectionRange * 1.5f)
                {
                    SetState(EnemyState.Idle);
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case EnemyState.Attack:
                if (distToPlayer > attackRange)
                {
                    SetState(EnemyState.Chase);
                }
                else
                {
                    AttackPlayer();
                }
                break;

            case EnemyState.Stunned:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    SetState(EnemyState.Chase);
                }
                break;
        }

        attackCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Initialize enemy with spawn data
    /// </summary>
    public void Initialize(EnemySpawnData data)
    {
        maxHealth = Random.Range(data.Difficulty * 30, data.Difficulty * 50);
        currentHealth = maxHealth;
        damage = data.Difficulty * 5f;
        detectionRange = 20f;
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    private void ChasePlayer()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(player.transform.position);
        }
        else if (rb != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    /// <summary>
    /// Attack the player
    /// </summary>
    private void AttackPlayer()
    {
        if (navAgent != null)
        {
            navAgent.velocity = Vector3.zero;
        }

        stateTimer -= Time.deltaTime;

        if (attackCooldown <= 0)
        {
            // Face player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);

            player.TakeDamage(damage);
            attackCooldown = 2f;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    /// <summary>
    /// Set enemy state
    /// </summary>
    private void SetState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        stateTimer = 0;

        if (animator != null)
        {
            animator.SetInteger("State", (int)newState);
        }
    }

    /// <summary>
    /// Take damage
    /// </summary>
    public void TakeDamage(float damageAmount, DamageType damageType)
    {
        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Enemy death
    /// </summary>
    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Drop items/rewards
        DropRewards();

        OnDefeated?.Invoke(this);
        
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// Drop rewards on death
    /// </summary>
    private void DropRewards()
    {
        // Instantiate reward prefabs at death location
    }

    /// <summary>
    /// Stun the enemy
    /// </summary>
    public void Stun(float duration)
    {
        SetState(EnemyState.Stunned);
        stateTimer = duration;
        
        if (navAgent != null)
        {
            navAgent.velocity = Vector3.zero;
        }
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}

/// <summary>
/// Enemy AI states
/// </summary>
public enum EnemyState
{
    Idle = 0,
    Chase = 1,
    Attack = 2,
    Stunned = 3,
    Dead = 4
}
