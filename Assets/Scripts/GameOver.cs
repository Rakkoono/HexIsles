using UnityEngine;

[CreateAssetMenu(fileName = "Game Over Type", menuName = "HexIsles/GameOver", order = 0)]
public class GameOver : ScriptableObject
{
    [SerializeField] private string[] messages;
    public string[] Messages => messages;

    [SerializeField] private bool unlockNextLevel;
    public bool UnlockNextLevel => unlockNextLevel;
}