using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


enum RectBorder
{
    None,
    Left,
    Right,
    Top,
    Bottom
}


public class PrefabSpawnerWindow : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 scrollPosition;
    private Vector3 spawnPosition = Vector3.zero;
    private string searchFilter = "";
    private Rect sceneViewRect;
    private List<Rect> snapCornerRects = new List<Rect>();


    private bool enableSceneViewRectDrag = false;

    private bool enableSceneViewRectResize = false;

    private RectBorder rectBorder = RectBorder.None;

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawnerWindow>("Prefab Spawner");
    }

    private void OnEnable()
    {
        LoadPrefabs();

        InitSnapCorner();

        sceneViewRect.size = new Vector2(300, 400);

        SceneView.duringSceneGui += DrawOnSceneGUI;

    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DrawOnSceneGUI;
    }

    private void OnValidate()
    {
        Debug.Log("OnValidate");
    }

    private void OnGUI()
    {
        GUILayout.Label("Super Cool Tool", EditorStyles.boldLabel);



        GUILayout.Space(20);

        spawnPosition = EditorGUILayout.Vector3Field("Spawn Point", spawnPosition);

        if (GUILayout.Button("Refresh Prefabs Folder"))
        {
            LoadPrefabs();
            InitSnapCorner();
        }
    }

    private void LoadPrefabs()
    {
        prefabs.Clear();
        // Find all prefabs in the Prefabs folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });

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

        Undo.RecordObject(instance, "");
    }

    private void DrawOnSceneGUI(SceneView sceneView)
    {
        sceneViewRect = GUI.Window(0, sceneViewRect, DrawOnSceneViewRect, "SHISH");


        if (UpdateSceneViewRectScale(sceneView))
        {
            return;
        }
        UpdateSceneViewRectPosition(sceneView);
    }

    private void UpdateSceneViewRectPosition(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (IsMouseOverRect(sceneViewRect))
                enableSceneViewRectDrag = true;
        }

        if (Event.current.type == EventType.MouseUp)
        {
            TrySnapSceneViewRect();
            enableSceneViewRectDrag = false;
        }

        if (enableSceneViewRectDrag)
        {
            DrawSnapCornerPreview();

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                sceneViewRect.position += Event.current.delta;
                sceneViewRect.x = Mathf.Clamp(sceneViewRect.x, 0f, sceneView.camera.pixelWidth - sceneViewRect.width);
                sceneViewRect.y = Mathf.Clamp(sceneViewRect.y, 0f, sceneView.camera.pixelHeight - sceneViewRect.height);
                sceneView.Repaint();
                Event.current.Use();
            }
        }
    }

    
    /// <returns>True if the scale was changed</returns>
    private bool UpdateSceneViewRectScale(SceneView sceneView)
    {

        if (Event.current.type == EventType.MouseDown)
        {
            if (IsMouseOverRectBorder(sceneViewRect, 10, out rectBorder))
                enableSceneViewRectResize = true;          
        }
        if (Event.current.type == EventType.MouseUp)
        {
            enableSceneViewRectResize = false;
            rectBorder = RectBorder.None;
        }
        if (enableSceneViewRectResize)
        {
            switch (rectBorder)
            {
                case RectBorder.None:
                    break;
                case RectBorder.Left:

                    break;
                case RectBorder.Right:
                    float newRectWidth = sceneViewRect.width + Event.current.delta.x;
                    if (newRectWidth < 100)
                        return false;
                    sceneViewRect.width = newRectWidth;
                    sceneView.Repaint();
                    return true;
                case RectBorder.Top:

                    break;
                case RectBorder.Bottom:
                    float newRectHeight = sceneViewRect.height + Event.current.delta.y;
                    if (newRectHeight < 100)
                        return false;
                    sceneViewRect.height = newRectHeight;
                    sceneView.Repaint();
                    return true;
            }
        }
        return false;
    }

    private void DrawOnSceneViewRect(int index)
    {

        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            scrollPosition.y += e.delta.y * 5;
            e.Use();
        }

        GUILayout.Space(20);

        searchFilter = EditorGUILayout.TextField("Search", searchFilter);

        GUILayout.Space(20);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null)
            {
                LoadPrefabs();
                break;
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
    }

    private bool IsMouseOverRect(Rect rect)
    {
        if (rect == null) return false;
        Vector2 relativePointerPos = Event.current.mousePosition - rect.position;
        if (relativePointerPos.x < rect.width && relativePointerPos.y < rect.height && relativePointerPos.x > 0 && relativePointerPos.y > 0)
            return true;

        return false;
    }

    private bool IsMouseOverRectBorder(Rect rect, float tolerance, out RectBorder wichBorder)
    {
        wichBorder = RectBorder.None;
        if (rect == null) return false;
        if (!IsMouseOverRect(rect)) return false;

        Vector2 relativePointerPos = Event.current.mousePosition - rect.position;

        if (relativePointerPos.x < tolerance || relativePointerPos.y < tolerance || relativePointerPos.x > rect.width - tolerance || relativePointerPos.y > rect.height - tolerance)
        {
            if (relativePointerPos.x < tolerance)
            {
                wichBorder = RectBorder.Left;
            }
            else if (relativePointerPos.x > rect.width - tolerance)
            {
                wichBorder = RectBorder.Right;
            }
            else if (relativePointerPos.y < tolerance)
            {
                wichBorder = RectBorder.Top;
            }
            else if (relativePointerPos.y > rect.height - tolerance)
            {
                wichBorder = RectBorder.Bottom;
            }
            else wichBorder = RectBorder.None;
            return true;
        }
        return false;

    }

    private List<GameObject> GetPrefabs()
    {
        List<GameObject> gameobject = new List<GameObject>();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log(path + " unity " + guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab)
            {
                gameobject.Add(prefab);
            }
        }
        return gameobject;
    }


    private void InitSnapCorner()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        float snapCornerSize = 50;

        snapCornerRects = new List<Rect>();
        snapCornerRects.Add(new Rect(0, 0, snapCornerSize, snapCornerSize));
        snapCornerRects.Add(new Rect(0, sceneView.camera.pixelHeight - snapCornerSize, snapCornerSize, snapCornerSize));
        snapCornerRects.Add(new Rect(sceneView.camera.pixelWidth - snapCornerSize, 0, snapCornerSize, snapCornerSize));
        snapCornerRects.Add(new Rect(sceneView.camera.pixelWidth - snapCornerSize, sceneView.camera.pixelHeight - snapCornerSize, snapCornerSize, snapCornerSize));
    }

    private void DrawSnapCornerPreview()
    {
        foreach (Rect rect in snapCornerRects)
        {
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(rect, new Color(0, 0, 0, 0.1f), Color.blue);
            Handles.EndGUI();
        }
    }


    private void TrySnapSceneViewRect()
    {
        foreach (Rect rect in snapCornerRects)
        {
            if (IsMouseOverRect(rect))
            {
                sceneViewRect.position = rect.position;
                if (sceneViewRect.max.x > SceneView.lastActiveSceneView.camera.pixelWidth)
                    sceneViewRect.x = SceneView.lastActiveSceneView.camera.pixelWidth - sceneViewRect.width;
                if (sceneViewRect.max.y > SceneView.lastActiveSceneView.camera.pixelHeight)
                    sceneViewRect.y = SceneView.lastActiveSceneView.camera.pixelHeight - sceneViewRect.height;
                break;
            }
        }
    }
}