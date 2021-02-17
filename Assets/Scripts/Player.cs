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
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, GameManager.instance.playerMovementSpeed * 10f * Time.deltaTime);
        else 
        {
            if (CompareTag("Petrified") && enabled == true)
            {
                enabled = false;
                if (!GameManager.instance.players.Any(p => p.enabled))
                    GameManager.instance.GameOver(GameManager.instance.allPetrifiedMSG[Random.Range(0, GameManager.instance.allPetrifiedMSG.Length)]);
            }
            if (justMoved)
            {
                if (GameManager.instance.players.All(p => GameManager.instance.playersMoved.Contains(p)))
                    GameManager.instance.lvl.TurnsLeft--;

                if (GameManager.instance && GameManager.instance.lvl.targetPosition == position && GameManager.instance.lvl.playerType == moveSet && !CompareTag("Petrified"))
                    GameManager.instance.GameOver(GameManager.instance.levelCompleteMSG[Random.Range(0, GameManager.instance.levelCompleteMSG.Length)], true);

                if (GameManager.instance.lvl.turnsLeft <= 0)
                    GameManager.instance.GameOver(GameManager.instance.outOfTurnsMSG[Random.Range(0, GameManager.instance.outOfTurnsMSG.Length)]);
                justMoved = false;
            }
        }
    }

    public override void OnSelect()
    {
        if (GameManager.instance.selectedPlayer && GameManager.instance.selectedPlayer != this && GameManager.instance.selectedPlayer.validMoves.Contains(position) && position != GameManager.instance.selectedPlayer.position)
        {
            HexField field = HexGrid.GetFieldAt(position);

            GameManager.instance.Selected = GameManager.instance.selectedPlayer;
            field.OnMouseDown();
            return;
        }

        if (!enabled || GameManager.instance.inMenu || GameManager.instance.playersMoved.Contains(this)) return;
        // find and highlight possible moves
        int jumpHeight;
        switch (moveSet)
        {
            case MoveSet.AdjacentFields:
                jumpHeight = HexGrid.GetFieldAt(position).height + 1;
                foreach (Player p in HexGrid.GetPlayersAt(position, true))
                    if (p.transform.position.y < transform.position.y) jumpHeight += p.height;
                (validMoves, GameManager.instance.coloredFields) = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), jumpHeight);
                break;
        }
    }
    
    private static (Vector2Int[], HexField[]) GetAndColorValidMoves(Vector2Int[] moves, int jumpHeight)
    {
        List<Vector2Int> testedMoves = new List<Vector2Int>();
        List<Vector2Int> validMoves = new List<Vector2Int>();
        List<HexField> coloredFields = new List<HexField>();
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
            coloredFields.Add(field);
            field.rend.material.SetColor("_Color", field.initialColor + GameManager.instance.nextMoveTint);
        }
        return (validMoves.ToArray(), coloredFields.ToArray());
    }

    public override void OnDeselect()
    {
        if (GameManager.instance.selectedPlayer == this) GameManager.instance.selectedPlayer = null;
        foreach (HexField field in GameManager.instance.coloredFields)
            field.rend.material.SetColor("_Color", field.initialColor);
    }

    public void Move(HexField target)
    {
        if (!enabled) return;
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

        GameManager.instance.playersMoved.Add(this);
        GameManager.instance.lvl.MovesLeft--;
        justMoved = true;
        //OnMouseDown();
    }
}
