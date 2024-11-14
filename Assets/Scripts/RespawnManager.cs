using System.Collections;
using UnityEngine;


/// Manages the respawn functionality for the player, handling both death and fall detection.

public class RespawnManager : MonoBehaviour
{
    
    /// Reference to the player GameObject.
    
    [SerializeField]
    private GameObject player;

[SerializeField] public Vector3 respawnPoint = Vector3.zero;

    
    /// The Y-axis threshold below which the player is considered fallen.
    
    [SerializeField]
    private float fallThreshold = -10f;

    
    /// Delay before the player is respawned after death or fall.
    
    [SerializeField]
    private float respawnDelay = 2f;

    
    /// Flag to indicate if the respawn process is currently active.
    
    private bool isRespawning = false;

    
    /// Reference to the PlayerController script.
    
    private PlayerController playerController;

    
    /// Reference to the Rigidbody component of the player.
    
    private Rigidbody playerRigidbody;

    
    /// Initializes the RespawnManager by setting up references.
    
    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("RespawnManager: Player reference is not set.");
            return;
        }

        respawnPoint = player.transform.position;

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("RespawnManager: PlayerController component not found on player.");
        }

        playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogError("RespawnManager: Rigidbody component not found on player.");
        }
    }

    
    /// Updates every frame to check if the player has fallen below the fall threshold.
    
    private void Update()
    {
        if (player == null || isRespawning) return;

        if (player.transform.position.y < fallThreshold)
        {
            HandleRespawn();
        }
    }

    
    /// Handles the respawn process by initiating the respawn sequence.
    
    public void HandleRespawn()
    {
        if (isRespawning) return;

        isRespawning = true;
        //playerController.HandleDeath(); // Assumes PlayerController has a HandleDeath method
        Invoke(nameof(RespawnPlayer), respawnDelay);
    }

    
    /// Respawns the player at the designated respawn point.
    
    private void RespawnPlayer()
    {
        if (player == null) return;

        player.transform.position = respawnPoint;
        player.transform.rotation = Quaternion.identity;

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        player.SetActive(true);
        playerController.ResetPlayer(); // Assumes PlayerController has a ResetPlayer method

        isRespawning = false;
    }

    
    /// Sets a new respawn point for the player.
    
    /// <param name="newRespawnPoint">The new position to respawn the player.</param>
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}