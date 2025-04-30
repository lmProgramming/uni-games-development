using UnityEngine;
using UnityEngine.AI;

namespace Agents.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        protected NavMeshAgent Agent;
        protected Damageable Damageable;

        public virtual void GoTo(Target target)
        {
            Agent.SetDestination(target.CurrentPosition);
        }
    }
}