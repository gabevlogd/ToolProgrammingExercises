using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawnerWindow : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 scrollPosition;
    private Vector3 spawnPosition = Vector3.zero;
    private string searchFilter = "";

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawnerWindow>("Prefab Spawner");
    }

    private void OnEnable()
    {
        LoadPrefabs();
    }

    private void OnGUI()
    {
        GUILayout.Label("Super Cool Tool", EditorStyles.boldLabel);
        
        GUILayout.Space(20);
        
        searchFilter = EditorGUILayout.TextField("Search", searchFilter);
        
        GUILayout.Space(20);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null)
            {
                LoadPrefabs();
                return;
            }
            
            if (!string.IsNullOrEmpty(searchFilter) && !prefab.name.ToLower().Contains(searchFilter.ToLower()))
                continue;
            
            GUILayout.BeginHorizontal();
        
            // Show the preview of the prefab
            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            GUILayout.Label(preview, GUILayout.Width(50), GUILayout.Height(50));

            // Button for spawning the prefab
            if (GUILayout.Button(prefab.name, GUILayout.Height(50)))
            {
                SpawnPrefab(prefab);
            }
        
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndScrollView();
        
        GUILayout.Space(20);
        
        spawnPosition = EditorGUILayout.Vector3Field("Spawn Point", spawnPosition);

        if (GUILayout.Button("Refresh Prefabs Folder"))
        {
            LoadPrefabs();
        }
    }

    private void LoadPrefabs()
    {
        prefabs.Clear();
        // Find all prefabs in the Prefabs folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Level 2/Exercise 2/Prefabs" });

        foreach (string guid in guids)
        {
            // Get the path of the prefab
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Load the prefab
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                prefabs.Add(prefab);
            }
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        // Not the correct way to instantiate prefabs in Editor if you want to keep the prefab connection
        //GameObject instance = Instantiate(prefab); 
        
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = spawnPosition;
        Selection.activeGameObject = instance;
        
        // Register the creation of the object for undo (Make ctrl+z work)
        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
    }
}