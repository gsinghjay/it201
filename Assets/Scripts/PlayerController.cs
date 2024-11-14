using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    [Header("Movement")]
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float rotationSpeed = 720f;

    private Rigidbody rb;
    private float movementX;
    private float movementY;

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
    private PowerUpManager powerUpManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        respawnManager = FindObjectOfType<RespawnManager>();
        powerUpManager = GetComponent<PowerUpManager>();

        if (respawnManager == null)
        {
            Debug.LogError("PlayerController: RespawnManager not found in scene!");
        }
        if (animator == null)
        {
            Debug.LogError("PlayerController: Animator component not found!");
        }
        if (powerUpManager == null)
        {
            Debug.LogError("PlayerController: PowerUpManager component not found!");
        }

        InitializeGameState();
    }

    private void OnMove(InputValue movementValue)
    {
        if (isDead) return;

        Vector2 movementVector = movementValue.Get<Vector2>();

        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        if (!isGameActive || isDead) return;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                toRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.AddForce(movement * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

            float maxSpeed = speed;
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }

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

            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isGameActive || isDead) return;

        if (other.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            currentTime += PICKUP_TIME_BONUS;
            SetCountText();

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
        if (isDead) return;

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
            Debug.Log("PlayerController: No animator - disabling immediately");
            gameObject.SetActive(false);
            if (respawnManager != null)
            {
                respawnManager.HandleRespawn();
            }
        }
    }

    private IEnumerator DisableAfterDeathAnimation()
    {
        Debug.Log("Starting death animation");
        yield return new WaitForSeconds(1f);

        if (isDead && !isGameActive)
        {
            Debug.Log("Disabling player after death animation");
            gameObject.SetActive(false);

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

        gameObject.SetActive(true);

        InitializeGameState();
    }

    private void InitializeGameState()
    {
        Debug.Log("InitializeGameState called");
        isGameActive = true;
        isDead = false;
        SetCountText();
        winTextObject.SetActive(false);

        if (respawnManager != null)
        {
            transform.position = respawnManager.respawnPoint;
            transform.rotation = Quaternion.identity;
        }
    }

    private void SetCountText()
    {
        if (countText != null)
        {
            countText.text = $"Count: {count}";
        }

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

    private void HandleVictory()
    {
        isGameActive = false;
        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
        }

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

    private void Update()
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
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void GameOver()
    {
        isGameActive = false;
        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
            TextMeshProUGUI tmPro = winTextObject.GetComponent<TextMeshProUGUI>();
            if (tmPro != null)
            {
                tmPro.text = "Time's Up - Game Over!";
            }
        }
        HandleDeath();
    }
}