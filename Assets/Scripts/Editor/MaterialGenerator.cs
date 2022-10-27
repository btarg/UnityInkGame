using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class MaterialGenerator : EditorWindow
{
    private string path = "Assets/";
    private string output_path = "Assets/Materials";
    FilterMode filterMode = FilterMode.Point;
    private string shaderName = "Universal Render Pipeline/Unlit";
    private string texturePropertyName = "_BaseMap";

    private bool overwrite = false;

    [MenuItem("Quake Tools/Generate materials")]
    public static void ShowWindow()
    {
        GetWindow<MaterialGenerator>("Material Generator");
    }


    void OnGUI()
    {
        GUILayout.Label("Current texture directory:");
        GUILayout.Label(path);
        if (GUILayout.Button("Browse texture folder"))
        {
            path = EditorUtility.OpenFolderPanel("Select folder with textures", "Assets/", "");
        }


        GUILayout.Space(24);
        GUILayout.Label("Current output directory:");
        GUILayout.Label(output_path);
        if (GUILayout.Button("Browse output folder"))
        {
            output_path = EditorUtility.OpenFolderPanel("Select folder to dump materials to", "Assets/", "");
        }


        GUILayout.Space(24);

        GUILayout.Label("Material Shader");
        shaderName = GUILayout.TextField(shaderName);
        GUILayout.Label("Texture Property Name");
        texturePropertyName = GUILayout.TextField(texturePropertyName);

        GUILayout.Space(24);
        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode:", filterMode);

        GUILayout.Space(24);
        overwrite = GUILayout.Toggle(overwrite, "Overwrite existing files");
        if (GUILayout.Button("GENERATE!"))
        {
            Generate();
        }

    }

    void Generate()
    {
        if (path.Length > 0 && output_path.Length > 0)
        {
            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".jfif") || s.EndsWith(".png") || s.EndsWith(".tga"));

            foreach (var file in files)
            {
                // "file" is the full path and name
                string[] fileSplit = file.Split("/");
                string extension = fileSplit[fileSplit.Length - 1];

                // load the texture from the file using relative path
                Texture2D tex = AssetDatabase.LoadAssetAtPath(BSPCommon.ConvertPath(file), typeof(Texture2D)) as Texture2D;
                tex.filterMode = filterMode;
                // set the shader and texture
                Material material = new Material(Shader.Find(shaderName));
                material.SetTexture(texturePropertyName, tex);

                // get the last item split by slash
                string fileName = fileSplit[fileSplit.Length - 1].Split(".")[0];

                // remove topmost folder
                string toRemove = fileName.Substring(0, fileName.Split("\\")[0].Length + 1);
                fileName = fileName.Replace(toRemove, "");

                string finalPath = Path.Combine(output_path, fileName) + ".mat";
                finalPath = BSPCommon.ConvertPath(finalPath);


                // the parent folder (1 level) is part of the file name usually
                string dirName = Path.GetDirectoryName(BSPCommon.ConvertPath(output_path));

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                if (!Directory.Exists(output_path))
                {
                    Directory.CreateDirectory(output_path);
                }
                if (!Directory.Exists(Path.GetDirectoryName(finalPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
                }

                // overwrite existing asset
                if (Directory.Exists(finalPath))
                {
                    if (overwrite) {
                        Debug.LogWarning("Overwriting file: " + finalPath);
                        AssetDatabase.DeleteAsset(finalPath);

                    } else {
                        Debug.LogWarning("Material already exists! Skipping...");
                        continue;
                    }
                    
                }

                AssetDatabase.CreateAsset(material, finalPath);
                Debug.Log("Created new asset: " + finalPath);

            }

        }
    }
}