using UnityEngine;
using TMPro; // Asegúrate de usar TextMeshPro para mejor calidad visual

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            // Formatea a 2 decimales: "00.00"
            timerText.text = elapsedTime.ToString("F2");
        }
    }

    // Esta es la función que llamarás desde el Boss
    public void StopTimer()
    {
        isRunning = false;
        // Opcional: Cambiar el color del texto para indicar que terminó
        timerText.color = Color.yellow;
    }
}