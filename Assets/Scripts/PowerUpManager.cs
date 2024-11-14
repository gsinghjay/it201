using UnityEngine;
using System.Collections;

/// <summary>
/// Manages temporary power-ups and their effects on the player
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    [SerializeField] private float speedBoostMultiplier = 2f;
    [SerializeField] private float speedBoostDuration = 1f;

    [Header("Trail Settings")]
    [SerializeField] private Color trailColor = Color.blue;
    [SerializeField] private float trailWidth = 0.5f;

    private PlayerController playerController;
    private TrailRenderer trailRenderer;
    private float originalSpeed;
    private bool isSpeedBoosted = false;

    private void Start()
    {
        SetupComponents();
        ConfigureTrailRenderer();
    }

    private void SetupComponents()
    {
        playerController = GetComponent<PlayerController>();
        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        
        if (playerController == null)
        {
            Debug.LogError("PowerUpManager requires a PlayerController component!");
        }
        originalSpeed = playerController.Speed;
    }

    private void ConfigureTrailRenderer()
    {
        if (trailRenderer != null)
        {
            trailRenderer.startWidth = trailWidth;
            trailRenderer.endWidth = 0f;
            trailRenderer.time = 0.5f; // How long the trail remains visible
            trailRenderer.startColor = trailColor;
            trailRenderer.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
            trailRenderer.enabled = false; // Start with trail disabled
            
            // Create a material for the trail
            Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
            trailRenderer.material = trailMaterial;
        }
    }

    /// <summary>
    /// Activates speed boost power-up
    /// </summary>
    public void ActivateSpeedBoost()
    {
        if (!isSpeedBoosted)
        {
            StartCoroutine(ApplySpeedBoost());
        }
        else
        {
            // Reset the current boost coroutine
            StopAllCoroutines();
            StartCoroutine(ApplySpeedBoost());
        }
    }

    private IEnumerator ApplySpeedBoost()
    {
        isSpeedBoosted = true;
        
        // Enable trail and apply speed boost
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }
        playerController.Speed = originalSpeed * speedBoostMultiplier;

        // Wait for duration
        yield return new WaitForSeconds(speedBoostDuration);

        // Reset speed and disable trail
        playerController.Speed = originalSpeed;
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
        isSpeedBoosted = false;
    }
}