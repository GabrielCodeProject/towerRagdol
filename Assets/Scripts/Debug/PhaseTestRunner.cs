using System.Collections;
using RagdollRealms.Core;
using UnityEngine;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Temporary test helper. Calls StartGame() after one frame.
    /// Delete after testing.
    /// </summary>
    public class PhaseTestRunner : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;

            if (!ServiceLocator.Instance.TryGet<IPhaseManager>(out var phaseManager))
            {
                UnityEngine.Debug.LogError("[PhaseTestRunner] IPhaseManager not found. Is PhaseManager in the scene?");
                yield break;
            }

            UnityEngine.Debug.Log("[PhaseTestRunner] Starting game...");
            phaseManager.StartGame();
        }
    }
}
