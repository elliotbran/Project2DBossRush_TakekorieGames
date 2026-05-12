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
    public TMP_InputField nameInputField;
    public Button submitButton;
    public GameObject submitbuttonGame;

    private float elapsedTime = 0f;
    private bool isRunning = true;
    private const string BestTimeKey = "BestTimeRecord";

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

        // Guardamos el tiempo exacto al morir el boss
        PlayerPrefs.SetFloat("CurrentTime", elapsedTime);
        PlayerPrefs.Save();

        CheckNewRecord();
    }

    private void CheckNewRecord()
    {
        float currentBest = PlayerPrefs.GetFloat(BestTimeKey, 999999f);
        if (elapsedTime < currentBest)
        {
            PlayerPrefs.SetFloat(BestTimeKey, elapsedTime);
            PlayerPrefs.Save();
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

    public void SubmitScore()
    {
        if (submitButton != null) submitButton.interactable = false;
        submitbuttonGame.SetActive(false);

        // LEEMOS EL TIEMPO DESDE PLAYERPREFS (para evitar el error del 0)
        float timeToSubmit = PlayerPrefs.GetFloat("CurrentTime", 0f);

        string playerName = "Anonymous Player";
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            playerName = nameInputField.text;
        }

        StartCoroutine(PostToGoogle(playerName, timeToSubmit));
    }

    // El IEnumerator DEBE estar fuera de la función SubmitScore
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
                if (submitButton != null) submitButton.interactable = true; // Reintentar si falla
            }
            else
            {
                Debug.Log("¡Puntuación subida con éxito! Tiempo: " + time);
            }
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(6);
    }
}