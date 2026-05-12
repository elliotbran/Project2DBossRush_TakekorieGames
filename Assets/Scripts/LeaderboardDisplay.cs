using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ScoreEntry
{
    public string name;
    public float time;
}

[System.Serializable]
public class ScoreList
{
    public List<ScoreEntry> items;
}

public class LeaderboardDisplay : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI leaderboardText;

    // Tu URL de implementación
    private string webAppUrl = "https://script.google.com/macros/s/AKfycbwHKmNKOepsUCLYDIPkcBlIrs4pm5rAPqpzE8GzURPUsBXciIApP1yRgH8mo4etP4dJxA/exec";

    void Start()
    {
        StartCoroutine(GetScores());
    }

    public void RefreshLeaderboard()
    {
        StartCoroutine(GetScores());
    }

    IEnumerator GetScores()
    {
        if (leaderboardText != null) leaderboardText.text = "Cargando rankings...";

        using (UnityWebRequest www = UnityWebRequest.Get(webAppUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                if (leaderboardText != null) leaderboardText.text = "Error cargando los datos...";
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;

                // Formateamos el JSON para que Unity lo entienda (añadiendo la raíz "items")
                string fixJson = "{ \"items\": " + jsonResponse + "}";
                ScoreList data = JsonUtility.FromJson<ScoreList>(fixJson);

                DisplayScores(data.items);
            }
        }
    }

    void DisplayScores(List<ScoreEntry> scores)
    {
        if (leaderboardText == null) return;

        leaderboardText.text = "<color=#FFD700>TOP 10 RANKING</color>\n\n";

        if (scores == null || scores.Count == 0)
        {
            leaderboardText.text += "No hay records todavia. ¡Sé el primero!";
            return;
        }

        for (int i = 0; i < scores.Count; i++)
        {
            // Formato: 1. Jugador - 10.50s
            leaderboardText.text += $"{i + 1}. {scores[i].name} - {scores[i].time:F2}s\n";
        }
    }
}