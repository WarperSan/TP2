using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using BossModule;

namespace EnemyModule
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Bat : BehaviourTree.Trees.Tree
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform[] destinations;

        [SerializeField]
        private float detectionRange = 3f;

        private Fiddlesticks fiddlesticks;

        private GlitchController glitchController;

        private void Awake()
        {
            fiddlesticks = FindObjectOfType<Fiddlesticks>();
            glitchController = FindObjectOfType<GlitchController>(true);
        }

        #region Behaviour Tree

        private const string PLAYER_TARGET = "target";

        /// <inheritdoc/>
        protected override Node SetUpTree()
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            var root = new Selector();
            root += new ChaseNode(
                this,
                agent,
                PLAYER_TARGET,
                detectionRange
            );
            root += new PatrolNode(
                agent,
                destinations
            );

            // Set data
            root.SetData(PLAYER_TARGET, target);

            return root.Alias("Root");
        }

        #endregion

        #region Animation

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        public void SetAlerted(bool isAlerted)
        {
            animator.SetBool("isCircling", isAlerted);
            fiddlesticks.IsAlerted = isAlerted;
            glitchController.gameObject.SetActive(isAlerted);
        }

        #endregion

        #region Editor
#if UNITY_EDITOR
        /// <inheritdoc/>
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, detectionRange, 5);
        }
#endif
        #endregion
    }
}