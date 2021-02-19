using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class HexGrid : MonoBehaviour
{
    //Serialized variables
    public Transform prefab;
    [Space]
    [SerializeField] private Vector2Int size = new Vector2Int(9, 9);
    [SerializeField] private float gap = 0f;

    //Hidden variables
    private Vector2 calculatedSize;
    private Vector3 startPos;

    // Constants
    private static readonly Vector2 HEX_SIZE = new Vector2(0.8666666f, 1);

    private static readonly Vector2Int[] OFFSETS = new Vector2Int[4] {
        new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0)
    };
    private static readonly Vector2Int[][] OFFSET_PAIRS = new Vector2Int[2][] {
        new Vector2Int[2] { new Vector2Int(1, 1), new Vector2Int(-1, 1) },
        new Vector2Int[2] { new Vector2Int(1, -1), new Vector2Int(-1, -1) }
    };

    // Singleton instance
    static HexGrid instance;
    public static HexGrid Instance
    {
        get => instance;
        private set
        {
            if (instance)
                Debug.LogWarning("Singleton class HexagonGrid already exists!");
            else
                instance = value;
        }
    }

    // Debugging method
    public static void FindAndInitializeInstance()
    {
        Instance = FindObjectOfType<HexGrid>();
        if (Instance) Instance.Initialize();
    }
    private void Start()
    {
        if (!FindObjectOfType<Manager>()) { SceneManager.LoadScene(0); return; }
        Initialize();
        foreach (Player p in Manager.Players.players)
            p.position = WorldToGridPos(p.transform.position);
    }

    [ContextMenu("Reload")]
    public void Initialize()
    {
        instance = this;
        Calculate();
    }

    // Debugging method
    [ContextMenu("Clear")]
    private void Clear()
    {
        // Destroy all fields
        foreach (HexField field in GetComponentsInChildren<HexField>())//fields.Values)
        {
            if (field)
            {
#if UNITY_EDITOR
                DestroyImmediate(field.gameObject);
#else
                Destroy(field.gameObject);
#endif
            }
        }
    }

    // Debugging Method
    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();
        Calculate();
        Create();
    }

    // Debugging Method
    private void Calculate()
    {
        // Calculate field size & start position
        calculatedSize = HEX_SIZE + HEX_SIZE * gap;

        float offset = (size.y % 2 == 1) ? calculatedSize.y / 2 : 0;

        startPos = new Vector3(
            size.x / 2 * -calculatedSize.x - offset,
            0,
            size.y / 2 * -calculatedSize.y * .75f
        );
    }

    // Debugging method
    private void Create()
    {
        // Instantiate the fields
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Transform hex = Instantiate(prefab, GridToWorldPos(gridPos), Quaternion.identity, transform);
                hex.name = "Field " + gridPos.ToString();

                HexField field = hex.GetComponent<HexField>();
                field.position = gridPos;
            }
    }

    //
    // Utility methods
    //

    public static Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        Vector3 worldPos = Vector3.zero;

        worldPos.z = gridPos.y * Instance.calculatedSize.y * .75f + Instance.startPos.z;

        float offset = (gridPos.y % 2 == 1) ? Instance.calculatedSize.y / 2 - .066666f : 0;
        worldPos.x = gridPos.x * Instance.calculatedSize.x + Instance.startPos.x + offset;
        return worldPos;
    }

    public static Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        Vector2Int gridPos = Vector2Int.zero;

        gridPos.y = Mathf.RoundToInt((worldPos.z - Instance.startPos.z) / .75f / Instance.calculatedSize.y);

        float offset = (gridPos.y % 2 == 1) ? Instance.calculatedSize.y / 2 - .066666f : 0;
        gridPos.x = Mathf.RoundToInt((worldPos.x - offset - Instance.startPos.x) / Instance.calculatedSize.x);
        return gridPos;
    }

    public static HexField GetFieldAt(Vector2Int pos)
        => Instance.GetComponentsInChildren<HexField>().FirstOrDefault(f => f.position == pos);

    public static Player[] GetPlayersAt(Vector2Int pos, bool includePetrified = true)
    {
        if (Manager.Players)
            return Manager.Players.players.Where(p => p.position == pos && (includePetrified || p.enabled)).ToArray();
        else
            return FindObjectsOfType<Player>().Where(p => p.position == pos && (includePetrified || p.enabled)).ToArray();

    }


    public static Vector2Int[] GetAdjacentFields(Vector2Int pos, bool OnlyExisting = true)
    {
        List<Vector2Int> fields = new List<Vector2Int>();

        List<Vector2Int> offsets = new List<Vector2Int>();
        foreach (Vector2Int[] offsetPair in OFFSET_PAIRS)
            offsets.Add(offsetPair[(pos.y % 2 == 1) ? 0 : 1]);

        foreach (Vector2Int offset in OFFSETS.Union(offsets))
        {
            if (!OnlyExisting
             || ((pos + offset).x < Instance.size.x
             && (pos + offset).y < Instance.size.y
             && (pos + offset).x >= 0
             && (pos + offset).y >= 0))
                fields.Add(pos + offset);
        }

        return fields.ToArray();
    }
}