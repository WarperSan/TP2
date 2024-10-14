using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using BehaviourTree.Nodes.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyModule
{
    public class ChaseNode : Sequence
    {
        float cooldownElapsed = 0;
        readonly float cooldown = 5f;
        float chaseElapsed = 0;
        readonly float chase = 5f;

        private readonly Bat bat;
        private readonly NavMeshAgent agent;
        private readonly string target;

        public ChaseNode(Bat bat, NavMeshAgent agent, string target, float range)
        {
            this.bat = bat;
            this.agent = agent;
            this.target = target;

            var cooldownSequence = new CallbackNode(() =>
            {
                cooldownElapsed -= Time.deltaTime;

                if (cooldownElapsed > 0)
                {
                    bat.SetAlerted(false);
                    return NodeState.RUNNING;
                }

                agent.enabled = true;
                return NodeState.SUCCESS;
            }).Alias("Cooldown");

            Attach(cooldownSequence);

            Attach(
                new DistanceSmaller(agent.transform, target, range)
            );

            Attach(ChaseSequence());
        }

        #region Chase Sequence

        private Node ChaseSequence()
        {
            var root = new Parallel();
            //root += new GoToTarget(agent, target); // Commented for a bug
            root += new CallbackNode(FollowPlayer);
            root += new CallbackNode(ChaseCooldown).Alias("IsChasing");
            root += AlertSequence();

            return root.Alias("Chasing");
        }

        private NodeState ChaseCooldown()
        {
            chaseElapsed += Time.deltaTime;

            if (chaseElapsed < chase)
                return NodeState.RUNNING;

            cooldownElapsed = cooldown;
            chaseElapsed = 0;
            agent.enabled = false;
            bat.SetAlerted(false);
            return NodeState.SUCCESS;
        }

        private NodeState FollowPlayer()
        {
            Transform t = GetData<Transform>(target);

            if (t == null)
                return NodeState.FAILURE;

            agent.enabled = false;
            bat.transform.position = t.position;

            return NodeState.RUNNING;
        }

        #endregion

        #region Alert Sequence

        private Node AlertSequence()
        {
            var root = new Sequence();
            root += new DistanceSmaller(agent.transform, target, 2);
            root += new CallbackNode(SetAlert);

            return root.Alias("Alert");
        }

        private NodeState SetAlert()
        {
            bat.SetAlerted(true);
            return NodeState.SUCCESS;
        }

        #endregion

        #region Node

        /// <inheritdoc/>
        public override string GetText() => "Chase";

        #endregion
    }
}