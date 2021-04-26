using UnityEngine;

[CreateAssetMenu(fileName = "GameOver", menuName = "HexIsles/Game Over Data File", order = 0)]
public class GameOverData : ScriptableObject
{
    [SerializeField] private string[] messages;
    public string[] Messages => messages;

    [SerializeField] private bool unlockNextLevel;
    public bool UnlockNextLevel => unlockNextLevel;

    [SerializeField] private AudioClip sound;
    public AudioClip Sound => sound;
}