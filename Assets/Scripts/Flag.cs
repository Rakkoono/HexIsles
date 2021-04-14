using UnityEngine;
using System.Linq;

public class Flag : MonoBehaviour
{
    [Header("Requirements")]
    // Required jump height for a player to unlock this flag, if 0 any player can unlock it
    [SerializeField] private int requiredJumpHeight = 0;
    public int RequiredJumpHeight => requiredJumpHeight;

    private Vector2Int? position = null;
    public Vector2Int Position => (Vector2Int)(position = position ?? GridUtility.WorldToGridPos(transform.position));

    // Returns true if there is a player with the required jump height at the flag position
    public bool IsReached => GridUtility.GetPlayersAt(Position).Any(player => requiredJumpHeight == 0 || requiredJumpHeight == player.JumpHeight);
}