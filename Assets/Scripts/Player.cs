using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class Player : MouseSelectable
{
    public enum MoveSet { None, AdjacentFields }

    // Serialized fields
    [SerializeField] MoveSet moveSet = MoveSet.AdjacentFields;
    [SerializeField, Range(1, 5)] int height = 1;
    [SerializeField, Range(1, 3)] int jump = 1;

    // Hidden fields
    [HideInInspector] public Color currentColor;
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public Vector3 targetPosition;
    private bool justMoved = false;

    private void Start()
    {
#if UNITY_EDITOR
        if (!FindObjectOfType<Manager>())
        {
            SceneManager.LoadScene(0);
            return;
        }

#endif
        targetPosition = transform.position;
        currentColor = InitialColor;
    }

#if UNITY_EDITOR
    [ContextMenu("Adjust Position")]
    public void AdjustPosition()
    {
        position = GridUtility.WorldToGridPos(transform.position);
        transform.position = GridUtility.GridToWorldPos(position) + (.5f * (GridUtility.GetFieldAt(position).Height + height / 2) - (height % 2 != 0 ? 0 : .25f)) * Vector3.up;
        
        transform.localScale = new Vector3(transform.localScale.x, (float)height / 2, transform.localScale.z);
    }
#endif

    private void Update()
    {
        // Move to target position
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Config.Current.playerAnimationSpeed * 10f * Time.deltaTime);
        else if (justMoved)
            {
                // When finished moving...
                justMoved = false;
                
                // Check if petrified
                foreach(var player in Manager.Players.players)
                    if (player.CompareTag("Petrified") && player.enabled)
                    player.enabled = false;

                // Check for game over
                if (Manager.Players.players.All(p => !p.enabled))
                    Manager.UI.ShowGameOver(Config.Current.AllPetrified);

                else if (Manager.Current.level.TargetPosition == position)
                    Manager.UI.ShowGameOver(Config.Current.LevelComplete);

                else if (Manager.Current.TurnsLeft <= 0)
                    Manager.UI.ShowGameOver(Config.Current.OutOfTurns);
            }
    }

    public override void OnSelect()
    {
        if (Manager.Players.lastSelected && Manager.Players.possibleMoves.Contains(this) && position != Manager.Players.lastSelected.position)
        {
            HexField field = GridUtility.GetFieldAt(position);

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
                jumpHeight = GridUtility.GetFieldAt(position).Height + jump;
                foreach (var p in GridUtility.GetPlayersAt(position, true))
                    if (p.transform.position.y < transform.position.y) jumpHeight += p.height;
                Manager.Players.possibleMoves = GetAndColorValidMoves(GridUtility.GetAdjacentFields(position), jumpHeight);
                break;
        }
    }
    
    private static MouseSelectable[] GetAndColorValidMoves(Vector2Int[] moves, int jumpHeight)
    {
        var testedMoves = new List<Vector2Int>();
        var validMoves = new List<MouseSelectable>();

        foreach (var move in moves)
        {
            if (testedMoves.Contains(move)) continue;
            testedMoves.Add(move);

            HexField field = GridUtility.GetFieldAt(move);
            int height = field ? field.Height : 0;
            foreach (var p in GridUtility.GetPlayersAt(move, true))
                height += p.height;
            if (!field || height > jumpHeight)
                continue;
            validMoves.Add(field);
            field.Renderer.material.color = field.InitialColor + Config.Current.MovePreviewTint;

            Player[] players = GridUtility.GetPlayersAt(move);
            validMoves.AddRange(players);
            foreach (var p in players)
                p.Renderer.material.color = p.InitialColor + Config.Current.MovePreviewTint;
        }
        return validMoves.ToArray();
    }

    public override void OnDeselect()
    {
        if (Manager.Players.lastSelected == this) Manager.Players.lastSelected = null;
        foreach (var obj in Manager.Players.possibleMoves)
            obj.ResetColor();
    }

    public void Move(HexField target)
    {
        if (!enabled) return;

        var undo = new List<PlayerState>();
        foreach (var player in Manager.Players.players)
            undo.Add(new PlayerState(player, player.transform.position, player.CompareTag("Petrified")));
        Manager.Players.undoStack.Push(undo.ToArray());

        targetPosition = GridUtility.GridToWorldPos(target.Position) + (.5f * (GridUtility.GetFieldAt(target.Position).Height + height / 2) - (height % 2 != 0 ? 0 : .25f)) * Vector3.up;
        foreach (var p in GridUtility.GetPlayersAt(target.Position))
            targetPosition.y += p.height * .5f;
        foreach (var p in GridUtility.GetPlayersAt(position))
            if (p != this && p.transform.position.y > transform.position.y)
            {
                p.targetPosition = targetPosition + p.transform.position - transform.position;
                p.position = target.Position;
            }


        position = target.Position;

        if (Manager.Current.level.Petrify)
            Manager.Players.PetrifyLonePlayers();

        Manager.Current.TurnsLeft--;
        justMoved = true;
        Manager.Current.SfxSource.PlayOneShot(Config.Current.MoveSounds[Random.Range(0, Config.Current.MoveSounds.Length)]);
        // Player automatically gets selected after every move // ToggleSelect();
    }
}
