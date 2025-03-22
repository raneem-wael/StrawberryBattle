using UnityEngine;

using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    //[SerializeField] private Transform _target;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
        //agent.SetDestination(_target.transform.position);
    }

   
    public void MoveTo(Vector3 position)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            Debug.Log($"Moving player to {position}");
            agent.SetDestination(position);
        }
        else
        {
            Debug.LogError("Player is not on a NavMesh or NavMeshAgent is missing!");
        }
    }
}

