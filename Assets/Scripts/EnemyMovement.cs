using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    [Range(0.5f, 2f)]
    private float updateInterval = 1f;

    private NavMeshAgent agent;
    private bool isInitialized;
    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("EnemyMovement: Player Object not found.");
            }
        }
        isInitialized = agent != null && playerTransform != null;

        if (isInitialized)
        {
            lastPlayerPosition = playerTransform.position;
            StartCoroutine(UpdateDestinationRoutine());
        }
    }

    private IEnumerator UpdateDestinationRoutine()
    {
        while (isInitialized)
        {
            if (Vector3.Distance(playerTransform.position, lastPlayerPosition) > 0.5f)
            {
                agent.SetDestination(playerTransform.position);
                lastPlayerPosition = playerTransform.position;
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }
}