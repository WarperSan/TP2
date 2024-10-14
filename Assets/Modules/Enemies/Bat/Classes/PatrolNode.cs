using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyModule
{
    public class PatrolNode : Sequence
    {
        private readonly Transform[] targets;
        private int currentIndex;

        const string CURRENT_TARGET = "currentTarget";

        public PatrolNode(NavMeshAgent agent, Transform[] targets)
        {
            // Aller � la prochaine destination
            Attach(
                new GoToTarget(agent, CURRENT_TARGET)
            );

            // Attendre un certain temps
            Attach(new Delay(5));

            // Changer la destination
            Attach(new CallbackNode(ChangeTarget));

            // Set data
            SetData(CURRENT_TARGET, targets[0]);
            this.targets = targets;
        }

        private NodeState ChangeTarget()
        {
            currentIndex++;

            if (currentIndex >= targets.Length)
                currentIndex = 0;

            SetData(CURRENT_TARGET, targets[currentIndex]);

            return NodeState.SUCCESS;
        }

        /// <inheritdoc/>
        public override string GetText() => "Patrol";
    }
}