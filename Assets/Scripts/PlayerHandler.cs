using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [HideInInspector] public MouseSelectable[] possibleMoves;
    [HideInInspector] public Player[] players;

    [HideInInspector] public Stack<PlayerState[]> undoStack = new Stack<PlayerState[]>();

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
        foreach (var p in players)
        {
            if (GridUtility.GetPlayersAt(p.position, false).Length > 1) continue;
            bool playersNearby = false;
            foreach (var f in GridUtility.GetAdjacentFields(p.position))
                if (GridUtility.GetPlayersAt(f, false).Length > 0)
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

        if (undoStack.Count == 0)
            return;

        PlayerState[] undo = undoStack.Pop();
        if (undo == default(PlayerState[]))
            return;


        Manager.Current.TurnsLeft++;

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.ToggleSelect();

        Manager.Current.SfxSource.PlayOneShot(Config.Current.MoveSounds[Random.Range(0, Config.Current.MoveSounds.Length)]);

        foreach (var p in undo)
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
            p.Player.position = GridUtility.WorldToGridPos(p.Position);
            p.Player.targetPosition = p.Position;
        }
    }
}
public struct PlayerState
{
    public PlayerState(Player player, Vector3 position, bool isPetrified)
    {
        Player = player;
        Position = position;
        IsPetrified = isPetrified;
    }

    public Player Player { get; }
    public Vector3 Position { get; }
    public bool IsPetrified { get; }
}