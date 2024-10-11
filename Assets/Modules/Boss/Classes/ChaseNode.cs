using System.Threading;
using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class ChaseNode : Sequence
    {
        private const float TIMER_SECONDS = 5f;
        private readonly Fiddlesticks fiddlesticks;
        private readonly NavMeshAgent agent;

        private float timer;
        private bool wasChasing;

        public ChaseNode(Fiddlesticks fiddlesticks, NavMeshAgent agent)
        {
            this.fiddlesticks = fiddlesticks;
            this.agent = agent;

            // Verify if chasing
            Attach(new CallbackNode(IsChasing));

            var chasing = new Parallel();

            // Chase timer
            chasing += new CallbackNode(ChaseTimer).Not();

            // Chase Target
            chasing += new CallbackNode(ChaseTarget);

            Attach(chasing.Alias("Chasing"));
        }

        private NodeState IsChasing()
        {
            bool isCurrentlyChasing = fiddlesticks.isChasing;

            // Still chasing
            if (isCurrentlyChasing && wasChasing)
                return NodeState.SUCCESS;

            // Start chasing
            if (isCurrentlyChasing && !wasChasing)
            {
                StartChase();
                return NodeState.SUCCESS;
            }

            // End chasing
            if (!isCurrentlyChasing && wasChasing)
            {
                EndChase();
                return NodeState.FAILURE;
            }

            return NodeState.FAILURE;
        }
        private NodeState ChaseTimer()
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, TIMER_SECONDS);

            if (timer <= 0)
            {
                EndChase();
                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
        private NodeState ChaseTarget()
        {
            // If agent disabled, skip
            if (!agent.enabled)
                return NodeState.FAILURE;

            var target = fiddlesticks.GetTarget();

            if (target == null)
                return NodeState.FAILURE;

            agent.SetDestination(target.position);

            return NodeState.RUNNING;
        }

        private void StartChase()
        {
            wasChasing = true;
            timer = TIMER_SECONDS;
            fiddlesticks.StartChase();
        }

        private void EndChase()
        {
            wasChasing = false;
            fiddlesticks.EndChase();
        }

        #region Node

        public override string GetText() => "Chase";

        #endregion
    }
}

