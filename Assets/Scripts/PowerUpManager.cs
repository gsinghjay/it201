using UnityEngine;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    [SerializeField]
    private float speedBoostMultiplier = 2f;
    [SerializeField]
    private float speedBoostDuration = 1f;

    [Header("Trail Settings")]
    [SerializeField]
    private Color trailColor = Color.blue;
    [SerializeField]
    private float trailWidth = 0.5f;

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
        if (playerController == null)
        {
            Debug.LogError("PowerUpManager: PlayerController component not found!");
            return;
        }

        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            Debug.LogError("PowerUpManager: TrailRenderer component not found!");
        }

        originalSpeed = playerController.Speed;
    }

    private void ConfigureTrailRenderer()
    {
        if (trailRenderer != null)
        {
            trailRenderer.startWidth = trailWidth;
            trailRenderer.endWidth = 0f;
            trailRenderer.time = 0.5f;
            trailRenderer.startColor = trailColor;
            trailRenderer.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
            trailRenderer.enabled = false;

            if (trailRenderer.material == null)
            {
                trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    public void ActivateSpeedBoost()
    {
        if (!isSpeedBoosted)
        {
            StartCoroutine(ApplySpeedBoost());
        }
        else
        {
            StopCoroutine(ApplySpeedBoost());
            StartCoroutine(ApplySpeedBoost());
        }
    }

    private IEnumerator ApplySpeedBoost()
    {
        isSpeedBoosted = true;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }
        playerController.Speed = originalSpeed * speedBoostMultiplier;

        yield return new WaitForSeconds(speedBoostDuration);

        playerController.Speed = originalSpeed;
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
        isSpeedBoosted = false;
    }
}