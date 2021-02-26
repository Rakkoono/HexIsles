using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    // Serialized variables
    public Transform sun;

    // Hidden variables
    private bool onStartup = true;
    public static bool showUseSigns = true;
    public static UIManager UI { get; private set; }
    public static DialogManager Dialogs { get; private set; }
    public static PlayerManager Players { get; private set; }
    public static LevelManager Levels { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UI = GetComponent<UIManager>();
        Dialogs = GetComponent<DialogManager>();
        Players = GetComponent<PlayerManager>();
        Levels = GetComponent<LevelManager>();

        if (PlayerPrefs.HasKey("completedLevels"))
            Levels.completed = PlayerPrefs.GetInt("completedLevels");

        SceneManager.sceneLoaded += OnLoadCallback;
        if (Levels.completed >= Levels.count)
            Levels.LoadNext();
        else
            Levels.LoadLatest();
        Players.source = GetComponent<AudioSource>();
    }

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        Levels.CurrentIndex = scene.buildIndex;
        Levels.current = FindObjectOfType<LevelData>();

        Players.players = FindObjectsOfType<Player>();
        Players.moved = new List<Player>();
        Players.undoList = new List<PlayerInfo[]>();

        if (Levels.CurrentIndex > 0 && onStartup)
        {
            UI.MainMenu();
            onStartup = false;
        }
        else UI.ExitMenu();

        if (Levels.current)
        {
            Levels.current.TurnsLeft = Levels.current.turns;
            Levels.current.MovesLeft = Levels.current.movesPerTurn;
            UI.movesPerTurnDisplay.text = "";
            for (int i = 0; i < Levels.current.movesPerTurn; i++)
                UI.movesPerTurnDisplay.text += "o ";
        }
    }

    [ContextMenu("Reset Player Prefs")]
    public void ResetPlayerPrefs() => PlayerPrefs.DeleteAll();

    private void Update() => sun.RotateAround(transform.position, Vector3.up, 8 * Time.deltaTime);

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("completedLevels", Levels.completed);
        PlayerPrefs.Save();
    }
}