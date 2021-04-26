using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Config", menuName = "HexIsles/Config File", order = 0)]
public class Config : SingletonScriptableObject<Config>
{
    [SerializeField] private LevelData[] levels = new LevelData[15];
    public static LevelData[] Levels => Instance.levels;

    #region Game Over Types

    [Space(2), Header("Game Over Types")]

    [SerializeField] private GameOverData levelComplete;
    public static GameOverData LevelComplete => Instance.levelComplete;

    [SerializeField] private GameOverData outOfTurns;
    public static GameOverData OutOfTurns => Instance.outOfTurns;

    [SerializeField] private GameOverData allPetrified;
    public static GameOverData AllPetrified => Instance.allPetrified;

    #endregion
    #region Camera Settings

    [Space(2), Header("Camera Settings")]

    [SerializeField, Range(0f, 10f)] private float zoomSpeed = 5f;
    public static float ZoomSpeed => Instance.zoomSpeed;

    [SerializeField, Range(0f, 200f)] private float panSpeed = 100f;
    public static float PanSpeed => Instance.panSpeed;

    [Space(2)]

    [SerializeField] private float zoomRangeMin = 1f;
    public static float ZoomRangeMin => Instance.zoomRangeMin;

    [SerializeField] private float zoomRangeMax = 8f;
    public static float ZoomRangeMax => Instance.zoomRangeMax;

    #endregion
    #region Player

    [Space(2), Header("Player")]

    [Range(.1f, 10)] private float playerAnimationSpeed = 1;
    public static float PlayerAnimationSpeed => Instance.playerAnimationSpeed;

    [Space(2), Header("Colors")]

    [SerializeField] private Color highlightTint = new Color(40, 40, 40, 10);
    public static Color HighlightTint => Instance.highlightTint;

    [SerializeField] private Color movePreviewTint = new Color(100, 100, 40, 10);
    public static Color MovePreviewTint => Instance.movePreviewTint;

    [SerializeField] private Color selectionTint = new Color(80, 80, 80, 10);
    public static Color SelectionTint => Instance.selectionTint;

    [SerializeField] private Color petrifiedColor = new Color(120, 120, 120);
    public static Color PetrifiedColor => Instance.petrifiedColor;

    #endregion
    #region Audio

    [Space(2), Header("Audio")]

    [SerializeField] private AudioMixer audioMixer;
    public static AudioMixer AudioMixer => Instance.audioMixer;

    [SerializeField] private AudioClip music;
    public static AudioClip Music => Instance.music;

    [SerializeField] private AudioClip[] moveSounds;
    public static AudioClip[] MoveSounds => Instance.moveSounds;

    [SerializeField] private AudioClip[] menuSounds;
    public static AudioClip[] MenuSounds => Instance.menuSounds;

    #endregion
    #region Dialogs

    [Space(2), Header("Dialogs")]

    [SerializeField] private Dialog startUpDialog;
    public static Dialog StartUpDialog => Instance.startUpDialog;

    #endregion
#if UNITY_EDITOR
    #region Editor

    [Space(2), Header("Editor")]

    [SerializeField, Range(0f, 1f)] private float gapBetweenFields = 0.05f;
    public static float GapBetweenFields => Instance.gapBetweenFields;

    [SerializeField] private Object[] palette;
    public static Object[] Palette => Instance.palette;

    [SerializeField] private GameObject hexFieldPrefab;
    public static GameObject HexFieldPrefab => Instance.hexFieldPrefab;

    [SerializeField] private GameObject sunPrefab;
    public static GameObject SunPrefab => Instance.sunPrefab;

    [SerializeField] private LightingSettings lightingSettings;
    public static LightingSettings LightingSettings => Instance.lightingSettings;
    #endregion
#endif
}