using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class LevelEditor : EditorWindow
{
    public enum DrawingLayer { Decoration = 2, Terrain = 8, GameElements = 9, Players = 10 }
    private int layerMask = ~0;

    private Vector2 windowScrollPos = Vector2.zero;
    private Vector2 paletteScrollPos = Vector2.zero;

    private GameObject hexPrefab;
    private GameObject HexPrefab
    {
        get
        {
            if (!hexPrefab)
                hexPrefab = Resources.LoadAll<GameObject>("Terrain")[0];
            return hexPrefab;
        }
    }

    private List<Object> palette;

    private Object currentMaterialOrPrefab = null;

    private int currentHeight = 1;
    private int lastHeight = 1;
    private bool editScene = false;

    private const float floorHeight = -.25f;


    [MenuItem("Window/HexIsles/Level Editor")]
    private static void ShowWindow()
    {
        LevelEditor window = GetWindow<LevelEditor>("Level Editor");
        window.Show();

        window.minSize = new Vector2(300, 300);

        window.palette = new List<Object>(Resources.LoadAll<Material>("Terrain"));
        window.palette.AddRange(Resources.LoadAll<GameObject>("Game Elements"));
    }

    private void OnEnable() {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnGUI()
    {
        windowScrollPos = GUILayout.BeginScrollView(windowScrollPos, false, false, GUIStyle.none, GUIStyle.none);
        GUILayout.Space(10);

        OnTerrainSectionGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Delete all"))
            ClearLevel();

        GUILayout.EndScrollView();
    }

    private void OnTerrainSectionGUI()
    {
        GUIStyle assetPreviewStyle = new GUIStyle
        {
            margin = new RectOffset(5, 5, 0, 0),
            fixedHeight = 80,
            fixedWidth = 80
        };

        GUIStyle assetContainerStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fixedWidth = 90
        };

        editScene = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit Scene"), editScene);
        EditorGUILayout.HelpBox("Right click in scene view to draw, hold shift to erase, hold alt to replace, hold control to move camera", MessageType.Info);

        GUILayout.BeginHorizontal();
        layerMask = EditorGUILayout.MaskField(layerMask, System.Enum.GetNames(typeof(DrawingLayer)));


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Label("Apply materials to selected fields or select them or other game elements for drawing", EditorStyles.wordWrappedLabel);

        Object removed = null;

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();

        int maxElementsPerRow = Mathf.Max(1, (int)(position.width - 20) / (int)assetContainerStyle.fixedWidth);
        for (int i = 0; i < palette.Count; i++)
        {
            if (i % maxElementsPerRow == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            if (currentMaterialOrPrefab == palette[i])
                GUI.color = new Color(1.75f, 1.75f, 1.75f);

            GUILayout.BeginVertical(assetContainerStyle);
            GUILayout.Label(palette[i].name, EditorStyles.centeredGreyMiniLabel);
            GUI.color = Color.white;

            var tex = AssetPreview.GetAssetPreview(palette[i]);
            if (GUILayout.Button(tex, assetPreviewStyle))
            {
                currentMaterialOrPrefab = palette[i];

                var fields = Selection.GetFiltered<HexField>(SelectionMode.ExcludePrefab);
                if (fields.Length == 0)
                    continue;

                int group = 0;
                if (currentMaterialOrPrefab is Material)
                {
                    Undo.SetCurrentGroupName("Change material");
                    group = Undo.GetCurrentGroup();

                    foreach (var field in fields)
                    {
                        var renderer = field.GetComponent<Renderer>();

                        Undo.RecordObject(renderer, "Change material");
                        renderer.material = currentMaterialOrPrefab as Material;
                    }
                }
                else
                {
                    Undo.SetCurrentGroupName("Change game element");
                    group = Undo.GetCurrentGroup();

                    foreach (var field in fields)
                        AddGameElement(field, overridePrevious: true);

                    Undo.CollapseUndoOperations(group);
                }

                Undo.CollapseUndoOperations(group);
            }

            if (GUILayout.Button("Remove"))
                removed = palette[i];

            GUILayout.EndVertical();
        }

        if (removed)
        {
            palette.Remove(removed);
            if (removed == currentMaterialOrPrefab)
                currentMaterialOrPrefab = null;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();

        GUILayout.Space(5);
        GUILayout.Label("Height", EditorStyles.boldLabel);
        EditorGUILayout.EndFoldoutHeaderGroup();

        GUILayout.FlexibleSpace();

        currentHeight = EditorGUILayout.IntSlider(currentHeight, 1, 10);

        if (currentHeight != lastHeight)
        {
            var fields = Selection.GetFiltered<HexField>(SelectionMode.ExcludePrefab);
            if (fields.Length > 0)
            {
                Undo.SetCurrentGroupName("Change height");
                int group = Undo.GetCurrentGroup();

                foreach (var field in fields)
                {
                    Undo.RecordObject(field, "Change height");
                    Undo.RecordObject(field.transform, "Change height");

                    field.Height = currentHeight;
                }

                Undo.CollapseUndoOperations(group);
            }

            lastHeight = currentHeight;
        }

        GUILayout.EndHorizontal();

        GUILayout.Label("Change the brush height / the height of selected objects", EditorStyles.wordWrappedLabel);

        EditorGUILayout.EndToggleGroup();

        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        if (Event.current.type == EventType.DragExited)
        {
            var objs = new List<Object>();

            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (obj is Material || obj is GameObject)
                {
                    if (palette.Contains(obj))
                        continue;

                    objs.Add(obj);
                }
            }

            if (objs.Count != 0)
            {
                palette.AddRange(objs);
                DragAndDrop.AcceptDrag();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (focusedWindow != sceneView || !editScene || Event.current.control)
            return;

        if (currentMaterialOrPrefab != null)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 worldPos;

            if (Physics.Raycast(mouseRay, out RaycastHit hit, 100, layerMask))
            {
                worldPos = hit.transform.position;
            }
            else
            {
                float dstToFloor = (floorHeight - mouseRay.origin.y) / mouseRay.direction.y;
                worldPos = mouseRay.GetPoint(dstToFloor);
            }

            Vector2Int gridPos = GridUtility.WorldToGridPos(worldPos);

            if (Event.current.button == 1)
                if (Event.current.shift)
                {
                    if (LayerIsActive(DrawingLayer.Terrain))
                        DeleteField(gridPos);
                    else if (LayerIsActive(DrawingLayer.GameElements))
                        DeleteGameElement(gridPos);
                }
                else
                {
                    if (currentMaterialOrPrefab is Material)
                        AddField(gridPos, Event.current.alt);
                    else if (currentMaterialOrPrefab is GameObject)
                        AddGameElement(gridPos, Event.current.alt);
                }
        }

        if (Event.current.isMouse)
            Event.current.Use();
    }

    public void ClearLevel()
    {
        // Delete all fields and game elements
        Undo.SetCurrentGroupName("Clear level");
        int group = Undo.GetCurrentGroup();

        foreach (var field in GridUtility.Map.GetComponentsInChildren<HexField>())
            Undo.DestroyObjectImmediate(field.transform.parent.gameObject);

        Undo.CollapseUndoOperations(group);
    }

    public void DeleteField(Vector2Int pos)
    {
        HexField field = GridUtility.GetFieldAt(pos);
        if (field)
            Undo.DestroyObjectImmediate(field.transform.parent.gameObject);
    }

    public void DeleteField(int x, int y) => DeleteField(new Vector2Int(x, y));

    public void DeleteGameElement(Vector2Int pos)
    {
        GameObject gameElement = GetObjectInLayerAt(pos, DrawingLayer.GameElements);
        if (gameElement)
            Undo.DestroyObjectImmediate(gameElement.transform.parent.gameObject);
    }

    public void DeleteGameElement(int x, int y) => DeleteGameElement(new Vector2Int(x, y));

    private void AddField(Vector2Int pos, bool overridePrevious = false)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (field)
            if (overridePrevious)
                Undo.DestroyObjectImmediate(field.transform.parent.gameObject);
            else
                return;

        var obj = (PrefabUtility.InstantiatePrefab(HexPrefab) as Transform).gameObject;
        obj.transform.position = GridUtility.GridToWorldPos(pos);
        obj.transform.SetParent(GridUtility.Map);

        obj.name = pos.ToString();

        field = obj.GetComponentInChildren<HexField>();
        field.Position = pos;
        field.Height = currentHeight;
        field.GetComponent<Renderer>().material = currentMaterialOrPrefab as Material;
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Generate level");
    }

    private void AddField(int x, int y, bool overridePrevious = false) => AddField(new Vector2Int(x, y), overridePrevious);

    private void AddGameElement(HexField field, bool overridePrevious = false)
    {
        var prefab = (GameObject)currentMaterialOrPrefab;
        var layer = (DrawingLayer)prefab.layer;
        var obj = GetObjectInLayerAt(field, layer);
        if (obj)
            if (overridePrevious)
                Undo.DestroyObjectImmediate(obj);
            else
                return;

        obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        obj.transform.position = GridUtility.GridToWorldPos(field.Position) + (Vector3.up * field.Height * .5f);
        obj.transform.SetParent(field.transform.parent);

        if (layer == DrawingLayer.Decoration)
            obj.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 3600) / 10, Vector3.up);

        obj.name = currentMaterialOrPrefab.name;
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Add game element");
    }

    private void AddGameElement(Vector2Int pos, bool overridePrevious = false)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (field)
            AddGameElement(field, overridePrevious);
    }

    private static GameObject GetObjectInLayerAt(HexField field, DrawingLayer layer) => field.transform.parent.GetComponentsInChildren<Transform>().FirstOrDefault(obj => obj.gameObject.layer == (int)layer)?.gameObject;

    private static GameObject GetObjectInLayerAt(Vector2Int pos, DrawingLayer layer)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (!field)
            return null;
        return GetObjectInLayerAt(field, layer);
    }

    private bool LayerIsActive(DrawingLayer layer) => (layerMask & 1 << (int)layer) != 0;
}
