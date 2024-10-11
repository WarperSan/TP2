using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using BehaviourTree.Trees;
using ExtensionsModule;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class TeleportCloserNode : Sequence
    {
        private readonly string TARGET;
        private readonly NavMeshAgent self;

        public TeleportCloserNode(NavMeshAgent self, string target)
        {
            this.TARGET = target;
            this.self = self;

            Attach(
                new CallbackNode(IsFarEnough)
            );

            Attach(
                new Delay(10)
            );

            Attach(
                new CallbackNode(TeleportCloser)
            );
        }

        private NodeState IsFarEnough()
        {
            Transform target = GetData<Transform>(TARGET);

            if (target == null)
                return NodeState.FAILURE;

            if (self.transform.Distance(target) > 20)
                return NodeState.SUCCESS;

            return NodeState.FAILURE;
        }

        private NodeState TeleportCloser()
        {
            Transform target = GetData<Transform>(TARGET);

            if (target == null)
                return NodeState.FAILURE;

            self.enabled = false;
            self.transform.position = self.transform.position + (target.position - self.transform.position).normalized * 5;
            self.enabled = true;

            return NodeState.SUCCESS;
        }

        #region Node

        public override string GetText() => "Teleport Closer";

        #endregion
    }
}