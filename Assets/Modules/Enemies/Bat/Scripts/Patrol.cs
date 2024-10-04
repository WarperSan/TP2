using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Generic;
using BehaviourTree.Nodes.Controls;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

namespace BehaviourTree.Trees
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Patrol : Tree
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform[] destinations;

        [SerializeField]
        private float detectionRange = 3f;

        protected override Node SetUpTree()
        {
            var agent = GetComponent<NavMeshAgent>();

            var root = new Selector();
            root += new ChaseNode(
                agent,
                target,
                detectionRange
            );
            root += new PatrolNode(
                agent,
                destinations
            );

            return root;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, detectionRange, 5);
        }

#endif
    }

    public class GoToTarget : Node
    {
        private readonly NavMeshAgent agent;
        private readonly string target;

        public GoToTarget(NavMeshAgent agent, string target)
        {
            this.agent = agent;
            this.target = target;
        }

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            agent.destination = this.GetData<Transform>(target).position;

            if (agent.remainingDistance <= agent.stoppingDistance)
                return NodeState.SUCCESS;

            return NodeState.RUNNING;
        }
    }

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

    public class PatrolNode : Sequence
    {
        private readonly Transform[] targets;
        private int currentIndex;

        const string CURRENT_TARGET = "currentTarget";

        public PatrolNode(NavMeshAgent agent, Transform[] targets)
        {
            // Aller à la prochaine destination
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

    public class ChaseNode : Sequence
    {
        const string TARGET = "target";

        public ChaseNode(NavMeshAgent agent, Transform target, float range)
        {
            Attach(
                new DistanceSmaller(agent.transform, TARGET, range)
            );

            Attach(
                new GoToTarget(agent, TARGET)
            );

            // Set data
            SetData(TARGET, target);
        }

        /// <inheritdoc/>
        public override string GetText() => "Chase";
    }
}