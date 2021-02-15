using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MouseSelectable
{
    public enum MoveSet { None, AdjacentFields, JumpOtherPlayers }

    // Serialized variables
    [SerializeField]
    MoveSet moveSet = MoveSet.AdjacentFields;

    // Hidden variables
    //[HideInInspector]
    public Vector2Int position;
    public Vector3 targetPosition;
    [HideInInspector]
    public Vector2Int[] validMoves = { };

    private void Start()
    {
        //position = HexGrid.WorldToGridPos(transform.position);
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, GameManager.instance.playerMovementSpeed * 10f * Time.deltaTime);
        else if (GameManager.instance && GameManager.instance.lvl.targetPosition == position && GameManager.instance.lvl.playerType == moveSet)
            GameManager.instance.GameOver("You've made it!", true);
        else if (CompareTag("Petrified") && enabled == true)
        {
            enabled = false;
            if (!FindObjectsOfType<Player>().Any(p => p.enabled))
                GameManager.instance.GameOver("EVERYONE IS DEAD!");
        }
    }

    public override void OnSelect()
    {
        if (GameManager.instance.selectedPlayer && GameManager.instance.selectedPlayer.validMoves.Contains(position) && position != GameManager.instance.selectedPlayer.position)
        {
            HexField field = HexGrid.GetFieldAt(position);

            GameManager.instance.Selected = GameManager.instance.selectedPlayer;
            field.OnMouseDown();
            return;
        }

        if (!enabled || GameManager.instance.inMenu || GameManager.instance.movedPlayers.Contains(this)) return;
        // find and highlight possible moves
        switch (moveSet)
        {
            case MoveSet.AdjacentFields:
                (validMoves, GameManager.instance.coloredFields) = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), position);
                break;
            case MoveSet.JumpOtherPlayers:
                (validMoves, GameManager.instance.coloredFields) = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), position, true);
                break;
        }
    }
    
    private static (Vector2Int[], HexField[]) GetAndColorValidMoves(Vector2Int[] moves, Vector2Int position, bool jump = false)
    {
        int jumpHeight = HexGrid.GetFieldAt(position).height + HexGrid.GetPlayersAt(position, true).Length;
        List<Vector2Int> testedMoves = new List<Vector2Int>();
        List<Vector2Int> validMoves = new List<Vector2Int>();
        List<HexField> coloredFields = new List<HexField>();
        foreach (Vector2Int move in moves)
        {
            if (testedMoves.Contains(move)) continue;
            testedMoves.Add(move);

            HexField field = HexGrid.GetFieldAt(move);
            if (!field || field.height > jumpHeight)
                continue;
            validMoves.Add(move);
            coloredFields.Add(field);
            field.rend.material.SetColor("_Color", field.initialColor + GameManager.instance.nextMoveTint);

            if (jump && HexGrid.GetPlayersAt(move, true).Length > 0)
            {
                Debug.Log("Players At " + move);
                Vector2Int[] m; HexField[] f;
                (m, f) = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), move);
                Debug.Log(m.Length);
                validMoves.AddRange(m); coloredFields.AddRange(f);
            }
        }
        return (validMoves.ToArray(), coloredFields.ToArray());
    }

    public override void OnDeselect()
    {
        foreach (HexField field in GameManager.instance.coloredFields)
            field.rend.material.SetColor("_Color", field.initialColor);
    }

    public void Move(HexField target)
    {
        if (!enabled) return;
        foreach (Player p in HexGrid.GetPlayersAt(position))
            if (p != this && p.transform.position.y > transform.position.y)
                p.targetPosition += .5f * Vector3.down;

        targetPosition = HexGrid.GridToWorldPos(target.position) + new Vector3(0, (target.height + HexGrid.GetPlayersAt(target.position, true).Length) * .5f, 0);
        position = target.position;

        GameManager.instance.movedPlayers.Add(this);

        GameManager.instance.lvl.MovesLeft--;

        GameManager.instance.lightRotationAngle += 180 / GameManager.instance.lvl.movesPerTurn;
        GameManager.instance.lightRotationAngle %= 360;
        if (GameManager.instance.lightRotationAngle > 180)
            GameManager.instance.extra90deg += 2;
    }
}
