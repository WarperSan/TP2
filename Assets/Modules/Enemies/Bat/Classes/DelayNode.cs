using BehaviourTree.Nodes;
using UnityEngine;

namespace EnemyModule
{
    public class Delay : Node
    {
        private float delayAmount;
        private float remaining;

        public Delay(float amount)
        {
            delayAmount = amount;
            remaining = amount;
        }

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            remaining -= Time.deltaTime;

            if (remaining <= 0)
            {
                remaining = delayAmount;
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
    }
}