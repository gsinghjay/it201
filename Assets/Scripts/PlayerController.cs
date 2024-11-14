using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Controls player movement, interactions, and game state
/// </summary>
public class PlayerController : MonoBehaviour
{
       public float Speed
       {
              get { return speed; }
              set { speed = value; }
       }
    [Header("Movement")]
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    [SerializeField] private float speed = 10f;  // Changed from 0 to 10
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Animation")]
    private Animator animator;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Victory = Animator.StringToHash("Victory");

    [Header("Game State")]
    private int count;
    private bool isGameActive = true;
    private bool isDead = false;

    [Header("UI Elements")]
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public TextMeshProUGUI timerText;

    [Header("Level Elements")]
    public GameObject eastWall;
    public GameObject level2Enemy;

    [Header("Timer Settings")]
    private float currentTime = 15f;
    private const float PICKUP_TIME_BONUS = 15f;

    [Header("Components")]
    private RespawnManager respawnManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        respawnManager = FindObjectOfType<RespawnManager>();
        
        if (respawnManager == null)
        {
            Debug.LogError("RespawnManager not found in scene!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
        
        InitializeGameState();
    }

       void OnMove(InputValue movementValue)
       {
       if (isDead) return;
       
       // Get the input value as Vector2
       Vector2 movementVector = movementValue.Get<Vector2>();
       
       // Normalize the input to prevent faster diagonal movement
       if (movementVector.magnitude > 1f)
       {
              movementVector.Normalize();
       }
       
       // Store the movement values
       movementX = movementVector.x;
       movementY = movementVector.y;
       }

private void FixedUpdate()
{
    if (!isGameActive || isDead) return;
    
    Vector3 movement = new Vector3(movementX, 0.0f, movementY);
    
    if (movement.magnitude > 0.1f)
    {
        // Rotate the character using fixedDeltaTime
        Quaternion toRotation = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            toRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        // Move the character using fixedDeltaTime and ForceMode
        rb.AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        
        // Limit velocity for consistent movement speed
        float maxSpeed = speed * Time.fixedDeltaTime;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Animate
        if (animator != null)
        {
            animator.SetBool(IsMoving, true);
        }
    }
    else
    {
        if (animator != null)
        {
            animator.SetBool(IsMoving, false);
        }
        
        // Gradually slow down when not moving
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 5f);
    }
}

void OnTriggerEnter(Collider other)
{
    if (!isGameActive || isDead) return;

    if (other.gameObject.CompareTag("PickUp"))
    {
        other.gameObject.SetActive(false);
        count++;
        currentTime += PICKUP_TIME_BONUS;
        SetCountText();
        
        // Activate speed boost
        PowerUpManager powerUpManager = GetComponent<PowerUpManager>();
        if (powerUpManager != null)
        {
            powerUpManager.ActivateSpeedBoost();
        }
    }
}

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGameActive || isDead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleDeath();
        }
    }

public void HandleDeath()
{
    if (isDead) return; // Prevent multiple death calls
    
    Debug.Log("HandleDeath called");
    isDead = true;
    isGameActive = false;
    
    if (animator != null)
    {
        animator.SetTrigger(Die);
        StartCoroutine(DisableAfterDeathAnimation());
    }
    else
    {
        Debug.Log("No animator - disabling immediately");
        gameObject.SetActive(false);
        // Call respawn manager immediately if no animation
        if (respawnManager != null)
        {
            respawnManager.HandleRespawn();
        }
    }
}
private IEnumerator DisableAfterDeathAnimation()
{
    Debug.Log("Starting death animation");
    // Wait for animation to complete
    yield return new WaitForSeconds(1f); // Adjust based on death animation length
    
    // Only disable if we're still in death state (not respawning)
    if (isDead && !isGameActive)
    {
        Debug.Log("Disabling player after death animation");
        gameObject.SetActive(false);
        
        // Call respawn manager after player is disabled
        if (respawnManager != null)
        {
            respawnManager.HandleRespawn();
        }
    }
}


public void ResetPlayer()
{
    Debug.Log("ResetPlayer called");
    isDead = false;
    isGameActive = true;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    
    if (animator != null)
    {
        animator.SetBool(IsMoving, false);
    }
    
    // Make sure the GameObject is active
    gameObject.SetActive(true);
    
    // Reset game state without resetting count or timer
    InitializeGameState();
}

private void InitializeGameState()
{
    Debug.Log("InitializeGameState called");
    isGameActive = true;
    isDead = false;
    SetCountText();
    winTextObject.SetActive(false);
    
    // Reset position if needed
    if (respawnManager != null)
    {
        transform.position = respawnManager.respawnPoint;
        transform.rotation = Quaternion.identity;
    }
}

    private void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count == 4)
        {
            HandleLevelTransition();
        }

        if (count >= 12)
        {
            HandleVictory();
        }
    }

    private void HandleLevelTransition()
    {
        if (eastWall != null)
        {
            eastWall.SetActive(false);
            
            GameObject firstLevelEnemy = GameObject.FindGameObjectWithTag("Enemy");
            if (firstLevelEnemy != null)
            {
                Destroy(firstLevelEnemy);
            }

            if (level2Enemy != null)
            {
                level2Enemy.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Handles the victory condition
    /// </summary>
    private void HandleVictory()
    {
        isGameActive = false;
        winTextObject.SetActive(true);
        
        if (animator != null)
        {
            animator.SetTrigger(Victory);
        }
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    void Update()
    {
        if (isGameActive && !isDead)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentTime <= 0)
        {
            GameOver();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        isGameActive = false;
        winTextObject.gameObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "Time's Up - Game Over!";
        HandleDeath();
    }
}