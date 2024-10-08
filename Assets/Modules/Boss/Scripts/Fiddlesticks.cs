using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Fiddlesticks : BehaviourTree.Trees.Tree
    {
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            killCollider = GetComponent<Collider>();
        }

        public Transform tempTarget;

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
            var root = new Selector();
            root += new ChaseNode(agent, "TARGET");

            root.SetData("TARGET", tempTarget);

            return root.Alias("ROOT");
        }

        #endregion

        #region Movement

        [Header("Movement")]
        private NavMeshAgent agent;

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

        #region Damage

        [Header("Damage")]
        [SerializeField]
        private ParticleSystem damageParticles;

        private int healthLeft;

        public void SetHealth(int amount)
        {
            healthLeft = amount;
        }

        public void Damage()
        {
            healthLeft--;

            damageParticles.Play();
        }

        #endregion

        #region Kill

        private Collider killCollider;

        public void StartKill()
        {
            animator.SetTrigger("Kill");
            agent.enabled = false;
        }

        public void Kill()
        {
            FindAnyObjectByType<CutscenesModule.DeathCutscene>().Play();
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

            Kill();
        }

        #endregion
    }
}