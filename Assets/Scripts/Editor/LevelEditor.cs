using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    public enum DrawingLayer
    {
        Other = 1 << 1,
        Decoration = 1 << 2,
        Terrain = 1 << 8,
        GameElements = 1 << 9,
        Players = 1 << 10
    }

    #region GUI layout fields
    private Vector2 windowScrollPos = Vector2.zero;
    private Vector2 paletteScrollPos = Vector2.zero;

    private bool editMap = false;
    private DrawingLayer layerMask = (DrawingLayer)~0;

    private int currentHeight = 1;
    private int lastHeight = 1;

    private List<Object> palette;

    #region Lighting Settings
    private float sunAngleVertical = 45;
    private float SunAngleVertical
    {
        get => sunAngleVertical;
        set
        {
            sunAngleVertical = value;
            if (sun != null)
                sun.transform.rotation = Quaternion.Euler(sunAngleVertical, sunAngleHorizontal, 0);
        }
    }
    private float sunAngleHorizontal = 0;
    private float SunAngleHorizontal
    {
        get => sunAngleHorizontal;
        set
        {
            sunAngleHorizontal = value;
            if (sun != null)
                sun.transform.rotation = Quaternion.Euler(sunAngleVertical, sunAngleHorizontal, 0);
        }
    }

    private Color sunLightColor = new Color();
    public Color SunLightColor
    {
        get => sunLightColor;
        set
        {
            sunLightColor = value;
            if (sun != null)
                sun.color = value;
        }
    }
    #endregion
    #endregion

    private Light sun;

    private Object currentMaterialOrObject = null;

    [MenuItem("Window/HexIsles/Level Editor")]
    private static void ShowWindow()
    {
        LevelEditor window = GetWindow<LevelEditor>("Level Editor");
        window.minSize = new Vector2(300, 300);
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        palette = new List<Object>(Config.Palette);
    }

    #region GUI
    private void OnGUI()
    {
        windowScrollPos = GUILayout.BeginScrollView(windowScrollPos, false, false, GUIStyle.none, GUIStyle.none);
        GUILayout.Space(10);

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

        #region Edit Map
        editMap = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit Map"), editMap);

        GUILayout.BeginHorizontal();

        layerMask = (DrawingLayer)EditorGUILayout.EnumFlagsField("Layer Mask", layerMask);

        GUILayout.Space(400);
        if (GUILayout.Button("Delete all"))
            ClearLevel();
        GUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Right click in scene view to draw, hold shift to erase, hold alt to replace, hold control to move camera", MessageType.Info);

        GUILayout.Label("Apply materials to selected fields or select them or other game elements for drawing", EditorStyles.wordWrappedLabel);

        Object removed = null;

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();

        var selection = Selection.GetFiltered<GameObject>(SelectionMode.ExcludePrefab);
        var fields = new List<HexField>();
        var players = new List<Player>();
        foreach (var obj in selection)
        {
            var field = obj.GetComponentInChildren<HexField>();
            if (field)
            {
                fields.Add(field);
                continue;
            }

            var player = obj.GetComponentInChildren<Player>();
            if (player)
                players.Add(player);
        }

        int maxElementsPerRow = Mathf.Max(1, (int)(position.width - 20) / (int)assetContainerStyle.fixedWidth);
        for (int i = 0; i < palette.Count; i++)
        {
            if (i % maxElementsPerRow == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            if (currentMaterialOrObject == palette[i])
                GUI.color = new Color(1.75f, 1.75f, 1.75f);

            GUILayout.BeginVertical(assetContainerStyle);
            GUILayout.Label(palette[i].name, EditorStyles.centeredGreyMiniLabel);
            GUI.color = Color.white;

            var tex = AssetPreview.GetAssetPreview(palette[i]);
            if (GUILayout.Button(tex, assetPreviewStyle))
            {
                currentMaterialOrObject = palette[i];

                if (fields.Count == 0)
                    continue;

                int group = 0;
                if (currentMaterialOrObject is Material)
                {
                    Undo.SetCurrentGroupName("Change material");
                    group = Undo.GetCurrentGroup();

                    foreach (var field in fields)
                    {
                        var renderer = field.GetComponent<Renderer>();

                        Undo.RecordObject(renderer, "Change material");
                        renderer.material = currentMaterialOrObject as Material;
                    }
                }
                else
                {
                    Undo.SetCurrentGroupName("Change element");
                    group = Undo.GetCurrentGroup();

                    foreach (var field in fields)
                        AddObject(field, overridePrevious: true);

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
            if (removed == currentMaterialOrObject)
                currentMaterialOrObject = null;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (currentMaterialOrObject is Material || 1 << (currentMaterialOrObject as GameObject)?.layer == (int)DrawingLayer.Players || fields.Count > 0 || players.Count > 0)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("Height", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            currentHeight = EditorGUILayout.IntSlider(currentHeight, 1, 10);
            if (currentHeight != lastHeight)
            {
                if (fields.Count > 0 || players.Count > 0)
                {
                    Undo.SetCurrentGroupName("Change height");
                    int group = Undo.GetCurrentGroup();

                    foreach (var field in fields)
                    {
                        Undo.RecordObject(field, "Change height");
                        Undo.RecordObject(field.transform, "Change height");

                        field.Height = currentHeight;
                    }
                    foreach (var player in players)
                    {
                        Undo.RecordObject(player, "Change height");
                        Undo.RecordObject(player.transform, "Change height");

                        player.Height = currentHeight;
                    }

                    Undo.CollapseUndoOperations(group);
                }

                lastHeight = currentHeight;
            }

            GUILayout.EndHorizontal();
            GUILayout.Label("Change the brush height / the height of selected objects", EditorStyles.wordWrappedLabel);
        }

        EditorGUILayout.EndToggleGroup();
        #endregion

        bool mapExists = GameObject.Find("Map");
        // find or create sun
        if (mapExists && sun == null)
        {
            sun = (GameObject.Find("Sun") ?? (GameObject)PrefabUtility.InstantiatePrefab(Config.SunPrefab)).GetComponent<Light>();
            var euler = sun.transform.rotation.eulerAngles;
            sunAngleHorizontal = euler.y;
            sunAngleVertical = euler.x;
            sunLightColor = sun.color;
            RenderSettings.sun = sun;

            Lightmapping.lightingSettings = Config.LightingSettings;
        }

        EditorGUI.BeginDisabledGroup(!mapExists);

        GUILayout.Space(10);

        #region Lighting Settings
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Lighting", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Space(5);
        if (GUILayout.Button("Bake Current"))
            Lightmapping.BakeAsync();
        if (GUILayout.Button("Bake All"))
        {
            var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            var paths = new string[guids.Length];

            for (int i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            Lightmapping.BakeMultipleScenes(paths);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sun: Vertical Angle", GUILayout.Width(150));
        SunAngleVertical = GUILayout.HorizontalSlider(SunAngleVertical, 0, 180, GUILayout.MinWidth(350));
        SunAngleVertical = Mathf.Clamp(EditorGUILayout.FloatField(SunAngleVertical), 0, 180);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sun: Horizontal Angle", GUILayout.Width(150));
        SunAngleHorizontal = GUILayout.HorizontalSlider(SunAngleHorizontal, 0, 360, GUILayout.MinWidth(350));
        SunAngleHorizontal = Mathf.Clamp(EditorGUILayout.FloatField(SunAngleHorizontal), 0, 360);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Light Color", GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        SunLightColor = EditorGUILayout.ColorField(SunLightColor);
        GUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();

        GUILayout.EndScrollView();
        #endregion

        #region Drag & Drop
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
        #endregion
    }
    #endregion

    #region Edit Map
    private void OnSceneGUI(SceneView sceneView)
    {
        if (focusedWindow != sceneView || !editMap || Event.current.control)
            return;

        if (currentMaterialOrObject != null)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 worldPos;

            var raycast = Physics.Raycast(mouseRay, out RaycastHit hit, 100, Event.current.shift ? (int)layerMask : (int)DrawingLayer.Terrain);

            if (raycast)
            {
                worldPos = hit.transform.position;
            }
            else
            {
                float dstToFloor = (-.25f - mouseRay.origin.y) / mouseRay.direction.y;
                worldPos = mouseRay.GetPoint(dstToFloor);
            }

            Vector2Int gridPos = GridUtility.WorldToGridPos(worldPos);

            if (Event.current.button == 1)
                if (Event.current.shift)
                {
                    if (raycast)
                    {
                        // Delete on shift
                        var layer = (DrawingLayer)(1 << hit.transform.gameObject.layer);
                        if (layer == DrawingLayer.Terrain)
                            DeleteField(gridPos);
                        else
                            DeleteObject(gridPos, layer);
                    }
                }
                else
                {
                    // Else draw and replace with alt
                    if (currentMaterialOrObject is Material)
                        AddField(gridPos, Event.current.alt);
                    else if (currentMaterialOrObject is GameObject)
                        AddObject(gridPos, Event.current.alt);
                }
        }

        if (Event.current.isMouse)
            Event.current.Use();
    }

    #endregion

    public void ClearLevel()
    {
        // Delete all fields and game elements
        Undo.SetCurrentGroupName("Clear level");
        int group = Undo.GetCurrentGroup();

        foreach (var field in GridUtility.Fields)
            Undo.DestroyObjectImmediate(field.transform.parent.gameObject);

        Undo.CollapseUndoOperations(group);
    }

    #region Delete / Add Fields
    public void DeleteField(Vector2Int pos)
    {
        HexField field = GridUtility.GetFieldAt(pos);
        if (field)
            Undo.DestroyObjectImmediate(field.transform.parent.gameObject);
    }

    public void DeleteField(int x, int y) => DeleteField(new Vector2Int(x, y));

    private void AddField(Vector2Int pos, bool overridePrevious = false)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (field)
            if (overridePrevious)
                Undo.DestroyObjectImmediate(field.transform.parent.gameObject);
            else
                return;

        // Instantiate container
        var container = new GameObject(pos.ToString());
        container.transform.position = GridUtility.GridToWorldPos(pos);
        container.transform.SetParent(GridUtility.Map);
        container.layer = 11;

        // Instantiate field
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(Config.HexFieldPrefab, container.transform);
        obj.name += " " + pos.ToString();
        field = obj.GetComponent<HexField>();
        field.Position = pos;
        field.Height = currentHeight;
        field.GetComponent<Renderer>().material = currentMaterialOrObject as Material;

        Undo.RegisterCreatedObjectUndo(container.gameObject, "Add field");
    }

    private void AddField(int x, int y, bool overridePrevious = false) => AddField(new Vector2Int(x, y), overridePrevious);
    #endregion

    #region Delete / Add Objects
    public void DeleteObject(Vector2Int pos, DrawingLayer layer)
    {
        var obj = GetObjectInLayerAt(pos, layer);
        if (obj)
            Undo.DestroyObjectImmediate(obj);
    }

    private void AddObject(HexField field, bool overridePrevious = false)
    {
        var prefab = (GameObject)currentMaterialOrObject;
        var layer = (DrawingLayer)(1 << prefab.layer);
        var obj = GetObjectInLayerAt(field, layer);
        if (obj)
            if (overridePrevious)
                Undo.DestroyObjectImmediate(obj);
            else
                return;

        obj = PrefabUtility.InstantiatePrefab(prefab, field.transform.parent) as GameObject;
        obj.transform.position = GridUtility.GridToWorldPos(field.Position) + (Vector3.up * field.Height * .5f);

        switch (layer)
        {
            case DrawingLayer.Players:
                var player = obj.GetComponentInChildren<Player>();
                player.position = GridUtility.WorldToGridPos(obj.transform.position);
                player.Height = currentHeight;
                break;

            case DrawingLayer.GameElements:
                break;

            default:
                obj.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 3600) / 10, Vector3.up);
                break;
        }

        obj.name = currentMaterialOrObject.name;
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Add game element");
    }

    private void AddObject(Vector2Int pos, bool overridePrevious = false)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (field)
            AddObject(field, overridePrevious);
    }
    #endregion

    private static GameObject GetObjectInLayerAt(HexField field, DrawingLayer layer) => field.transform.parent.GetComponentsInChildren<Transform>().FirstOrDefault(obj => 1 << obj.gameObject.layer == (int)layer)?.gameObject;

    private static GameObject GetObjectInLayerAt(Vector2Int pos, DrawingLayer layer)
    {
        var field = GridUtility.GetFieldAt(pos);
        if (!field)
            return null;
        return GetObjectInLayerAt(field, layer);
    }
}
