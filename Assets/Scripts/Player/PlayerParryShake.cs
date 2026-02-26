using UnityEngine;

public class PlayerParryShake : MonoBehaviour
{
    public void TriggerShake()
    {
        CinemachineShake.Instance.ShakeCamera(10f, .2f);
    }
}
