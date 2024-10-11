using BehaviourTree.Nodes;
using BehaviourTree.Nodes.Controls;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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

        public bool IsVisible { get; private set; } = true;

        [SerializeField]
        private Animator animator;

        #region Behaviour Tree

        /// <inheritdoc/>
        protected override Node SetUpTree()
        {
            var root = new Selector();

            //root += new TeleportCloserNode(agent, "TARGET");
            //root += new HideNode(agent, "TARGET");
            root += new StareNode(this);
            root += new ChaseNode(this, agent);

            root.SetData("TARGET", tempTarget);

            return root.Alias("ROOT");
        }

        public Transform GetTarget() => tempTarget;

        #endregion

        #region Movement

        [Header("Movement")]
        private NavMeshAgent agent;

        private void OnDrawGizmos()
        {
            if (agent == null)
                return;

            Gizmos.color = new Color(255f / 255f, 127 / 255f, 80 / 255f);
            Gizmos.DrawCube(agent.destination, Vector3.one);
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

            this.IsVisible = isVisible;
        }

        public void Show() => SetVisibility(true);
        public void Hide() => SetVisibility(false);

        #endregion

        #region Stare

        [Header("Stare")]
        [SerializeField]
        private Volume stareEffect;

        public void StartStare()
        {
            agent.enabled = false;
            animator.SetTrigger("Spotted");
            SetKillActive(false);
            stareEffect.weight = 0.5f;
        }

        public void StareUpdate(float percent)
        {
            stareEffect.weight = Mathf.Clamp01(percent);
        }

        public void EndStare()
        {
            agent.enabled = true;
            SetKillActive(true);
        }

        #endregion

        #region Damage

        private int healthLeft;

        public void SetHealth(int amount)
        {
            healthLeft = amount;
        }

        public void Damage()
        {
            healthLeft--;
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
            enabled = false;
        }

        /// <summary>
        /// Sets the boss to be able to kill the player or not
        /// </summary>
        private void SetKillActive(bool isKillActive) => killCollider.isTrigger = isKillActive;

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