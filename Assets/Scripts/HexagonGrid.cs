using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class HexagonGrid : MonoBehaviour
{
    //Serialized Variables
    [SerializeField] public Transform prefab;
    [Space]
    [SerializeField] private Vector2Int size = new Vector2Int(9, 9);
    [SerializeField] private float gap = 0f;
    public List<Vector2Int> deleted = new List<Vector2Int>();

    //Hidden Variables
    private Vector2 calculatedSize = new Vector2();
    private Vector3 startPos = Vector3.zero;

    [HideInInspector]
    public Dictionary<Vector2Int, HexagonField> fields = new Dictionary<Vector2Int, HexagonField>();

    private Vector2Int lastSize = new Vector2Int(9, 9);
    private List<Vector2Int> lastDeleted = new List<Vector2Int>();

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
    static HexagonGrid instance;
    public static HexagonGrid Instance
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

    // Debugging Method
    [InitializeOnLoadMethod]
    public static void FindAndInitializeInstance()
    {
        Instance = FindObjectOfType<HexagonGrid>();
        Instance.Initialize();
    }

    [ContextMenu("Reload")]
    public void Initialize()
    {
        if (deleted == null)
            deleted = new List<Vector2Int>();

        if (fields == null)
            fields = new Dictionary<Vector2Int, HexagonField>();

        if (fields.Count == 0)
            foreach (HexagonField field in GetComponentsInChildren<HexagonField>())
                fields.Add(field.position, field);

        instance = this;
    }

    // Debugging Method
    [ContextMenu("Clear")]
    private void Clear()
    {
        // Destroy all fields
        foreach (HexagonField field in fields.Values)
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
        fields.Clear();
    }

    // Debugging Method
    [ContextMenu("Reset deleted")]
    private void ResetDeleted()
    {
        // Regenerate all fields
        deleted.Clear();
        Generate();
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

    // Debugging Method
    private void Create()
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                if (!deleted.Contains(gridPos))
                {
                    Transform hex = Instantiate(prefab, GridToWorldPos(gridPos), Quaternion.identity, transform);
                    hex.name = "Field " + gridPos.ToString();

                    HexagonField field = hex.GetComponent<HexagonField>();
                    field.position = gridPos;
                
                    fields.Add(gridPos, field);
                }
            }
    }
    
    // Public methods

    public Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        Vector3 worldPos = Vector3.zero;

        worldPos.z = gridPos.y * calculatedSize.y * .75f + startPos.z;

        float offset = (gridPos.y % 2 == 1) ? calculatedSize.y / 2 - .066666f : 0;
        worldPos.x = gridPos.x * calculatedSize.x + startPos.x + offset;

        return worldPos;
    }

    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        Vector2Int gridPos = Vector2Int.zero;

        gridPos.y = (int)((worldPos.z - startPos.z) / .75f / calculatedSize.y);

        float offset = (gridPos.y % 2 == 1) ? calculatedSize.y / 2 - .066666f : 0;
        gridPos.x = (int)((worldPos.x  - offset - startPos.x) / calculatedSize.x);

        return gridPos;
    }

    public HexagonField TryGetFieldAt(Vector2Int pos)
    {
        fields.TryGetValue(pos, out HexagonField field);
        return field;
    }

    public GameObject[] TryGetObjectsAt(Vector2Int pos)
    {
        HexagonField field = TryGetFieldAt(pos);
        if (field == null) return null;
        else return field.objects.ToArray();
    }

    public List<Vector2Int> GetAdjacentFields(Vector2Int pos, bool OnlyExisting = true)
    {
        List<Vector2Int> fields = new List<Vector2Int>();

        List<Vector2Int> offsets = new List<Vector2Int>();
        foreach (Vector2Int[] offsetPair in OFFSET_PAIRS)
            offsets.Add(offsetPair[(pos.y % 2 == 1) ? 0 : 1]);

        foreach (Vector2Int offset in OFFSETS.Union(offsets))
            if (((pos + offset).x < size.x && (pos + offset).y < size.y && !deleted.Contains(pos + offset)) || !OnlyExisting)
                fields.Add(pos + offset);

        return fields;
    }
}