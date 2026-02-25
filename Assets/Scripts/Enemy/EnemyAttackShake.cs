using UnityEngine;

public class EnemyAttackShake : MonoBehaviour
{
    private void Update()
    {
        CinemachineShake.Instance.ShakeCamera(25f, .3f); // Shake the camera when the boss attacks
    }
}
