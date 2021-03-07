using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MouseSelectable
{
    public enum MoveSet { None, AdjacentFields }

    // Serialized variables
    [SerializeField] MoveSet moveSet = MoveSet.AdjacentFields;
    [SerializeField, Range(1, 5)] int height = 1;
    [SerializeField, Range(1, 3)] int jump = 1;

    // Hidden variables
    [HideInInspector] public Color currentColor;
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public Vector3 targetPosition;
    private bool justMoved = false;

    private void Start()
    {
        targetPosition = transform.position;
        currentColor = InitialColor;
    }

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
        // Move to target position
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Manager.Players.playerAnimationSpeed * 10f * Time.deltaTime);
        else if (justMoved)
            {
                // When finished moving...
                justMoved = false;
                
                // Check if petrified
                foreach(Player player in Manager.Players.players)
                    if (player.CompareTag("Petrified") && player.enabled)
                    player.enabled = false;

                // Check for game over
                if (Manager.Players.players.All(p => !p.enabled))
                    Manager.UI.GameOver(Config.Instance.AllPetrified);

                else if (Manager.Levels.current.TargetPosition == position)
                    Manager.UI.GameOver(Config.Instance.LevelComplete);

                else if (Manager.TurnsLeft <= 0)
                    Manager.UI.GameOver(Config.Instance.OutOfTurns);
            }
    }

    public override void OnSelect()
    {
        if (Manager.Players.lastSelected && Manager.Players.possibleMoves.Contains(this) && position != Manager.Players.lastSelected.position)
        {
            HexField field = HexGrid.GetFieldAt(position);

            Manager.Players.SelectedObject = Manager.Players.lastSelected;
            field.ToggleSelect();
            return;
        }

        if (!enabled || Manager.UI.currentMenu != UIHandler.Menu.None)
            return;
        
        // find and highlight possible moves
        int jumpHeight;
        switch (moveSet)
        {
            case MoveSet.AdjacentFields:
                jumpHeight = HexGrid.GetFieldAt(position).height + jump;
                foreach (Player p in HexGrid.GetPlayersAt(position, true))
                    if (p.transform.position.y < transform.position.y) jumpHeight += p.height;
                Manager.Players.possibleMoves = GetAndColorValidMoves(HexGrid.GetAdjacentFields(position), jumpHeight);
                break;
        }
    }
    
    private static MouseSelectable[] GetAndColorValidMoves(Vector2Int[] moves, int jumpHeight)
    {
        List<Vector2Int> testedMoves = new List<Vector2Int>();
        List<MouseSelectable> validMoves = new List<MouseSelectable>();

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
            validMoves.Add(field);
            field.Renderer.material.color = field.InitialColor + Manager.Players.nextMoveTint;

            Player[] players = HexGrid.GetPlayersAt(move);
            validMoves.AddRange(players);
            foreach (Player p in players)
                p.Renderer.material.color = p.InitialColor + Manager.Players.nextMoveTint;
        }
        return validMoves.ToArray();
    }

    public override void OnDeselect()
    {
        if (Manager.Players.lastSelected == this) Manager.Players.lastSelected = null;
        foreach (MouseSelectable obj in Manager.Players.possibleMoves)
            obj.ResetColor();
    }

    public void Move(HexField target)
    {
        if (!enabled) return;

        List<PlayerInfo> undo = new List<PlayerInfo>();
        foreach (Player player in Manager.Players.players)
            undo.Add(new PlayerInfo(player, player.transform.position, player.CompareTag("Petrified")));
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

        if (Manager.Levels.current.Petrify)
            Manager.Players.PetrifyLonePlayers();

        Manager.TurnsLeft--;
        justMoved = true;
        Manager.Players.sfxSource.PlayOneShot(Manager.Players.moveSounds[Random.Range(0, Manager.Players.moveSounds.Length)]);
        // Player automatically gets selected after every move // ToggleSelect();
    }
}
