using UnityEngine;

public class PlayerParryShake : MonoBehaviour
{
    private void Update()
    {
        CinemachineShake.Instance.ShakeCamera(10f, .2f); // Shake the camera when the boss attacks
    }
}
