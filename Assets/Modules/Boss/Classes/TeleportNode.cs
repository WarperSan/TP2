using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using ExtensionsModule;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class TeleportNode : Sequence
    {
        private readonly Fiddlesticks fiddlesticks;
        private readonly NavMeshAgent agent;
        private float delay;

        public TeleportNode(Fiddlesticks fiddlesticks, NavMeshAgent agent)
        {
            this.fiddlesticks = fiddlesticks;
            this.agent = agent;

            Attach(new CallbackNode(CanCheck));
            Attach(new CallbackNode(IsAlerted));
            Attach(new CallbackNode(FarEnough));
            Attach(new CallbackNode(Teleport));
        }

        private NodeState CanCheck()
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                delay = 5;
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
        private NodeState IsAlerted()
        {
            if (fiddlesticks.IsAlerted)
                return NodeState.SUCCESS;

            return NodeState.FAILURE;
        }
        private NodeState FarEnough()
        {
            Transform target = fiddlesticks.GetTarget();

            if (target == null)
                return NodeState.FAILURE;

            if (fiddlesticks.transform.Distance(target) > 15)
                return NodeState.SUCCESS;

            return NodeState.FAILURE;
        }
        private NodeState Teleport()
        {
            Transform target = fiddlesticks.GetTarget();

            if (target == null)
                return NodeState.FAILURE;

            if (!NavMesh.SamplePosition(target.position + target.forward, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return NodeState.FAILURE;

            agent.enabled = false;
            fiddlesticks.transform.position = hit.position;
            return NodeState.SUCCESS;
        }

        #region Node

        /// <inheritdoc/>
        public override string GetText() => "Teleport";

        #endregion
    }
}