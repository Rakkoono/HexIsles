using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [HideInInspector] public MouseSelectable[] possibleMoves;
    [HideInInspector] public Player[] players;

    [HideInInspector] public List<PlayerInfo[]> undoList = new List<PlayerInfo[]>();

    // Selected object and player
    [HideInInspector] public Player selected;
    [HideInInspector] public Player lastSelected;

    private MouseSelectable selectedObject;
    public MouseSelectable SelectedObject
    {
        get => selectedObject;
        set
        {
            // Deselect last object
            if (selectedObject)
            {
                selectedObject.ResetColor();
                selectedObject.OnDeselect();
            }
            lastSelected = selected;

            // Update selectedObject
            selectedObject = value;

            // Select new object
            selectedObject?.OnSelect();
            if (selectedObject)
            {
                selectedObject.Renderer.material.color = selectedObject.InitialColor + Config.Current.SelectionTint;
                selected = selectedObject.GetComponent<Player>();
            }
            else
                selected = null;

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
        p.currentColor = p.Renderer.material.color = Config.Current.PetrifiedColor;
        p.tag = "Petrified";
    }

    public void Undo()
    {
        if (Manager.UI.currentMenu == UIHandler.Menu.GameOver)
            Manager.UI.ExitMenus();
        else if (Manager.UI.currentMenu != UIHandler.Menu.None)
            return;
        PlayerInfo[] undo = undoList.LastOrDefault();
        if (undo == default(PlayerInfo[]))
            return;

        undoList.Remove(undo);

        Manager.Current.TurnsLeft++;

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.ToggleSelect();

        Manager.Current.SfxSource.PlayOneShot(Config.Current.MoveSounds[Random.Range(0, Config.Current.MoveSounds.Length)]);

        foreach (PlayerInfo p in undo)
        {
            if (!p.IsPetrified)
            {
                p.Player.enabled = true;
                p.Player.currentColor = p.Player.Renderer.material.color = p.Player.InitialColor;
                p.Player.tag = "Untagged";
            }
            else Petrify(p.Player);

            if (p.Position == p.Player.transform.position)
                    continue;
            p.Player.position = HexGrid.WorldToGridPos(p.Position);
            p.Player.targetPosition = p.Position;
        }
    }
}
public struct PlayerInfo
{
    public PlayerInfo(Player player, Vector3 position, bool isPetrified)
    {
        Player = player;
        Position = position;
        IsPetrified = isPetrified;
    }

    public Player Player { get; }
    public Vector3 Position { get; }
    public bool IsPetrified { get; }
}