using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Serialized variables
    [Range(.1f, 10)]
    public float playerMovementSpeed = 1;
    [Space(2), Header("Colors")]
    public Color highlightTint = new Color(40, 40, 40, 10);
    public Color selectionTint = new Color(80, 80, 80, 10);
    public Color nextMoveTint = new Color(100, 100, 40, 10);
    public Color petrifiedColor = new Color(120, 120, 120);
    public Color defaultColor = new Color(255, 78, 0);

    // Hidden variables
    [HideInInspector]
    public MouseSelectable[] coloredObjects;
    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public List<Player> moved;
    [HideInInspector]
    public List<PlayerInfo[]> undoList = new List<PlayerInfo[]>();

    MouseSelectable selectedObject;
    [HideInInspector]
    public Player selected;
    public MouseSelectable SelectedObject
    {
        get => selectedObject;
        set
        {
            if (selectedObject) selected = selectedObject.GetComponent<Player>();
            else selected = null;
            selectedObject = value;
        }
    }
    public void PetrifyLonePlayers()
    {
        foreach (Player p in players)
        {
            if (HexGrid.GetPlayersAt(p.position, false).Length > 1) continue;
            bool playersNearby = false;
            foreach (Vector2Int f in HexGrid.GetAdjacentFields(p.position))
                if (HexGrid.GetPlayersAt(f, false).Length > 0)
                {
                    playersNearby = true;
                    break;
                }
            if (playersNearby) continue;
            Petrify(p);
        }
    }

    private void Petrify(Player p)
    {
        p.rend.material.SetColor("_Color", petrifiedColor);
        p.initialColor = petrifiedColor;
        p.tag = "Petrified";
    }

    public void Undo()
    {
        if (Manager.GUI.inMenu)
            return;
        PlayerInfo[] undo = undoList.LastOrDefault();
        if (undo == default(PlayerInfo[]))
            return;

        Manager.Levels.current.MovesLeft++;
        undoList.Remove(undo);

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.OnMouseDown();

        foreach (PlayerInfo p in undo)
        {
            if (!p.IsPetrified)
            {
                p.Player.enabled = true;
                p.Player.rend.material.SetColor("_Color", defaultColor);
                p.Player.initialColor = defaultColor;
                p.Player.tag = "Untagged";
            }
            else Petrify(p.Player);

            if (p.HasMoved)
            {
                if (!Manager.Players.moved.Contains(p.Player))
                    Manager.Players.moved.Add(p.Player);
            }
            else if (Manager.Players.moved.Contains(p.Player))
                Manager.Players.moved.Remove(p.Player);

            if (p.Position == p.Player.transform.position)
                    continue;
            p.Player.position = HexGrid.WorldToGridPos(p.Position);
            p.Player.targetPosition = p.Position;
        }
    }
}
public struct PlayerInfo
{
    public PlayerInfo(Player player, Vector3 position, bool isPetrified, bool hasMoved)
    {
        Player = player;
        Position = position;
        IsPetrified = isPetrified;
        HasMoved = hasMoved;
    }

    public Player Player { get; }
    public Vector3 Position { get; }
    public bool IsPetrified { get; }
    public bool HasMoved { get; }
}