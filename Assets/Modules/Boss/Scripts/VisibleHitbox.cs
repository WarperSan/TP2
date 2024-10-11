using UnityEngine;

namespace BossModule
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VisibleHitbox : MonoBehaviour
    {
        public Fiddlesticks fiddlesticks;

        private MeshRenderer _renderer;

        #region MonoBehaviour

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            // If both are not visible, skip
            if (!fiddlesticks.IsVisible && !_renderer.isVisible)
                return;

            // If was visible, but not anymore, hide
            if (fiddlesticks.IsVisible && !_renderer.isVisible)
            {
                fiddlesticks.Hide();
                return;
            }

            var target = fiddlesticks.GetTarget();

            // If target invalid, skip
            if (target == null)
                return;

            Vector3 start = fiddlesticks.transform.position;
            Vector3 end = target.position;
            start.y += 0.5f;

            end.y = start.y;

            // If doesn't have a direct sight, skip
            if (!Physics.Raycast(start, end - start, out var hit) || hit.transform != target)
                return;

            fiddlesticks.Show();
        }

        #endregion
    }
}