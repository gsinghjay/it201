using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    public Vector3 respawnPoint = Vector3.zero;

    [SerializeField]
    private float fallThreshold = -10f;

    [SerializeField]
    private float respawnDelay = 2f;

    private bool isRespawning = false;

    private PlayerController playerController;
    private Rigidbody playerRigidbody;

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

    private void Update()
    {
        if (player == null || isRespawning) return;

        if (player.transform.position.y < fallThreshold)
        {
            HandleRespawn();
        }
    }

    public void HandleRespawn()
    {
        if (isRespawning) return;

        isRespawning = true;
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (player == null) yield break;

        player.transform.position = respawnPoint;
        player.transform.rotation = Quaternion.identity;

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        player.SetActive(true);
        if (playerController != null)
        {
            playerController.ResetPlayer();
        }

        isRespawning = false;
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
}