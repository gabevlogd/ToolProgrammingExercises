using System;
using UnityEditor;
using UnityEngine;

public class GridPlacerWindow : EditorWindow
{
    private GameObject prefab;
    private int rows = 5;
    private int columns = 5;
    private float spacing = 2f;
    private Vector3 startPosition = Vector3.zero;
    
    private float rotation = 0f;
    private Vector3 scale = Vector3.one;
    
    private bool showGridSection = false;
    private bool showOrientationSection = false;
    
    private Transform referenceSurface;
    
    private enum PatternType { Griglia, Cerchio, Spirale, OnSurface }
    private PatternType patternType = PatternType.Griglia;

    [MenuItem("Tools/Grid Placer")]
    public static void ShowWindow()
    {
        GetWindow<GridPlacerWindow>("Grid Placer");
    }

    private void OnGUI()
    {

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        showGridSection = EditorGUILayout.Foldout(showGridSection, "Grid Options");
        if (showGridSection)
        {
            EditorGUI.indentLevel++;
            startPosition = EditorGUILayout.Vector3Field("Start Position", startPosition);
            rows = Mathf.Max(1, EditorGUILayout.IntField("Row", rows));
            columns = Mathf.Max(1, EditorGUILayout.IntField("Column", columns));
            spacing = Mathf.Max(1, EditorGUILayout.FloatField("Spacing", spacing));
            EditorGUI.indentLevel--;
        }
        
        showOrientationSection = EditorGUILayout.Foldout(showOrientationSection, "Orientation Options");
        if (showOrientationSection)
        {
            EditorGUI.indentLevel++;
            
            scale = EditorGUILayout.Vector3Field("Scale", scale);
            rotation = EditorGUILayout.FloatField("Rotation", rotation);
            EditorGUI.indentLevel--;
        }
        
        
        patternType = (PatternType)EditorGUILayout.EnumPopup("Tipo di Pattern", patternType);
        if (patternType == PatternType.OnSurface)
        {
            referenceSurface = (Transform)EditorGUILayout.ObjectField("Superficie di riferimento", referenceSurface,
                typeof(Transform), true);
        }
        else columns = 1;
        
        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }
    }

    private void GenerateGrid()
    {
        if (prefab == null)
        {
            Debug.LogWarning("Seleziona un prefab prima di generare la griglia!");
            return;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 transformPosition = startPosition;

                switch (patternType)
                {
                    // Generazione di un pattern a griglia
                    case PatternType.Griglia:
                        transformPosition += new Vector3(i * spacing, 0, j * spacing);
                        break;
                    // Generazione di un pattern a cerchio
                    case PatternType.Cerchio:
                    {
                        float angle = i * Mathf.PI * 2 / rows;
                        transformPosition += new Vector3(Mathf.Cos(angle) * spacing, 0, Mathf.Sin(angle) * spacing);
                        break;
                    }
                    // Generazione di un pattern a spirale
                    case PatternType.Spirale:
                    {
                        float angle = i * Mathf.PI * 2 / rows;
                        float height = i * spacing * 0.5f;
                        transformPosition += new Vector3(Mathf.Cos(angle) * spacing, height, Mathf.Sin(angle) * spacing);
                        break;
                    }
                    // Generazione di un pattern su una superficie
                    case PatternType.OnSurface:
                        if (referenceSurface != null)
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(new Vector3(transformPosition.x, 100, transformPosition.z), Vector3.down, out hit, Mathf.Infinity))
                            {
                                transformPosition.y = hit.point.y;  // Impostare la posizione Y sulla superficie
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.position = transformPosition;
                instance.transform.rotation = Quaternion.Euler(0, rotation, 0);
                instance.transform.localScale = scale;

                Undo.RegisterCreatedObjectUndo(instance, "Posizionato prefab in pattern");
            }
        }
    }
}