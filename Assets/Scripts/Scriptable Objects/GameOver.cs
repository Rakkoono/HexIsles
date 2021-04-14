using UnityEngine;

[CreateAssetMenu(fileName = "Game Over Type", menuName = "HexIsles/Game Over", order = 0)]
public class GameOver : ScriptableObject
{
    [SerializeField] private string[] messages;
    public string[] Messages => messages;

    [SerializeField] private bool unlockNextLevel;
    public bool UnlockNextLevel => unlockNextLevel;

    [SerializeField] private AudioClip sound;
    public AudioClip Sound => sound;
}