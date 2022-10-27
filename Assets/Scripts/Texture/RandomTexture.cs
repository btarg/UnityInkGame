using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RandomTexture : MonoBehaviour
{
    public string resourcePath = "Textures/";
    Texture2D[] loadedTextures;

    // Start is called before the first frame update
    void Start()
    {
        loadedTextures = Resources.LoadAll<Texture2D>(resourcePath);
        Texture2D chosenTexture = loadedTextures[Random.Range(0, loadedTextures.Length)];

        GetComponent<Renderer>().material.mainTexture = chosenTexture;
        Debug.Log(gameObject.name + " set texture to " + chosenTexture.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
