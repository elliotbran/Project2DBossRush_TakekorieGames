using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour,
    ISelectHandler, IDeselectHandler
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (text != null)
            text.color = Color.yellow;

        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (text != null)
            text.color = Color.black;

        transform.localScale = Vector3.one;
    }
}
