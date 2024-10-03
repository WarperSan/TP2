using System.Collections;
using UnityEngine;

namespace CutscenesModule
{
    public class DeathCutscene : Cutscene
    {
        #region Cutscene

        /// <inheritdoc/>
        protected override IEnumerator OnPlay()
        {
            // If couldn't place player, finish
            if (!PlacePlayer())
                yield break;

            // If couldn't place boss, finish
            if (!PlaceBoss())
                yield break;

            // Animation
            yield return null;

            // Kill player
            KillPlayer();
        }

        #endregion

        #region Player

        [Header("Player")]
        [SerializeField]
        private Transform playerPosition;

        /// <summary>
        /// Places the player at the death cutscene point
        /// </summary>
        /// <returns>Succeed to place the player</returns>
        private bool PlacePlayer()
        {
            GameObject player = GetPlayer();

            // If player not found, fail
            if (player == null)
                return false;

            // Disable controls
            if (player.TryGetComponent(out PlayerController controller))
                controller.enabled = false;

            // Set position of the player
            player.transform.SetParent(playerPosition.transform);
            player.transform.localPosition = Vector3.zero;

            // Set rotation of the player
            player.transform.localRotation = Quaternion.Euler(0, 0, 0);

            return true;
        }

        private GameObject GetPlayer() => GameObject.FindGameObjectWithTag("Player");

        #endregion

        #region Boss

        [Header("Boss")]
        [SerializeField]
        private Transform bossPosition;

        /// <summary>
        /// Places the boss at the death cutscene point
        /// </summary>
        /// <returns>Succeed to place the boss</returns>
        private bool PlaceBoss()
        {
            GameObject boss = GetBoss();

            // If boss not found, fail
            if (boss == null)
                return false;

            // Set position of the player
            boss.transform.SetParent(bossPosition.transform);
            boss.transform.localPosition = Vector3.zero;

            // Set rotation of the player
            boss.transform.localRotation = Quaternion.Euler(0, 0, 0);

            return true;
        }

        private GameObject GetBoss() => GameObject.FindGameObjectWithTag("Boss");

        #endregion

        #region Death

        private void KillPlayer()
        {
            Debug.Log("The player is ded :3");
        }

        #endregion
    }
}
