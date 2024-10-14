using BehaviourTree.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyModule
{
    public class GoToTarget : Node
    {
        private readonly NavMeshAgent agent;
        private readonly string target;

        public GoToTarget(NavMeshAgent agent, string target)
        {
            this.agent = agent;
            this.target = target;
        }

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            Transform t = GetData<Transform>(target);

            if (t == null)
                return NodeState.FAILURE;

            agent.destination = t.position;

            if (agent.remainingDistance <= agent.stoppingDistance)
                return NodeState.SUCCESS;

            return NodeState.RUNNING;
        }
    }
}