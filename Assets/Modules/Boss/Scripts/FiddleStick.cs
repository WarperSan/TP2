using UnityEngine;
using UnityEngine.AI;

namespace BossModule
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FiddleStick : MonoBehaviour
    {
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public Transform tempTarget;

        private void Update()
        {
            UpdateTarget(tempTarget.position);
        }

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
    }
}