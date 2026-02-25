using UnityEngine;

public class PlayerAttackShake : MonoBehaviour
{
    private void Update()
    {
        CinemachineShake.Instance.ShakeCamera(2.5f, .1f); // Shake the camera when the boss attacks
    }
}
