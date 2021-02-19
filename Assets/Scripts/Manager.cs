using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    // Serialized variables
    public Transform sun;

    // Hidden variables
    private bool onStartup = true;
    private bool showUseSigns = true;
    public static GUIManager GUI { get; private set; }
    public static DialogManager Dialogs { get; private set; }
    public static PlayerManager Players { get; private set; }
    public static LevelManager Levels { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this);
        GUI = GetComponent<GUIManager>();
        Dialogs = GetComponent<DialogManager>();
        Players = GetComponent<PlayerManager>();
        Levels = GetComponent<LevelManager>();

        SceneManager.sceneLoaded += OnLoadCallback;
        Levels.Load(1);
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
            GUI.MainMenu();
            onStartup = false;
        }
        else
        {
            GUI.ExitMenu();
            if (showUseSigns && Levels.CurrentIndex == 1)
            {
                showUseSigns = false;
                Dialogs.Show("\nClick on Signs to read them");
            }
        }

        if (Levels.current)
        {
            Levels.current.TurnsLeft = Levels.current.turns;
            Levels.current.MovesLeft = Levels.current.movesPerTurn;
            GUI.movesPerTurnDisplay.text = "";
            for (int i = 0; i < Levels.current.movesPerTurn; i++)
                GUI.movesPerTurnDisplay.text += "o ";
        }
    }

    private void Update() => sun.RotateAround(transform.position, Vector3.up, 8 * Time.deltaTime);
}