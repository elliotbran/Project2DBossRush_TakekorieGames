using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;
    public TMP_InputField nameInputField; // Arrastra aquí tu InputField de la UI
    public Button submitButton; // Arrastra aquí tu botón de "Enviar" de la UI

    private float elapsedTime = 0f;
    private bool isRunning = true;
    private const string BestTimeKey = "BestTimeRecord";

    // Datos de tu Google Form ya configurados
    private string formURL = "https://docs.google.com/forms/d/e/1FAIpQLSfVTjDmOfhvW6e0hobRuJyMZG5Zbg-lx3C_g9Eftw_UB__6Hw/formResponse";
    private string entryNombre = "entry.1881395907";
    private string entryTiempo = "entry.1765745545";

    void Start()
    {
        DisplayBestTime();
    }

    void Update()
    {
        if (isRunning && timerText != null)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = elapsedTime.ToString("F2");
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        if (timerText != null) timerText.color = Color.yellow;

        PlayerPrefs.SetFloat("CurrentTime", elapsedTime);

        CheckNewRecord();
    }

    private void CheckNewRecord()
    {
        float currentBest = PlayerPrefs.GetFloat(BestTimeKey, 999999f);

        if (elapsedTime < currentBest)
        {
            PlayerPrefs.SetFloat(BestTimeKey, elapsedTime);
            PlayerPrefs.Save();
            Debug.Log("¡Nuevo Récord Local!: " + elapsedTime);
            DisplayBestTime();
        }
    }

    private void DisplayBestTime()
    {
        if (bestTimeText != null)
        {
            float best = PlayerPrefs.GetFloat(BestTimeKey, 0f);
            if (best == 0f || best == 999999f)
                bestTimeText.text = "Record: --.--";
            else
                bestTimeText.text = "Record: " + best.ToString("F2");
        }
    }

    // Llama a esta función desde el botón de "Enviar"
    public void SubmitScore()
    {
        submitButton.interactable = false;
        string playerName = nameInputField != null ? nameInputField.text : "Anonymous Player";

        if (!string.IsNullOrEmpty(playerName))
        {
            StartCoroutine(PostToGoogle(playerName, elapsedTime));
        }
        else
        {
            playerName = "Anonymous Player";
            StartCoroutine(PostToGoogle(playerName, elapsedTime));
            Debug.LogWarning("Write a name please");
        }

        IEnumerator PostToGoogle(string name, float time)
        {
            WWWForm form = new WWWForm();
            form.AddField(entryNombre, name);
            form.AddField(entryTiempo, time.ToString("F2"));

            using (UnityWebRequest www = UnityWebRequest.Post(formURL, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error al subir a Google: " + www.error);
                }
                else
                {
                    Debug.Log("¡Puntuación subida a la nube con éxito!");
                    // Opcional: Desactivar el botón tras enviar
                }
            }
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(6);
        }
    }
}