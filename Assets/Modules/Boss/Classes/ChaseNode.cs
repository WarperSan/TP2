using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class ChaseNode : Sequence
    {
        private readonly Fiddlesticks fiddlesticks;
        private readonly NavMeshAgent agent;
        private const float TIMER_CHASE = 5f;
        private float timer;
        private bool wasChasing;

        public ChaseNode(Fiddlesticks fiddlesticks, NavMeshAgent agent)
        {
            this.fiddlesticks = fiddlesticks;
            this.agent = agent;

            // Verify if chasing
            Attach(new CallbackNode(IsChasing));

            // Chase Target
            Attach(new CallbackNode(ChaseTarget));
        }

        private NodeState IsChasing()
        {
            bool isCurrentlyChasing = fiddlesticks.isChasing;

            if(isCurrentlyChasing && wasChasing)
                return NodeState.SUCCESS;
           
            return NodeState.FAILURE;
        }

        private NodeState ChaseTarget()
        {
            if (!agent.enabled)
                return NodeState.FAILURE;

            var target = fiddlesticks.GetTarget();

            if (target == null)
                return NodeState.FAILURE;

            agent.SetDestination(target.position);

            return NodeState.RUNNING;
        }

        #region Node

        public override string GetText() => "Chase";

        #endregion
    }
}

