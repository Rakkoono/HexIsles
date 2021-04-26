using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level_0", menuName = "HexIsles/Level Data File", order = 0)]
public class LevelData : ScriptableObject
{
    [Header("Presentation")]
    [SerializeField] private string displayName = "";
    public string DisplayName => displayName;

    [SerializeField] private Sprite previewImage;
    public Sprite PreviewImage => previewImage;

    [Space(2), Header("Settings")]
    [SerializeField] private bool petrify = true;
    public bool Petrify => petrify;

    [SerializeField] private int turns = 1;
    public int Turns => turns;
}