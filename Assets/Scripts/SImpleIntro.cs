using UnityEngine;
using TMPro;

public class SImpleIntro : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private TMP_Text textLabel;

    [Header("Configuración")]
    [TextArea(3, 10)]
    [SerializeField] private string introText; // Aquí escribes tu historia
    [TextArea(3, 10)]
    [SerializeField] private string introText2;
    [TextArea(3, 10)]
    [SerializeField] private string introText3;

    void Start()
    {
        // Al empezar la escena, lanzamos el efecto
        if (typewriterEffect != null && textLabel != null)
        {
            typewriterEffect.Run(introText, textLabel);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (typewriterEffect.IsRunning)
            {
                typewriterEffect.Stop();
            }
            else
            {
                if (textLabel.text == introText)
                {
                    typewriterEffect.Run(introText2, textLabel);
                }
                else if (textLabel.text == introText2)
                {
                    typewriterEffect.Run(introText3, textLabel);
                }
            }
        }
    }
}