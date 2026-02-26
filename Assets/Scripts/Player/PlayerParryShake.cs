using UnityEngine;

public class PlayerParryShake : MonoBehaviour
{
    public void TriggerShake()
    {
        // Solo se activará cuando lo llamemos desde el PlayerController
        CinemachineShake.Instance.ShakeCamera(10f, .2f);
    }
}
