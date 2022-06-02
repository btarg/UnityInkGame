using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterScriptableObject", order = 1)]
public class CharacterScriptableObject : ScriptableObject
{
    public string characterName;
    public Color characterColor;
    public AudioClip[] characterTypingSounds;
    public AudioClip[] characterIdleSounds;
}