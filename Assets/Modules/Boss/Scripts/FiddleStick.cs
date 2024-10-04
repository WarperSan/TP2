using BehaviourTree.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class FiddleStick : BehaviourTree.Trees.Tree
    {
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            killCollider = GetComponent<Collider>();
        }

        public Transform tempTarget;

        private void Update()
        {
            UpdateTarget(tempTarget.position);
        }

        private void OnBecameVisible()
        {
            if (!wasHidden)
                return;

            wasHidden = false;
            animator.SetTrigger("Spotted");
            SetKillActive(false);
        }

        public bool wasHidden = false;
        public Animator animator;

        #region Behaviour Tree

        /// <inheritdoc/>
        protected override Node SetUpTree()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Movement

        [Header("Movement")]
        private NavMeshAgent agent;

        private void UpdateTarget(Vector3 position)
        {
            // Prevent off-mesh errors
            if (!agent.isOnNavMesh)
                return;

            agent.destination = position;
        }

        #endregion

        #region Visibility

        [Header("Visibility")]
        [SerializeField]
        private Renderer[] renderers;

        [SerializeField]
        private Light[] lights;

        private void SetVisibility(bool isVisible)
        {
            foreach (var item in renderers)
                item.enabled = isVisible;

            foreach (var item in lights)
                item.enabled = isVisible;
        }

        public void Show() => SetVisibility(true);
        public void Hide() => SetVisibility(false);

        #endregion

        #region Kill

        private Collider killCollider;

        public void StartKill()
        {
            animator.SetTrigger("Kill");
            agent.enabled = false;
        }

        /// <summary>
        /// Sets the boss to be able to kill the player or not
        /// </summary>
        public void SetKillActive(bool isKillActive) => killCollider.isTrigger = isKillActive;

        /// <inheritdoc/>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.CompareTag("Player"))
                return;

            FindAnyObjectByType<CutscenesModule.DeathCutscene>().Play();
        }

        #endregion
    }
}