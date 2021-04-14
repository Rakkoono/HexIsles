using UnityEngine;

[CreateAssetMenu(fileName = "Config File", menuName = "HexIsles/Config File", order = 0)]
public class Config : SingletonScriptableObject<Config>
{
    [SerializeField] private Level[] levels = new Level[15];
    public Level[] Levels => levels;

#region Game Over Types

    [Space(2), Header("Game Over Types")]

    [SerializeField] private GameOver levelComplete;
    public GameOver LevelComplete => levelComplete;

    [SerializeField] private GameOver outOfTurns;
    public GameOver OutOfTurns => outOfTurns;

    [SerializeField] private GameOver allPetrified;
    public GameOver AllPetrified => allPetrified;

#endregion
#region Camera Settings

    [Space(2), Header("Camera Settings")]

    [SerializeField, Range(0f, 10f)] public float zoomSpeed = 5f;

    [SerializeField, Range(0f, 200f)] public float panSpeed = 100f;

    [Space(2)]

    [SerializeField] private float zoomRangeMin = 1f;
    public float ZoomRangeMin => zoomRangeMin;

    [SerializeField] private float zoomRangeMax = 8f;
    public float ZoomRangeMax => zoomRangeMax;

#endregion
#region Player

    [Space(2), Header("Player")]

    [Range(.1f, 10)] public float playerAnimationSpeed = 1;

    [Space(2), Header("Colors")]

    [SerializeField] private Color highlightTint = new Color(40, 40, 40, 10);
    public Color HighlightTint => highlightTint;

    [SerializeField] private Color movePreviewTint = new Color(100, 100, 40, 10);
    public Color MovePreviewTint => movePreviewTint;

    [SerializeField] private Color selectionTint = new Color(80, 80, 80, 10);
    public Color SelectionTint => selectionTint;

    [SerializeField] private Color petrifiedColor = new Color(120, 120, 120);
    public Color PetrifiedColor => petrifiedColor;

#endregion
#region Audio

    [Space(2), Header("Audio")]

    [SerializeField] private AudioClip music;
    public AudioClip Music => music;
    
    [SerializeField] private AudioClip[] moveSounds;
    public AudioClip[] MoveSounds => moveSounds;

#endregion
#region Dialogs

    [Space(2), Header("Dialogs")]

    [SerializeField, Range(0, 5)] private float letterAnimationTime;
    public float LetterAnimationTime => letterAnimationTime;

    [SerializeField] private Dialog startUpDialog;
    public Dialog StartUpDialog => startUpDialog;

    [Space(2), Header("Editor")]

    [SerializeField, Range(0f, 1f)] private float gapBetweenFields = 0.05f;
    public float GapBetweenFields => gapBetweenFields;

#endregion
}