using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite defaultPortrait;
    public List<PortraitData> portraits;

    public Sprite GetPortrait(string id)
    {
        foreach (var p in portraits)
        {
            if (p.id == id)
                return p.sprite;
        }
        return defaultPortrait;
    }
}

[System.Serializable]
public class PortraitData
{
    public string id; // "Neutral", "Happy", "Angry"
    public Sprite sprite;
}
