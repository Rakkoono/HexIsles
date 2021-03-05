using UnityEngine;

[CreateAssetMenu(fileName = "Config File", menuName = "HexIsles/Config File", order = 0)]
public class Config : SingletonScriptableObject<Config>
{
    [SerializeField] private Level[] levels = new Level[15];
    public Level[] Levels => levels;

    [Space, Header("Game Over Types")]

    [SerializeField] private GameOver levelComplete;
    public GameOver LevelComplete => levelComplete;

    [SerializeField] private GameOver outOfTurns;
    public GameOver OutOfTurns => outOfTurns;

    [SerializeField] private GameOver allPetrified;
    public GameOver AllPetrified => allPetrified;

    [Space, Header("Camera Settings")]

    [SerializeField, Range(0f, 10f)] public float zoomSpeed = 5f;
    [SerializeField, Range(0f, 200f)] public float panSpeed = 100f;

    [Space]

    [SerializeField] private float zoomRangeMin = 1f;
    public float ZoomRangeMin => zoomRangeMin;

    [SerializeField] private float zoomRangeMax = 8f;
    public float ZoomRangeMax => zoomRangeMax;

}