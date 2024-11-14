using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private NavMeshAgent agent;
    private bool isInitialized;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        isInitialized = agent != null && playerTransform != null;
    }

    private void Update()
    {
        if (!isInitialized) return;
        agent.SetDestination(playerTransform.position);
    }
}