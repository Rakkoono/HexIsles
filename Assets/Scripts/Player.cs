using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class Player : MouseSelectable
{
    #region Hidden fields
    [HideInInspector] public Vector2Int position;
    [HideInInspector] public Vector3 targetPosition;
    private bool justMoved = false;

    [SerializeField, Range(0, 3)] private int jump = 1;
    public int JumpHeight => jump;

    [SerializeField] private int height = 1;
    public int Height
    {
        get => height;
        set
        {
            height = value;
            transform.localPosition = new Vector3(0, .5f * GridUtility.GetFieldAt(position).Height + .25f * height - .25f, 0);
            transform.localScale = new Vector3(transform.localScale.x, .5f * height, transform.localScale.x);
        }
    }

    // Is this object a possible target for the last selected player?
    public bool IsTarget => Manager.Current.LastSelectedPlayer && Manager.Current.validTargets.Contains(this);

    public int TotalJumpHeight
    {
        get
        {
            int jumpHeight = GridUtility.GetFieldAt(position).Height + jump;
            foreach (var player in GridUtility.GetPlayersAt(position, true))
                if (player.transform.position.y < transform.position.y)
                    jumpHeight += player.height;

            return jumpHeight;
        }
    }

    private Color initialColor;
    private bool isPetrified = false;
    public bool IsPetrified
    {
        get => isPetrified;
        set
        {
            isPetrified = value;
            Color = Renderer.material.color = value ? Config.Current.PetrifiedColor : initialColor;
        }
    }
    #endregion

    private void Start()
    {
#if UNITY_EDITOR
        if (!FindObjectOfType<Manager>() && FindObjectOfType<Player>() == this)
        {
            SceneManager.LoadScene(0);
            return;
        }
#endif
        targetPosition = transform.position;
        initialColor = Color;
    }

    private void Update()
    {
        // Move to target position
        if (transform.position != targetPosition)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Config.Current.playerAnimationSpeed * 10f * Time.deltaTime);
        else if (justMoved)
        {
            // When finished moving...
            justMoved = false;

            // Check for game over
            if (Manager.Current.Players.All(player => player.IsPetrified))
                Manager.Current.ShowGameOver(Config.Current.AllPetrified);

            else if (Manager.Current.Flags.All(flag => flag.IsReached))
                Manager.Current.ShowGameOver(Config.Current.LevelComplete);

            else if (Manager.Current.TurnsLeft <= 0)
                Manager.Current.ShowGameOver(Config.Current.OutOfTurns);
        }
    }

    #region Select / Deselect
    public override void OnSelect()
    {
        if (Manager.Current.menu != Menu.None)
            return;

        // if this player is a target for the last selected player move last selected to this field
        if (IsTarget)
        {
            Manager.Current.LastSelectedPlayer.MoveTo(this);
            return;
        }

        if (IsPetrified)
            return;

        // find and highlight possible moves
        var moves = GridUtility.GetAdjacentFields(position);
        Manager.Current.validTargets = GetAndColorValidTargets(moves, TotalJumpHeight);
    }

    private static MouseSelectable[] GetAndColorValidTargets(Vector2Int[] positions, int jumpHeight)
    {
        var valid = new List<MouseSelectable>();
        foreach (var pos in positions)
        {
            HexField field = GridUtility.GetFieldAt(pos);

            if (field == null)
                continue;

            int height = field.Height;
            var players = GridUtility.GetPlayersAt(pos);
            foreach (var player in players)
                height += player.height;

            if (height > jumpHeight)
                continue;

            valid.Add(field);
            field.Renderer.material.color = field.Color + Config.Current.MovePreviewTint;

            valid.AddRange(players);
            foreach (var player in players)
                player.Renderer.material.color = player.Color + Config.Current.MovePreviewTint;
        }

        return valid.ToArray();
    }

    public override void OnDeselect()
    {
        if (Manager.Current.LastSelectedPlayer == this) Manager.Current.LastSelectedPlayer = null;
        foreach (var obj in Manager.Current.validTargets)
            obj.ResetColor();
    }
    #endregion

    #region Move
    public void MoveTo(HexField field)
    {
        if (!enabled) return;

        var undo = new List<PlayerState>();
        foreach (var player in Manager.Current.Players)
            undo.Add(new PlayerState(player, player.transform.position, player.IsPetrified));
        Manager.Current.UndoStack.Push(undo.ToArray());
        targetPosition = GridUtility.GridToWorldPos(field.Position) + (.5f * field.Height + .25f * Height - .25f) * Vector3.up;
        foreach (var player in GridUtility.GetPlayersAt(field.Position))
            if (player != this)
                targetPosition.y += player.height * .5f;
        foreach (var player in GridUtility.GetPlayersAt(position))
            if (player != this && player.transform.position.y > transform.position.y)
            {
                player.targetPosition = targetPosition + player.transform.position - transform.position;
                player.position = field.Position;
            }

        position = field.Position;

        if (Manager.Current.Level.Petrify)
            Manager.Current.PetrifyLonePlayers();

        Manager.Current.TurnsLeft--;
        justMoved = true;
        Manager.Current.SfxSource.PlayOneShot(Config.Current.MoveSounds[Random.Range(0, Config.Current.MoveSounds.Length)]);
        Manager.Current.SelectedObject = null;
        // Player automatically gets selected after every move: 
        //// ToggleSelect();
    }

    public void MoveTo(Player player)
    {
        Manager.Current.SelectedObject = Manager.Current.SelectedPlayer;
        MoveTo(GridUtility.GetFieldAt(player.position));
    }
    #endregion
}
