// Code by gamesproi
// https://forum.unity.com/threads/custom-effect-links-textmeshpro-ignoring-spaces-in-linktextfirstcharacterindex.1201786/#post-7678843

using UnityEngine;
using TMPro;

public class TextmeshproEffect : MonoBehaviour
{
    public TMP_Text textComponent;

    public Vector2 movementStrength = new Vector2(0.1f, 0.1f);
    public float movementSpeed = 1f;
    public float rainbowStrength = 10f;

    private void Update()
    {
        textComponent.ForceMeshUpdate();

        // Loops each link tag
        foreach (TMP_LinkInfo link in textComponent.textInfo.linkInfo)
        {
            // Is it a rainbow tag? (<link="rainbow"></link>)
            if (link.GetLinkID() == "rainbow")
            {
                // Loops all characters containing the rainbow link.
                for (int i = link.linkTextfirstCharacterIndex; i < link.linkTextfirstCharacterIndex + link.linkTextLength; i++)
                {
                    TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[i]; // Gets info on the current character
                    int materialIndex = charInfo.materialReferenceIndex; // Gets the index of the current character material

                    Color32[] newColors = textComponent.textInfo.meshInfo[materialIndex].colors32;
                    Vector3[] newVertices = textComponent.textInfo.meshInfo[materialIndex].vertices;

                    // Loop all vertexes of the current characters
                    for (int j = 0; j < 4; j++)
                    {
                        if (charInfo.character == ' ') continue; // Skips spaces
                        int vertexIndex = charInfo.vertexIndex + j;

                        // Offset and Rainbow effects, replace it with any other effect you want.
                        Vector3 offset = new Vector2(Mathf.Sin((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * movementStrength.x)), Mathf.Cos((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * movementStrength.y))) * 10f;
                        Color32 rainbow = Color.HSVToRGB(((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * (0.001f * rainbowStrength))) % 1f, 1f, 1f);

                        // Sets the new effects
                        newColors[vertexIndex] = rainbow;
                        newVertices[vertexIndex] += offset;
                    }
                }
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All); // IMPORTANT! applies all vertex and color changes.
    }
}
