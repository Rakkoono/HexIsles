using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    // Serialized variables
    [Space(2), Header("Animation")]
    [Range(.1f, 10)] public float playerAnimationSpeed = 1;

    [Space(2), Header("Colors")]
    public Color highlightTint = new Color(40, 40, 40, 10);
    public Color nextMoveTint = new Color(100, 100, 40, 10);
    [SerializeField] private Color selectionTint = new Color(80, 80, 80, 10);
    [SerializeField] private Color petrifiedColor = new Color(120, 120, 120);

    [Space(2), Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip[] moveSounds;
    public AudioClip levelCompleteSound;
    public AudioClip gameOverSound;

    // Hidden variables
    [HideInInspector] public MouseSelectable[] possibleMoves;
    [HideInInspector] public Player[] players;

    // Undo list
    [HideInInspector] public List<PlayerInfo[]> undoList = new List<PlayerInfo[]>();

    // Selected object and player
    [HideInInspector] public Player selected;
    private MouseSelectable selectedObject;
    public MouseSelectable SelectedObject
    {
        get => selectedObject;
        set
        {
            if (selectedObject)
            {
                selectedObject.ResetMaterial();
                selectedObject.OnDeselect();
                selected = selectedObject.GetComponent<Player>();
            }
            else selected = null;

            selectedObject = value;
            if (selectedObject)
            {
                selectedObject.OnSelect();
                selectedObject.Renderer.material.color = selectedObject.InitialColor + Manager.Players.selectionTint;
            }
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
        p.currentColor = p.Renderer.material.color = petrifiedColor;
        p.tag = "Petrified";
    }

    public void Undo()
    {
        if (Manager.UI.currentMenu != UIHandler.Menu.None)
            return;
        PlayerInfo[] undo = undoList.LastOrDefault();
        if (undo == default(PlayerInfo[]))
            return;

        undoList.Remove(undo);

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.ToggleSelect();

        sfxSource.PlayOneShot(moveSounds[Random.Range(0, moveSounds.Length)]);

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