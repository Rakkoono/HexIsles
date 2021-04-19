using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridUtility
{
    // Constants
    private static readonly Vector2 HEX_SIZE = new Vector2(0.8666666f, 1);

    private static readonly Vector2Int[] OFFSETS = new Vector2Int[4] {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };
    private static readonly Vector2Int[][] OFFSET_PAIRS = new Vector2Int[2][] {
        new Vector2Int[2] {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1)
        },
        new Vector2Int[2] {
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
        }
    };

    // Properties
    private static Vector2 HexSize => HEX_SIZE * Config.Current.GapBetweenFields + HEX_SIZE;

    private static Transform map;
    public static Transform Map
    {
        get
        {
#if !UNITY_EDITOR
            if (map == null)
#endif
            FindOrCreateMap();
            return map;
        }
    }

    public static void FindOrCreateMap()
    {
        map = (GameObject.Find("Map") ?? new GameObject("Map")).transform;
#if !UNITY_EDITOR
        fields = null;
#endif
    }

#if UNITY_EDITOR
    // Check for changes if in editor
    public static HexField[] Fields => Map.GetComponentsInChildren<HexField>();
#else
    private static HexField[] fields;
    public static HexField[] Fields => fields ??= Map.GetComponentsInChildren<HexField>();
#endif

    public static Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        var worldPos = Vector3.zero;

        worldPos.z = gridPos.y * HexSize.y * .75f;

        float offset = (gridPos.y % 2 != 0) ? HexSize.y / 2 - .066666f : 0;
        worldPos.x = gridPos.x * HexSize.x + offset;

        return worldPos;
    }

    public static Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        var gridPos = Vector2Int.zero;

        gridPos.y = Mathf.RoundToInt((worldPos.z) / .75f / HexSize.y);

        float offset = (gridPos.y % 2 != 0) ? HexSize.y / 2 - .066666f : 0;
        gridPos.x = Mathf.RoundToInt((worldPos.x - offset) / HexSize.x);

        return gridPos;
    }

    public static HexField GetFieldAt(Vector2Int pos) => Fields.FirstOrDefault(field => field.Position == pos);

    public static Player[] GetPlayersAt(Vector2Int pos, bool includePetrified = true)
    {
#if UNITY_EDITOR
        return GameObject.FindObjectsOfType<Player>().Where(player => player.position == pos && (includePetrified || player.enabled)).ToArray();
#else
        return Manager.Current.Players.Where(player => player.position == pos && (includePetrified || player.enabled)).ToArray();
#endif
    }

    public static Vector2Int[] GetAdjacentFields(Vector2Int pos, bool onlyExisting = true)
    {
        var fields = new List<Vector2Int>();
        var offsets = new List<Vector2Int>();

        foreach (var offsetPair in OFFSET_PAIRS)
            offsets.Add(offsetPair[(pos.y % 2 != 0) ? 0 : 1]);

        foreach (var offset in OFFSETS.Union(offsets))
            if (!onlyExisting || GetFieldAt(pos + offset) != null)
                fields.Add(pos + offset);

        return fields.ToArray();
    }
}