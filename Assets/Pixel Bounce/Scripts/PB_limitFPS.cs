
using UnityEngine;

public class PB_limitFPS : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;
    }
}
