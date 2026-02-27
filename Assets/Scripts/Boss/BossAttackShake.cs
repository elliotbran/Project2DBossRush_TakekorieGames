using UnityEngine;

public class BossAttackShake : MonoBehaviour
{
    private void Update()
    {
        CinemachineShake.Instance.ShakeCamera(17.5f, .3f); // Shake the camera when the boss attacks
    }
}
