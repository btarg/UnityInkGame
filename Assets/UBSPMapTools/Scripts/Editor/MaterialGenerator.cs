using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class MaterialGenerator : EditorWindow
{
    private static string textures_path = "Assets/";
    private static string material_output_path = "Assets/Materials";
    FilterMode filterMode = FilterMode.Point;
    private string shaderName = "Universal Render Pipeline/Unlit";
    private string texturePropertyName = "_BaseMap";

    private bool overwrite = false;

    [MenuItem("Quake Tools/Generate materials")]
    public static void ShowWindow()
    {
        GetWindow<MaterialGenerator>("Material Generator");
        LoadSettings();
    }

    static void LoadSettings() {
        BSPCommon.LoadSettings();
        textures_path = BSPCommon.Q3TexturesPath;
        material_output_path = BSPCommon.MaterialsPath;
    }

    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal(GUILayout.Width(330), GUILayout.Height(20));
        EditorGUILayout.LabelField("Settings", GUILayout.Width(64));
        if (GUILayout.Button("Save", GUILayout.Width(64)))
        {
            BSPCommon.Q3TexturesPath = textures_path;
            BSPCommon.MaterialsPath = material_output_path;
            BSPCommon.SaveSettings();
        }
        if (GUILayout.Button("Load", GUILayout.Width(64)))
        {
            LoadSettings();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(24);

        GUILayout.Label("Current texture directory:");
        textures_path = GUILayout.TextField(textures_path);
        if (GUILayout.Button("Browse texture folder"))
        {
            textures_path = EditorUtility.OpenFolderPanel("Select folder with textures", "Assets/", "");
        }


        GUILayout.Space(8);
        GUILayout.Label("Current output directory:");
        
        material_output_path = GUILayout.TextField(material_output_path);

        if (GUILayout.Button("Browse output folder"))
        {
            material_output_path = EditorUtility.OpenFolderPanel("Select folder to dump materials to", "Assets/", "");
        }


        GUILayout.Space(24);

        GUILayout.Label("Material Shader");
        shaderName = GUILayout.TextField(shaderName);
        GUILayout.Label("Texture Property Name");
        texturePropertyName = GUILayout.TextField(texturePropertyName);

        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode:", filterMode);

        GUILayout.Space(24);
        overwrite = GUILayout.Toggle(overwrite, "Overwrite existing files");
        GUILayout.Space(8);
        if (GUILayout.Button("GENERATE!", GUILayout.Height(32)))
        {
            Generate();
        }

    }

    void Generate()
    {
        if (textures_path.Length > 0 && material_output_path.Length > 0)
        {
            var files = Directory.EnumerateFiles(textures_path, "*.*", SearchOption.AllDirectories)
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
                string fileName = BSPCommon.RemoveExtension(fileSplit[fileSplit.Length - 1]);

                // remove topmost folder
                string toRemove = fileName.Substring(0, fileName.Split("\\")[0].Length + 1);
                fileName = fileName.Replace(toRemove, "");

                string finalPath = Path.Combine(material_output_path, fileName) + ".mat";
                finalPath = BSPCommon.ConvertPath(finalPath);


                // the parent folder (1 level) is part of the file name usually
                string dirName = Path.GetDirectoryName(BSPCommon.ConvertPath(material_output_path));

                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                if (!Directory.Exists(material_output_path))
                {
                    Directory.CreateDirectory(material_output_path);
                }
                if (!Directory.Exists(Path.GetDirectoryName(finalPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
                }

                // overwrite existing asset
                if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(finalPath)))
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