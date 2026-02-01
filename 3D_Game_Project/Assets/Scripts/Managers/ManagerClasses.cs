using UnityEngine;

/// <summary>
/// Camera controller for third-person perspective
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float followDistance = 5f;
    [SerializeField] private float followHeight = 2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float smoothDamp = 0.1f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool aimMode = false;

    private void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    /// <summary>
    /// Handle camera input (mouse look)
    /// </summary>
    private void HandleInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        player.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    /// <summary>
    /// Update camera position based on player
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 targetPos = player.position + player.up * followHeight - player.forward * followDistance;
        
        if (aimMode)
        {
            targetPos = player.position + player.up * (followHeight * 0.7f) - player.forward * (followDistance * 0.5f);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothDamp);
        transform.LookAt(player.position + player.up * (followHeight * 0.5f));
    }

    /// <summary>
    /// Set aim mode
    /// </summary>
    public void SetAimMode(bool isAiming)
    {
        aimMode = isAiming;
    }
}

/// <summary>
/// Animation controller for player
/// </summary>
public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    public void PlayHitAnimation()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }

    public void PlaySpecialAbilityAnimation()
    {
        animator.SetTrigger("SpecialAbility");
    }

    public void SetAiming(bool isAiming)
    {
        animator.SetBool("Aiming", isAiming);
    }
}

/// <summary>
/// Audio Manager for music and sound effects
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip[] soundEffects;

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

    public void PlayBackgroundMusic(string musicName)
    {
        foreach (var clip in backgroundMusic)
        {
            if (clip.name == musicName)
            {
                musicSource.clip = clip;
                musicSource.Play();
                return;
            }
        }
    }

    public void PlaySound(string soundName)
    {
        foreach (var clip in soundEffects)
        {
            if (clip.name == soundName)
            {
                sfxSource.PlayOneShot(clip);
                return;
            }
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}

/// <summary>
/// Save/Load Manager for game data persistence
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [System.Serializable]
    private class GameSaveData
    {
        public int currentStage;
        public int totalScore;
        public int skillPoints;
        public string[] unlockedSkills;
        public float playerHealth;
        public Vector3 playerPosition;
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

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            currentStage = GameManager.Instance.CurrentStage,
            totalScore = GameManager.Instance.CurrentScore,
            skillPoints = GetComponent<SkillTree>()?.GetSkillPoints() ?? 0
        };

        string json = JsonUtility.ToJson(saveData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/gamesave.json", json);
    }

    public void LoadGame()
    {
        string savePath = Application.persistentDataPath + "/gamesave.json";
        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            // Apply loaded data
        }
    }
}

/// <summary>
/// Weapon system for ranged attacks
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField] private bool isRanged = false;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private GameObject projectilePrefab;

    private int currentAmmo;

    public bool IsRanged => isRanged;
    public int Ammo => currentAmmo;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        if (currentAmmo > 0 && projectilePrefab != null)
        {
            Instantiate(projectilePrefab, firePosition, Quaternion.LookRotation(fireDirection));
            currentAmmo--;
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
    }
}

/// <summary>
/// Inventory system
/// </summary>
public class Inventory : MonoBehaviour
{
    private Weapon currentWeapon;
    private System.Collections.Generic.Dictionary<string, int> items = new System.Collections.Generic.Dictionary<string, int>();

    public void EquipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void AddItem(string itemId, int amount)
    {
        if (items.ContainsKey(itemId))
        {
            items[itemId] += amount;
        }
        else
        {
            items[itemId] = amount;
        }
    }

    public int GetItemCount(string itemId)
    {
        return items.ContainsKey(itemId) ? items[itemId] : 0;
    }
}
