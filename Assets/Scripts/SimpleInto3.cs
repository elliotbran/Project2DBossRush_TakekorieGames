using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


public class SImpleIntro3 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject boton2;
    [SerializeField] private GameObject boton3;
    [SerializeField] private TextMeshProUGUI currentTime;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    private const string BestTimeKey = "BestTimeRecord";

    [Header("Configuración")]
    [TextArea(3, 10)]
    [SerializeField] private string introText; // Aquí escribes tu historia

    void Start()
    {
        float currentBest = PlayerPrefs.GetFloat(BestTimeKey, 999999f);
        // Al empezar la escena, lanzamos el efecto
        if (typewriterEffect != null && textLabel != null)
        {
            typewriterEffect.Run(introText, textLabel);
        }
        StartCoroutine(cutradaFacto());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit"))
        {
            if (typewriterEffect.IsRunning)
            {
                typewriterEffect.Stop();
            }
            else
            {
                if (textLabel.text == introText)
                {
                    inputField.SetActive(true);
                    boton2.SetActive(true);
                    boton3.SetActive(true);
                    currentTime.text = "Current time: " + PlayerPrefs.GetFloat("CurrentTime", 0f).ToString("F2");
                    if (bestTimeText != null)
                    {
                        float best = PlayerPrefs.GetFloat(BestTimeKey, 0f);
                        if (best == 0f || best == 999999f)
                            bestTimeText.text = "Record: --.--";
                        else
                            bestTimeText.text = "Record: " + best.ToString("F2");
                    }
                    //SceneManager.LoadScene(4);
                }

            }
        }
    }

    public void dontSUbmit()
    {
        SceneManager.LoadScene(6);
    }
    IEnumerator cutradaFacto()
    {
        yield return new WaitForSeconds(1.5f);
        inputField.SetActive(true);
        boton2.SetActive(true);
        boton3.SetActive(true);
        currentTime.text = "Current time: "+PlayerPrefs.GetFloat("CurrentTime", 0f).ToString("F2");
        if (bestTimeText != null)
        {
            float best = PlayerPrefs.GetFloat(BestTimeKey, 0f);
            if (best == 0f || best == 999999f)
                bestTimeText.text = "Record: --.--";
            else
                bestTimeText.text = "Record: " + best.ToString("F2");
        }
        //SceneManager.LoadScene(4);
    }
}