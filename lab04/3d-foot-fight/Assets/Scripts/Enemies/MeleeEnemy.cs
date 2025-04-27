using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class MeleeEnemy : MonoBehaviour
    {
        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        }
    }
}