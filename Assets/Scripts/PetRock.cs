using UnityEngine;
using UnityEngine.AI;

public class PetRock : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float stopDistance = 1f;
    
    private NavMeshAgent agent;
    private bool isInitialized;
    private Vector3 targetPosition;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        if (agent != null)
        {
            agent.stoppingDistance = stopDistance;
            agent.speed = 3.5f;
            agent.acceleration = 8f;
        }
        
        isInitialized = agent != null && playerTransform != null;
    }
    
    private void Update()
    {
        if (!isInitialized) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer > followDistance)
        {
            Vector3 directionToPlayer = (transform.position - playerTransform.position).normalized;
            targetPosition = playerTransform.position + (directionToPlayer * followDistance);
            agent.SetDestination(targetPosition);
        }
    }
}