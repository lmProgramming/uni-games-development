using LM;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Agents.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        protected NavMeshAgent Agent;
        protected Damageable Damageable;

        [Inject]
        public SoundManager SoundManager { get; protected set; }

        public virtual void GoTo(Target target)
        {
            Agent.SetDestination(target.CurrentPosition);
        }
    }
}