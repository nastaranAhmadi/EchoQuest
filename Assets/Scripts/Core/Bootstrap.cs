using UnityEngine;

namespace EchoQuest.Core
{
    /// <summary>
    /// Entry point for the EchoQuest session. Expanded in later phases.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("EchoQuest bootstrap ready. Phase 0 foundation.");
        }
    }
}
