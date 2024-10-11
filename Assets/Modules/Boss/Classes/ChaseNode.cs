using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    public class ChaseNode : Sequence
    {
        private const float TIMER_SECONDS = 5;

        private readonly Fiddlesticks fiddlesticks;
        private readonly NavMeshAgent agent;

        private float timer;

        public ChaseNode(Fiddlesticks fiddlesticks, NavMeshAgent agent)
        {
            this.fiddlesticks = fiddlesticks;
            this.agent = agent;
            this.timer = TIMER_SECONDS;

            // Walk behind the player
            Attach(new CallbackNode(GetBehindTarget));

            // Wait X seconds
            Attach(new CallbackNode(KillTimer));

            // Kill the player
            Attach(new CallbackNode(KillTarget));
        }

        private NodeState GetBehindTarget()
        {
            // If agent disabled, skip
            if (!agent.enabled)
                return NodeState.FAILURE;

            var behind = GetBehindPosition();

            if (behind.HasValue)
                agent.SetDestination(behind.Value);

            // If doesn't have a path, skip
            if (!agent.hasPath)
                return NodeState.FAILURE;

            // If can't reach destination, skip
            if (agent.pathEndPosition != agent.destination)
                return NodeState.FAILURE;

            // If reached destination, succeed
            if (agent.remainingDistance <= agent.stoppingDistance * 2f)
                return NodeState.SUCCESS;

            return NodeState.RUNNING;
        }

        private NodeState KillTimer()
        {
            var behind = GetBehindPosition();

            if (!behind.HasValue)
                return NodeState.FAILURE;

            var distance = Vector3.Distance(behind.Value, agent.transform.position);

            // If close enough, decrease timer
            if (distance <= 3.5f)
                timer -= Time.deltaTime;
            else
                timer += Time.deltaTime * 0.5f;

            timer = Mathf.Clamp(timer, 0, TIMER_SECONDS);

            return timer <= 0 ? NodeState.SUCCESS : NodeState.RUNNING;
        }

        private NodeState KillTarget()
        {
            fiddlesticks.Kill();
            return NodeState.RUNNING;
        }

        private Vector3? GetBehindPosition()
        {
            Vector3? position;

            Transform t = fiddlesticks.GetTarget();

            if (t != null)
            {
                position = t.position;
                var back = -t.forward;
                back.y = 0;

                // Get position behind the target
                if (Physics.Raycast(position.Value, back, out var hit, 2))
                {
                    position = new Vector3(
                        hit.transform.position.x,
                        t.position.y,
                        hit.transform.position.z
                    );
                }
                else
                    position += back * 2;
            }
            else
                position = agent.destination;

            if (!position.HasValue)
                return null;

            if (NavMesh.SamplePosition(position.Value, out var navMeshHit, maxDistance: 0.75f, NavMesh.AllAreas))
                return navMeshHit.position;
            return null;
        }

        #region Node

        public override string GetText() => "Lurk";

        #endregion
    }
}