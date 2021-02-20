using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MouseSelectable
{
    public enum MoveSet { None, AdjacentFields }

    // Serialized variables
    [SerializeField]
    MoveSet moveSet = MoveSet.AdjacentFields;
    [SerializeField, Range(1, 5)]
    int height;
    public Vector2Int position;

    // Hidden variables
    [HideInInspector]
    public Vector3 targetPosition;
    [HideInInspector]
    public Vector2Int[] validMoves = { };
    private bool justMoved = false;

    private void Start() => targetPosition = transform.position;

    // Debugging Method
    [ContextMenu("Adjust Position")]
    public void AdjustPosition()
    {
        position = HexGrid.WorldToGridPos(transform.position);
        transform.position = HexGrid.GridToWorldPos(position) + (.5f * (HexGrid.GetFieldAt(position).height + height / 2) - (height % 2 == 1 ? 0 : .25f)) * Vector3.up;
        transform.localScale = new Vector3(transform.localScale.x, (float)height / 2, transform.localScale.z);
    }

    private void Update()
    {
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Manager.Players.playerMovementSpeed * 10f * Time.deltaTime);
        else 
        {
            if (CompareTag("Petrified") && enabled == true)
            {
                enabled = false;
                if (!Manager.Players.players.Any(p => p.enabled))
                    Manager.GUI.GameOver(Manager.GUI.allPetrifiedMSG[Random.Range(0, Manager.GUI.allPetrifiedMSG.Length)]);
            }
            if (justMoved)
            {
                if (Manager.Players.players.All(p => Manager.Players.moved.Contains(p)))
                    Manager.Levels.current.TurnsLeft--;

                if (Manager.Levels.current.targetPosition == position && Manager.Levels.current.playerType == moveSet && !CompareTag("Petrified"))
                    Manager.GUI.GameOver(Manager.GUI.levelCompleteMSG[Random.Range(0, Manager.GUI.levelCompleteMSG.Length)], true);

                if (Manager.Levels.current.TurnsLeft <= 0)
                    Manager.GUI.GameOver(Manager.GUI.outOfTurnsMSG[Random.Range(0, Manager.GUI.outOfTurnsMSG.Length)]);
                justMoved = false;
            }
        }
    }

    public override void OnSelect()
    {
        if (Manager.Players.coloredObjects.Contains(this) && position != Manager.Players.selected.position)
        {
            HexField field = HexGrid.GetFieldAt(position);

            Manager.Players.SelectedObject = Manager.Players.selected;
            field.OnMouseDown();
            return;
        }

        if (!enabled || Manager.GUI.inMenu || Manager.Players.moved.Contains(this)) return;
        // find and highlight possible moves
        int jumpHeight;
        switch (moveSet)
        {
            case MoveSet.AdjacentFields:
                jumpHeight = HexGrid.GetFieldAt(position).height + 1;
                foreach (Player p in HexGrid.GetPlayersAt(position, true))
                    if (p.transform.position.y < transform.position.y) jumpHeight += p.height;
                (validMoves, Manager.Players.coloredObjects) = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), jumpHeight);
                break;
        }
    }
    
    private static (Vector2Int[], MouseSelectable[]) GetAndColorValidMoves(Vector2Int[] moves, int jumpHeight)
    {
        List<Vector2Int> testedMoves = new List<Vector2Int>();
        List<Vector2Int> validMoves = new List<Vector2Int>();
        List<MouseSelectable> coloredObjects = new List<MouseSelectable>();
        foreach (Vector2Int move in moves)
        {
            if (testedMoves.Contains(move)) continue;
            testedMoves.Add(move);

            HexField field = HexGrid.GetFieldAt(move);
            int height = field ? field.height : 0;
            foreach (Player p in HexGrid.GetPlayersAt(move, true))
                height += p.height;
            if (!field || height > jumpHeight)
                continue;
            validMoves.Add(move);
            coloredObjects.Add(field);
            field.rend.material.SetColor("_Color", field.initialColor + Manager.Players.nextMoveTint);

            Player[] players = HexGrid.GetPlayersAt(move);
            coloredObjects.AddRange(players);
            foreach (Player p in players)
                p.rend.material.SetColor("_Color", p.initialColor + Manager.Players.nextMoveTint);
        }
        return (validMoves.ToArray(), coloredObjects.ToArray());
    }

    public override void OnDeselect()
    {
        if (Manager.Players.selected == this) Manager.Players.selected = null;
        foreach (MouseSelectable obj in Manager.Players.coloredObjects)
            obj.rend.material.SetColor("_Color", obj.initialColor);
    }

    public void Move(HexField target)
    {
        if (!enabled) return;

        List<PlayerInfo> undo = new List<PlayerInfo>();
        foreach (Player player in Manager.Players.players)
            undo.Add(new PlayerInfo(player, player.transform.position, player.CompareTag("Petrified"), Manager.Players.moved.Contains(player)));
        Manager.Players.undoList.Add(undo.ToArray());

        targetPosition = HexGrid.GridToWorldPos(target.position) + (.5f * (HexGrid.GetFieldAt(target.position).height + height / 2) - (height % 2 == 1 ? 0 : .25f)) * Vector3.up;
        foreach (Player p in HexGrid.GetPlayersAt(target.position))
            targetPosition.y += p.height * .5f;
        foreach (Player p in HexGrid.GetPlayersAt(position))
            if (p != this && p.transform.position.y > transform.position.y)
            {
                p.targetPosition = targetPosition + p.transform.position - transform.position;
                p.position = target.position;
            }

        position = target.position;

        
        Manager.Players.moved.Add(this);
        Manager.Levels.current.MovesLeft--;
        justMoved = true;
    }
}
