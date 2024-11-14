using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    private Vector3 offset;

    private void Awake()
    {
        if (playerTransform == null)
        {
            Debug.LogError("CameraController: Player Transform is not assigned.");
            return;
        }
        offset = transform.position - playerTransform.position;
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + offset;
        }
    }
}