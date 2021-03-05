using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "HexIsles/Level", order = 0)]
public class Level : ScriptableObject
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

    [SerializeField] private Vector2Int targetPosition;
    public Vector2Int TargetPosition => targetPosition;
}