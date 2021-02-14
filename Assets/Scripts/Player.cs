using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MouseSelectable
{
    public enum MoveSet { None, AdjacentFields }

    // Serialized variables
    [SerializeField]
    MoveSet moveSet = MoveSet.AdjacentFields;

    // Hidden variables
    //[HideInInspector]
    public Vector2Int position;
    public Vector3 targetPosition;
    [HideInInspector]
    public Vector2Int[] possibleMoves = { };

    private void Start()
    {
        position = HexGrid.WorldToGridPos(transform.position);
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MouseAndPlayerHandler.Instance.playerMovementSpeed * 10f * Time.deltaTime);
    }

    public override void OnSelect()
    {
        if (MouseAndPlayerHandler.Instance.selectedPlayer && MouseAndPlayerHandler.Instance.selectedPlayer.possibleMoves.Contains(position) && position != MouseAndPlayerHandler.Instance.selectedPlayer.position)
        {
            HexField field = HexGrid.GetFieldAt(position);

            MouseAndPlayerHandler.Instance.Selected = MouseAndPlayerHandler.Instance.selectedPlayer;
            field.OnMouseDown();
            return;
        }
        // find and highlight possible moves
        switch (moveSet)
        {
            case MoveSet.AdjacentFields:
                Vector2Int[] adjacentFields = HexGrid.GetAdjacentFields(position);
                List<Vector2Int> moves = new List<Vector2Int>();
                List<HexField> fields = new List<HexField>();
                foreach(Vector2Int move in adjacentFields)
                {
                    HexField field = HexGrid.GetFieldAt(move);
                    if (!field) continue;
                    if (field.height > HexGrid.GetFieldAt(position).height + HexGrid.GetPlayersAt(position).Length) continue;

                    moves.Add(move);
                    fields.Add(field);
                    field.rend.material.SetColor("_Color", field.initialColor + MouseAndPlayerHandler.Instance.nextMoveColor);
                }
                possibleMoves = moves.ToArray();
                MouseAndPlayerHandler.Instance.colored = fields.ToArray();
                break;
        }
    }

    public override void OnDeselect()
    {
        foreach (HexField field in MouseAndPlayerHandler.Instance.colored)
            field.rend.material.SetColor("_Color", field.initialColor);
        MouseAndPlayerHandler.Instance.colored = new HexField[0];
    }

    public void Move(HexField target)
    {
        foreach (Player p in HexGrid.GetPlayersAt(position))
            if (p != this && p.transform.position.y > transform.position.y)
                p.targetPosition += .5f * Vector3.down;

        targetPosition = HexGrid.GridToWorldPos(target.position) + new Vector3(0, (target.height + HexGrid.GetPlayersAt(target.position).Length) * .5f, 0);
        position = target.position;
    }
}
