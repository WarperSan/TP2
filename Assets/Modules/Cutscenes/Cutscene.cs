using System.Collections;
using UnityEngine;

namespace CutscenesModule
{
    /// <summary>
    /// Class that represents a cutscene
    /// </summary>
    public abstract class Cutscene : MonoBehaviour
    {
        #region Play

        public void Play()
        {
            StartCoroutine(OnPlay());
        }

        protected abstract IEnumerator OnPlay();

        #endregion
    }
}