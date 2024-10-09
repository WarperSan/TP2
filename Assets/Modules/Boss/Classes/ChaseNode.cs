using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using ExtensionsModule;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class ChaseNode : Sequence
    {
        private NavMeshAgent agent;
        private string target;

        private float delay;

        public ChaseNode(NavMeshAgent agent, string target)
        {
            this.agent = agent;
            this.target = target;
            delay = 5;

            // Stay behind the player
            // Freeze when player looks
            // Delay
            // Start chase

            Attach(
                new CallbackNode(() =>
                {
                    if (!agent.enabled)
                        return NodeState.FAILURE;

                    agent.destination = GetTargetPosition();

                    return NodeState.SUCCESS;
                }).Alias("Go To Target")
            );

            Attach(new CallbackNode(DecayTimer));

            Attach(
                new CallbackNode(() =>
                {
                    GameObject.FindAnyObjectByType<Fiddlesticks>().Kill();
                    return NodeState.SUCCESS;
                })
            );
        }

        private Vector3 GetTargetPosition()
        {
            Transform t = GetData<Transform>(target);

            if (t == null)
                return agent.transform.position;

            var position = t.position;

            // Get position behind the target
            if (Physics.Raycast(position, -t.forward, out var hit, 2))
            {
                position = hit.transform.position;
                position.y = t.position.y;
                return position;
            }

            return position - t.forward * 2;
        }

        private NodeState DecayTimer()
        {
            Transform t = GetData<Transform>(target);

            if (t == null)
                return NodeState.FAILURE;

            if (t.Distance(agent.transform) <= 2f)
                delay -= Time.deltaTime;
            else
                delay += Time.deltaTime;

            delay = Mathf.Clamp(delay, 0, 5);

            if (delay <= 0)
                return NodeState.SUCCESS;
            return NodeState.RUNNING;
        }

        #region Node

        /// <inheritdoc/>
        public override string GetText() => "Sneaky Chase";

        #endregion
    }
}