using Unity.Cinemachine;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    private float startingIntensity;
    private float shakeTimer;
    private float shakeTimerTotal;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            noise.AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        noise.AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
}
