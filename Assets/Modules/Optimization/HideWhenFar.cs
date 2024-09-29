using UnityEngine;

namespace OptimizationModule
{
    public class HideWhenFar : MonoBehaviour
    {
        private static Camera cameraToCull = null;
        private static Plane[] planes;
        private static GameObject registerObject;
        private Renderer[] renderers;

        private void Awake()
        {
            // Assign camera to main
            if (cameraToCull == null)
                cameraToCull = Camera.main;

            // Become the register
            if (registerObject == null)
                registerObject = gameObject;

            renderers = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            // Become the register
            if (registerObject == null)
                registerObject = gameObject;

            // If this is the register
            if (registerObject == gameObject)
            {
                // Get the camera's frustum planes
                planes = GeometryUtility.CalculateFrustumPlanes(cameraToCull);
            }
        }

        private void LateUpdate()
        {
            // Check if the bounds are within the frustum
            foreach (var item in renderers)
                item.enabled = GeometryUtility.TestPlanesAABB(planes, item.bounds);
        }
    }
}
