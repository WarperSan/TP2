using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using ExtensionsModule;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class HideNode : Selector
    {
        private NavMeshAgent agent;
        private Vector3 hideSpot;
        private string TARGET;
        private bool isHiding;
        private float hideTimer;

        public HideNode(NavMeshAgent agent, string target)
        {
            this.agent = agent;
            this.TARGET = target;
            isHiding = false;

            // If hiding, skip
            Attach(
                new CallbackNode(IsHiding).Alias("Is Hiding")
            );

            var hideSequence = new Sequence();

            // If last corners are visible from the player, find hiding spot
            hideSequence += new CallbackNode(CanHide).Alias("Can Hide");
            hideSequence += new CallbackNode(Hide).Alias("Hide");

            Attach(
                hideSequence.Alias("Hide Sequence")
            );
        }

        private NodeState IsHiding()
        {
            if (isHiding)
            {
                if (RaycastToTarget(agent.transform.position))
                {
                    isHiding = false;
                }
                else
                {
                    hideTimer -= Time.deltaTime * Mathf.Max(1, GetTarget().Distance(agent.transform) / 10f);

                    if (hideTimer <= 0)
                        isHiding = false;
                }
            }

            if (isHiding)
            {
                agent.destination = hideSpot;
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }

        private NodeState CanHide()
        {
            if (!agent.enabled)
                return NodeState.FAILURE;

            for (int i = 1; i < agent.path.corners.Length - 1; i++)
            {
                if (!RaycastToTarget(agent.path.corners[i]))
                    continue;
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }

        private NodeState Hide()
        {
            if (!agent.enabled)
                return NodeState.FAILURE;

            if (agent.path.corners.Length < 4)
                return NodeState.FAILURE;

            hideSpot = agent.path.corners[^4];
            isHiding = true;
            hideTimer = 10;
            return NodeState.RUNNING;
        }

        #region Getters

        private Transform GetTarget() => GetData<Transform>(TARGET);

        private bool RaycastToTarget(Vector3 from)
        {
            Transform target = GetTarget();

            if (target == null)
                return false;

            var position = target.position + new Vector3(0, 0.5f, 0);
            from.y = position.y;

            if (!Physics.Raycast(from, position - from, out var hit))
                return false;

            return hit.transform == target;
        }

        #endregion

        #region Node

        /// <inheritdoc/>
        public override string GetText() => "Hide";

        #endregion
    }
}